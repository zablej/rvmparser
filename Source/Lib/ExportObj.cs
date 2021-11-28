using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using static Lib.Api.CommonHelper;

namespace Lib
{
    public class ExportObj : StoreVisitor, IDisposable
    {
        public bool groupBoundingBoxes = false;

        ~ExportObj()
        {
            if (@out != null)
            {
                fclose(@out);
            }
            if (mtl != null)
            {
                fclose(mtl);
            }
        }

        public bool open(string path_obj, string path_mtl)
        {
            if (!open_w(@out, path_obj)) return false;
            if (!open_w(mtl, path_mtl)) return false;

            var mtllib = path_mtl;
            var l = mtllib.LastIndexOf("/\\");
            if (l != -1)
            {
                mtllib = mtllib.Substring(l + 1);
            }

            fprintf(@out, "mtllib %s\n", mtllib);

            if (groupBoundingBoxes)
            {
                fprintf(mtl, "newmtl group_bbox\n");
                fprintf(mtl, "Ka 1 0 0\n");
                fprintf(mtl, "Kd 1 0 0\n");
                fprintf(mtl, "Ks 0 0 0\n");
            }

            return true;
        }

        public override void init(Store store)
        {
            Debug.Assert(@out != null);
            Debug.Assert(mtl != null);

            this.store = store;
            conn = store.conn;

            stack.accommodate(store.groupCountAllocated());

            string colorName = new string(new char[6]);
            for (var line = store.getFirstDebugLine(); line != null; line = line.next)
            {

                for (int k = 0; k < 6; k++)
                {
                    var v = (line.color >> (4 * k)) & 0xf;
                    if (v < 10) 
                        colorName.Remove(k).Insert(k, ('0' + v).ToString());
                    else 
                        colorName.Remove(k).Insert(k, ('a' + v - 10).ToString());
                }
                var name = store.strings.intern(colorName[0], colorName[6]);
                if (!definedColors.get((ulong)name))
                {
                    definedColors.insert((ulong)name, 1);
                    var r = (1f / 255f) * ((line.color >> 16) & 0xFF);
                    var g = (1f / 255f) * ((line.color >> 8) & 0xFF);
                    var b = (1f / 255f) * ((line.color) & 0xFF);
                    fprintf(mtl, "newmtl %s\n", name);
                    fprintf(mtl, "Ka %f %f %f\n", (2.f / 3.f) * r, (2.f / 3.f) * g, (2.f / 3.f) * b);
                    fprintf(mtl, "Kd %f %f %f\n", r, g, b);
                    fprintf(mtl, "Ks 0.5 0.5 0.5\n");
                }
                fprintf(@out, "usemtl %s\n", name);

                fprintf(@out, "v %f %f %f\n", line.a[0], line.a[1], line.a[2]);
                fprintf(@out, "v %f %f %f\n", line.b[0], line.b[1], line.b[2]);
                fprintf(@out, "l -1 -2\n");
                off_v += 2;
            }
        }

        public override void beginFile(Group group)
        {
            fprintf(@out, "# %s\n", group.file.info);
            fprintf(@out, "# %s\n", group.file.note);
            fprintf(@out, "# %s\n", group.file.date);
            fprintf(@out, "# %s\n", group.file.user);
        }

        public override void endFile()
        {
            fprintf(@out, "# End of file\n");
        }

        public override void beginModel(Group group)
        {
            fprintf(@out, "# Model project=%s, name=%s\n", group.model.project, group.model.name);
        }

        public override void endModel() { }

        public override void beginGroup(Group group)
        {
            for (uint i = 0; i < 3; i++) curr_translation[i] = group.group.translation[i];

            stack[stack_p++] = group.group.name;

            fprintf(@out, "o %s", stack[0]);
            for (uint i = 1; i < stack_p; i++)
            {
                fprintf(@out, "/%s", stack[i]);
            }
            fprintf(@out, "\n");


            //  fprintf(@out, "o %s\n", group.group.name);


            if (groupBoundingBoxes && !BBox3f.isEmpty(group.group.bboxWorld))
            {
                fprintf(@out, "usemtl group_bbox\n");
                wireBoundingBox(@out, off_v, group.group.bboxWorld);
            }

        }

        public override void EndGroup()
        {
            Debug.Assert(stack_p != 0);
            stack_p--;
        }

        public static void getMidpoint(Vec3f p, Geometry geo)
        {
            switch (geo.kind)
            {
                case Geometry.Kind.CircularTorus:
                    {
                        var ct = geo.circularTorus;
                        var c = (float)Math.Cos(0.5f * ct.angle);
                        var s = (float)Math.Sin(0.5f * ct.angle);
                        p.x = ct.offset * c;
                        p.y = ct.offset * s;
                        p.z = 0f;
                        break;
                    }
                default:
                    p = new Vec3f(0f);
                    break;
            }
            p = Vec3f.mul(geo.M_3x4, p);
        }

        public override void geometry(Geometry geometry)
        {
            var M = geometry.M_3x4;

            if (geometry.colorName == null)
            {
                geometry.colorName = store.strings.intern("default");
            }

            //if (geometry.kind == Geometry.Kind.Box) {
            //  geometry.colorName = store.strings.intern("blah-red");
            //  geometry.color = 0x880088;
            //}
            //if (geometry.kind == Geometry.Kind.Pyramid) {
            //  geometry.colorName = store.strings.intern("blah-green");
            //  geometry.color = 0x008800;
            //}
            //if (geometry.kind == Geometry.Kind.RectangularTorus) {
            //  geometry.colorName = store.strings.intern("blah-blue");
            //  geometry.color = 0x000088;
            //}
            //if (geometry.kind == Geometry.Kind.FacetGroup) {
            //  geometry.colorName = store.strings.intern("blah-redgg");
            //  geometry.color = 0x888800;
            //}

            if (!definedColors.get((ulong)geometry.colorName))
            {
                definedColors.insert((ulong)geometry.colorName, 1);

                var r = (1f / 255f) * ((geometry.color >> 16) & 0xFF);
                var g = (1f / 255f) * ((geometry.color >> 8) & 0xFF);
                var b = (1f / 255f) * ((geometry.color) & 0xFF);

                fprintf(mtl, "newmtl %s\n", geometry.colorName);
                fprintf(mtl, "Ka %f %f %f\n", (2f / 3f) * r, (2f / 3f) * g, (2f / 3f) * b);
                fprintf(mtl, "Kd %f %f %f\n", r, g, b);
                fprintf(mtl, "Ks 0.5 0.5 0.5\n");
            }

            fprintf(@out, "usemtl %s\n", geometry.colorName);

            var scale = 1f;

            if (geometry.kind == Geometry.Kind.Line)
            {
                var a = scale * Vec3f.mul(geometry.M_3x4, new Vec3f(geometry.line.a, 0, 0));
                var b = scale * Vec3f.mul(geometry.M_3x4, new Vec3f(geometry.line.b, 0, 0));
                fprintf(@out, "v %f %f %f\n", a.x, a.y, a.z);
                fprintf(@out, "v %f %f %f\n", b.x, b.y, b.z);
                fprintf(@out, "l -1 -2\n");
                off_v += 2;
            }
            else
            {
                Debug.Assert(geometry.triangulation != null);
                var tri = geometry.triangulation;

                if (tri.indices.Any())
                {
                    //fprintf(@out, "g\n");
                    if (geometry.triangulation.error != 0.f)
                    {
                        fprintf(@out, "# error=%f\n", geometry.triangulation.error);
                    }
                    for (uint i = 0; i < 3 * tri.vertices_n; i += 3)
                    {

                        var p = scale * Vec3f.mul(geometry.M_3x4, new Vec3f(tri.vertices + i));
                        Vec3f n = Vec3f.normalize(Vec3f.mul(new Mat3f(geometry.M_3x4.data), new Vec3f(tri.normals + i)));
                        if (!float.IsInfinity(n.x) || float.IsInfinity(n.y) || float.IsInfinity(n.z))
                        {
                            n = new Vec3f(1f, 0f, 0f);
                        }
                        fprintf(@out, "v %f %f %f\n", p.x, p.y, p.z);
                        fprintf(@out, "vn %f %f %f\n", n.x, n.y, n.z);
                    }
                    if (tri.texCoords != null)
                    {
                        for (uint i = 0; i < tri.vertices_n; i++)
                        {
                            var vt = new Vec2f(tri.texCoords + 2 * i);
                            fprintf(@out, "vt %f %f\n", vt.x, vt.y);
                        }
                    }
                    else
                    {
                        for (uint i = 0; i < tri.vertices_n; i++)
                        {
                            var p = scale * Vec3f.mul(geometry.M_3x4, new Vec3f(tri.vertices + 3 * i));
                            fprintf(@out, "vt %f %f\n", 0 * p.x, 0 * p.y);
                        }

                        for (uint i = 0; i < 3 * tri.triangles_n; i += 3)
                        {
                            var a = tri.indices[i + 0];
                            var b = tri.indices[i + 1];
                            var c = tri.indices[i + 2];
                            fprintf(@out, "f %d/%d/%d %d/%d/%d %d/%d/%d\n",
                                    a + off_v, a + off_t, a + off_n,
                                    b + off_v, b + off_t, b + off_n,
                                    c + off_v, c + off_t, c + off_n);
                        }
                    }

                    off_v += tri.vertices_n;
                    off_n += tri.vertices_n;
                    off_t += tri.vertices_n;
                }
            }
        }

        //if (primitiveBoundingBoxes) {
        //  fprintf(@out, "usemtl magenta\n");

        //  for (unsigned i = 0; i < 8; i++) {
        //    float px = (i & 1) ? geometry.bbox[0] : geometry.bbox[3];
        //    float py = (i & 2) ? geometry.bbox[1] : geometry.bbox[4];
        //    float pz = (i & 4) ? geometry.bbox[2] : geometry.bbox[5];

        //    float Px = M[0] * px + M[3] * py + M[6] * pz + M[9];
        //    float Py = M[1] * px + M[4] * py + M[7] * pz + M[10];
        //    float Pz = M[2] * px + M[5] * py + M[8] * pz + M[11];

        //    fprintf(@out, "v %f %f %f\n", Px, Py, Pz);
        //  }
        //  fprintf(@out, "l %d %d %d %d %d\n",
        //          off_v + 0, off_v + 1, off_v + 3, off_v + 2, off_v + 0);
        //  fprintf(@out, "l %d %d %d %d %d\n",
        //          off_v + 4, off_v + 5, off_v + 7, off_v + 6, off_v + 4);
        //  fprintf(@out, "l %d %d\n", off_v + 0, off_v + 4);
        //  fprintf(@out, "l %d %d\n", off_v + 1, off_v + 5);
        //  fprintf(@out, "l %d %d\n", off_v + 2, off_v + 6);
        //  fprintf(@out, "l %d %d\n", off_v + 3, off_v + 7);
        //  off_v += 8;
        //}


        //for (unsigned k = 0; k < 6; k++) {
        //  var other = geometry.conn_geo[k];
        //  if (geometry < other) {
        //    fprintf(@out, "usemtl blue_line\n");
        //    float p[3];
        //    getMidpoint(p, geometry);
        //    fprintf(@out, "v %f %f %f\n", p[0], p[1], p[2]);
        //    getMidpoint(p, other);
        //    fprintf(@out, "v %f %f %f\n", p[0], p[1], p[2]);
        //    fprintf(@out, "l %d %d\n", off_v, off_v + 1);

        //    off_v += 2;
        //  }
        //}

        private StreamWriter @out = null;
        private StreamWriter mtl = null;
        private Map definedColors;
        private Store store = null;
        private Buffer<string> stack;
        private uint stack_p = 0;
        private uint off_v = 1;
        private uint off_n = 1;
        private uint off_t = 1;
        private Connectivity conn = null;
        private float[] curr_translation = new float[3] { 0, 0, 0 };

        private bool anchors = false;
        private bool primitiveBoundingBoxes = false;
        private bool compositeBoundingBoxes = false;
        private bool disposedValue;

        public static bool open_w(StreamWriter f, string path)
        {
#if _WIN32
    var err = fopen_s(f, path, "w");
    if (err == 0) return true;

    char buf[1024];
    if (strerror_s(buf, sizeof(buf), err) != 0) {
      buf[0] = '\0';
    }
    fprintf(stderr, "Failed to open %s for writing: %s", path, buf);
#else
            f = fopen(path, "w");
            if (f != null) return true;

            using (var stderr = new StreamWriter(Console.OpenStandardError()))
                fprintf(stderr, "Failed to open %s for writing.", path);
#endif
            return false;
        }

        public static void wireBoundingBox(StreamWriter @out, uint off_v, BBox3f bbox)
        {
            for (uint i = 0; i < 8; i++)
            {
                float px = (i & 1) != 0 ? bbox.min[0] : bbox.min[3];
                float py = (i & 2) != 0 ? bbox.min[1] : bbox.min[4];
                float pz = (i & 4) != 0 ? bbox.min[2] : bbox.min[5];
                fprintf(@out, "v %f %f %f\n", px, py, pz);
            }
            fprintf(@out, "l %d %d %d %d %d\n",
                    off_v + 0, off_v + 1, off_v + 3, off_v + 2, off_v + 0);
            fprintf(@out, "l %d %d %d %d %d\n",
                    off_v + 4, off_v + 5, off_v + 7, off_v + 6, off_v + 4);
            fprintf(@out, "l %d %d\n", off_v + 0, off_v + 4);
            fprintf(@out, "l %d %d\n", off_v + 1, off_v + 5);
            fprintf(@out, "l %d %d\n", off_v + 2, off_v + 6);
            fprintf(@out, "l %d %d\n", off_v + 3, off_v + 7);
            off_v += 8;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}