using Lib.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Lib
{
    public class Contour
    {
        public float[] vertices;
        public float[] normals;
        public uint vertices_n;
    };

    public class Polygon
    {
        public Contour[] contours;
        public uint contours_n;
    };

    public class Triangulation
    {
        public float[] vertices;
        public float[] normals;
        public float[] texCoords;
        public uint[] indices;
        public uint vertices_n;
        public uint triangles_n;
        public int id;
        public float error;
    };

    public class Color : INext<Color>
    {
        public Color next { get; set; }
        public uint colorKind;
        public uint colorIndex;

        public sbyte[] rgb = new sbyte[3];
    };

    public class Connection : INext<Connection>
    {
        public enum Flags : sbyte
        {
            None = 0,
            HasCircularSide = 1 << 0,
            HasRectangularSide = 1 << 1
        };

        public Connection next { get; set; }

        public Geometry[] geo = new Geometry[2];
        public uint[] offset = new uint[2];
        public Vec3f p;
        public Vec3f d;
        public uint temp;
        public Flags flags;

        public void setFlag(Flags flag)
        {
            flags = (Flags)((sbyte)flags | (sbyte)flag);
        }
        public bool hasFlag(Flags flag)
        {
            return Convert.ToBoolean((sbyte)flags & (sbyte)flag);
        }

    };

    public class Geometry : INext<Geometry>
    {
        public enum Kind
        {
            Pyramid,
            Box,
            RectangularTorus,
            CircularTorus,
            EllipticalDish,
            SphericalDish,
            Snout,
            Cylinder,
            Sphere,
            Line,
            FacetGroup
        }
        public Geometry next { get; set; }                 // Next geometry in the list of geometries in group.
        public Triangulation triangulation;
        public Geometry next_comp;            // Next geometry in list of geometries of this composite
        public Connection[] connections = new Connection[6];
        public string colorName;

        public uint color = 0x202020u;

        public Kind kind;
        public uint id;

        public Mat3x4f M_3x4;
        public BBox3f bboxLocal;
        public BBox3f bboxWorld;
        public float sampleStartAngle;

        public class Pyramid
        {
            public float[] bottom = new float[2];
            public float[] top = new float[2];
            public float[] offset = new float[2];
            public float height;
        }

        public class Box
        {
            public float[] lengths = new float[3];
        }

        public class RectangularTorus
        {
            public float inner_radius;
            public float outer_radius;
            public float height;
            public float angle;
        }

        public class CircularTorus
        {
            public float offset;
            public float radius;
            public float angle;
        }

        public class EllipticalDish
        {
            public float baseRadius;
            public float height;
        }
        public class SphericalDish
        {
            public float baseRadius;
            public float height;
        }

        public class Snout
        {
            public float[] offset = new float[2];
            public float[] bshear = new float[2];
            public float[] tshear = new float[2];
            public float radius_b;
            public float radius_t;
            public float height;
        }

        public class Cylinder
        {
            public float radius;
            public float height;
        }

        public class Sphere
        {
            public float diameter;
        }

        public class Line
        {
            public float a;
            public float b;
        }

        public class FacetGroup
        {
            public Polygon[] polygons;
            public uint polygons_n;
        }

        public Pyramid pyramid;
        public Box box;
        public RectangularTorus rectangularTorus;
        public CircularTorus circularTorus;
        public EllipticalDish ellipticalDish;
        public SphericalDish sphericalDish;
        public Snout snout;
        public Cylinder cylinder;
        public Sphere sphere;
        public Line line;
        public FacetGroup facetGroup;
    };


    public class ListHeader<T> where T : class, INext<T>
    {
        public T first;
        public T last;

        public void clear()
        {
            first = last = null;
        }

        void insert(T item)
        {
            if (first == null)
            {
                first = last = item;
            }
            else
            {
                last.next = item;
                last = item;
            }
        }

        public static void insert(ListHeader<T> list, T item)
        {
            if (list.first == null)
            {
                list.first = list.last = item;
            }
            else
            {
                list.last.next = item;
                list.last = item;
            }
        }
    };

    public class Attribute : INext<Attribute>
    {
        public Attribute next { get; set; }
        public char? key = default;
        public char? val = default;
    };


    public class Group : INext<Group>
    {
        public Group() { }

        public enum Kind
        {
            File,
            Model,
            Group
        };

        public enum Flags
        {
            None = 0,
            ClientFlagStart = 1
        };

        public Group next { get; set; }
        public ListHeader<Group> groups;
        public ListHeader<Attribute> attributes;

        public Kind kind = Kind.Group;
        public Flags flags = Flags.None;

        public void setFlag(Flags flag)
        {
            flags = (Flags)((uint)flags | (uint)flag);
        }
        public void unsetFlag(Flags flag)
        {
            flags = (Flags)((uint)flags & (~(uint)flag));
        }
        public bool hasFlag(Flags flag)
        {
            return ((uint)flags & (uint)flag) != 0;
        }

        public class File
        {
            public char info;
            public char note;
            public char date;
            public char user;
            public char encoding;
        }

        public class Model
        {
            public ListHeader<Color> colors;
            public char project;
            public char name;
        }

        public class InnerGroup
        {
            public ListHeader<Geometry> geometries;
            public char name;
            public BBox3f bboxWorld;
            public uint material;
            public int id = 0;
            public float[] translation = new float[3];
            public uint clientTag;     // For use by passes to stuff temporary info
        }

        public File file;
        public Model model;
        public InnerGroup group;
    };


    public class DebugLine : INext<DebugLine>
    {
        public DebugLine next { get; set; }
        public float[] a = new float[3];
        public float[] b = new float[3];
        public uint color = 0xff0000u;
    };

    public class Store
    {
        public Store()
        {
            roots.clear();
            debugLines.clear();
            connections.clear();
            setErrorString("");
        }

        public void storeGroupIndexInGeometriesRecurse(Group group)
        {
            for (var child = group.groups.first; child != null; child = child.next)
            {
                storeGroupIndexInGeometriesRecurse(child);
            }
            if (group.kind == Group.Kind.Group)
            {
                for (var geo = group.group.geometries.first; geo != null; geo = geo.next)
                {
                    geo.id = (uint)group.group.id;
                    if (geo.triangulation != null)
                    {
                        geo.triangulation.id = group.group.id;
                    }
                }
            }
        }

        public void forwardGroupIdToGeometries()
        {
            for (var root = roots.first; root != null; root = root.next)
            {
                storeGroupIndexInGeometriesRecurse(root);
            }
        }

        public Color newColor(Group parent)
        {
            Debug.Assert(parent != null);
            Debug.Assert(parent.kind == Group.Kind.Model);
            var color = new Color();
            color.next = null;
            ListHeader<Color>.insert(parent.model.colors, color);
            return color;
        }

        public Geometry newGeometry(Group parent)
        {
            Debug.Assert(parent != null);
            Debug.Assert(parent.kind == Group.Kind.Group);

            var geo = arena.alloc<Geometry>();
            geo.next = null;
            geo.id = numGeometriesAllocated++;

            ListHeader<Geometry>.insert(parent.group.geometries, geo);
            return geo;
        }

        public Geometry cloneGeometry(Group parent, Geometry src)
        {
            var dst = newGeometry(parent);
            dst.kind = src.kind;
            dst.M_3x4 = src.M_3x4;
            dst.bboxLocal = src.bboxLocal;
            dst.bboxWorld = src.bboxWorld;
            dst.id = src.id;
            dst.sampleStartAngle = src.sampleStartAngle;
            switch (dst.kind)
            {
                case Geometry.Kind.Pyramid:
                case Geometry.Kind.Box:
                case Geometry.Kind.RectangularTorus:
                case Geometry.Kind.CircularTorus:
                case Geometry.Kind.EllipticalDish:
                case Geometry.Kind.SphericalDish:
                case Geometry.Kind.Snout:
                case Geometry.Kind.Cylinder:
                case Geometry.Kind.Sphere:
                case Geometry.Kind.Line:
                    dst.snout = src.snout;
                    break;
                case Geometry.Kind.FacetGroup:
                    dst.facetGroup.polygons_n = src.facetGroup.polygons_n;
                    dst.facetGroup.polygons = new Polygon[dst.facetGroup.polygons_n];
                    for (uint k = 0; k < dst.facetGroup.polygons_n; k++)
                    {
                        var dst_poly = dst.facetGroup.polygons[k];
                        var src_poly = src.facetGroup.polygons[k];
                        dst_poly.contours_n = src_poly.contours_n;
                        dst_poly.contours = new Contour[dst_poly.contours_n];
                        for (uint i = 0; i < dst_poly.contours_n; i++)
                        {
                            var dst_cont = dst_poly.contours[i];
                            var src_cont = src_poly.contours[i];
                            dst_cont.vertices_n = src_cont.vertices_n;
                            Array.Copy(src_cont.vertices, dst_cont.vertices, 3 * dst_cont.vertices_n);
                            Array.Copy(src_cont.normals, dst_cont.normals, 3 * dst_cont.vertices_n);
                        }
                    }
                    break;
                default:
                    Debug.Assert(false, "Geometry has invalid kind.");
                    break;
            }

            if (src.colorName != null)
            {
                dst.colorName = strings.intern(src.colorName);
            }
            dst.color = src.color;

            if (src.triangulation != null)
            {
                dst.triangulation = arena.alloc<Triangulation>();

                var stri = src.triangulation;
                var dtri = dst.triangulation;
                dtri.error = stri.error;
                dtri.id = stri.id;
                if (stri.vertices_n != 0)
                {
                    dtri.vertices_n = stri.vertices_n;
                    Array.Copy(stri.vertices, dtri.vertices, 3 * dtri.vertices_n);
                    Array.Copy(stri.normals, dtri.normals, 3 * dtri.vertices_n);
                    Array.Copy(stri.texCoords, dtri.texCoords, 2 * dtri.vertices_n);
                }
                if (stri.triangles_n != 0)
                {
                    dtri.triangles_n = stri.triangles_n;
                    Array.Copy(stri.indices, dtri.indices, 3 * dtri.triangles_n);
                }
            }

            return dst;
        }

        public Group getDefaultModel()
        {
            var file = roots.first;
            if (file == null)
            {
                file = newGroup(null, Group.Kind.File);
                file.file.info = strings.intern("");
                file.file.note = strings.intern("");
                file.file.date = strings.intern("");
                file.file.user = strings.intern("");
                file.file.encoding = strings.intern("");
            }
            var model = file.groups.first;
            if (model == null)
            {
                model = newGroup(file, Group.Kind.Model);
                model.model.project = strings.intern("");
                model.model.name = strings.intern("");
            }
            return model;
        }

        public Group newGroup(Group parent, Group.Kind kind)
        {
            var grp = new Group();

            if (parent == null)
            {
                ListHeader<Group>.insert(roots, grp);
            }
            else
            {
                ListHeader<Group>.insert(parent.groups, grp);
            }

            grp.kind = kind;
            numGroupsAllocated++;
            return grp;
        }

        public Group cloneGroup(Group parent, Group src)
        {
            var dst = newGroup(parent, src.kind);
            switch (src.kind)
            {
                case Group.Kind.File:
                    dst.file.info = strings.intern(src.file.info);
                    dst.file.note = strings.intern(src.file.note);
                    dst.file.date = strings.intern(src.file.date);
                    dst.file.user = strings.intern(src.file.user);
                    dst.file.encoding = strings.intern(src.file.encoding);
                    break;
                case Group.Kind.Model:
                    dst.model.project = strings.intern(src.model.project);
                    dst.model.name = strings.intern(src.model.name);
                    break;
                case Group.Kind.Group:
                    dst.group.name = strings.intern(src.group.name);
                    dst.group.bboxWorld = src.group.bboxWorld;
                    dst.group.material = src.group.material;
                    dst.group.id = src.group.id;
                    for (uint k = 0; k < 3; k++) dst.group.translation[k] = src.group.translation[k];
                    break;
                default:
                    Debug.Assert(false, "Group has invalid kind.");
                    break;
            }

            for (var src_att = src.attributes.first; src_att != null; src_att = src_att.next)
            {
                var dst_att = newAttribute(dst, strings.intern(src_att.key));
                dst_att.val = strings.intern(src_att.val);
            }

            return dst;
        }

        public Group findRootGroup(char name)
        {
            for (var file = roots.first; file != null; file = file.next)
            {
                Debug.Assert(file.kind == Group.Kind.File);
                for (var model = file.groups.first; model != null; model = model.next)
                {
                    //fprintf(stderr, "model '%s'\n", model.model.name);
                    Debug.Assert(model.kind == Group.Kind.Model);
                    for (var group = model.groups.first; group != null; group = group.next)
                    {
                        //fprintf(stderr, "group '%s' %p\n", group.group.name, (void*)group.group.name);
                        if (group.group.name == name) return group;
                    }
                }
            }
            return null;
        }

        public Attribute getAttribute(Group group, char key)
        {
            for (var attribute = group.attributes.first; attribute != null; attribute = attribute.next)
            {
                if (attribute.key == key) return attribute;
            }
            return null;
        }

        public Attribute newAttribute(Group group, char key)
        {
            var attribute = arena.alloc<Attribute>();
            attribute.key = key;
            ListHeader<Attribute>.insert(group.attributes, attribute);
            return attribute;
        }

        public void addDebugLine(float[] a, float[] b, uint color)
        {
            var line = arena.alloc<DebugLine>();
            for (uint k = 0; k < 3; k++)
            {
                line.a[k] = a[k];
                line.b[k] = b[k];
            }
            line.color = color;
            ListHeader<DebugLine>.insert(debugLines, line);
        }

        public Connection newConnection()
        {
            var connection = arena.alloc<Connection>();
            ListHeader<Connection>.insert(connections, connection);
            return connection;
        }

        public void apply(StoreVisitor visitor, Group group)
        {
            Debug.Assert(group.kind == Group.Kind.Group);
            visitor.beginGroup(group);

            if (group.attributes.first != null)
            {
                visitor.beginAttributes(group);
                for (var a = group.attributes.first; a != null; a = a.next)
                {
                    visitor.attribute(a.key, a.val);
                }
                visitor.endAttributes(group);
            }

            if (group.kind == Group.Kind.Group && group.group.geometries.first != null)
            {
                visitor.beginGeometries(group);
                for (var geo = group.group.geometries.first; geo != null; geo = geo.next)
                {
                    visitor.geometry(geo);
                }
                visitor.endGeometries();
            }

            visitor.doneGroupContents(group);

            if (group.groups.first != null)
            {
                visitor.beginChildren(group);
                for (var g = group.groups.first; g != null; g = g.next)
                {
                    apply(visitor, g);
                }
                visitor.endChildren();
            }

            visitor.EndGroup();
        }

        public void apply(StoreVisitor visitor)
        {
            visitor.init(this);
            do
            {
                for (var file = roots.first; file != null; file = file.next)
                {
                    Debug.Assert(file.kind == Group.Kind.File);
                    visitor.beginFile(file);

                    for (var model = file.groups.first; model != null; model = model.next)
                    {
                        Debug.Assert(model.kind == Group.Kind.Model);
                        visitor.beginModel(model);

                        for (var group = model.groups.first; group != null; group = group.next)
                        {
                            apply(visitor, group);
                        }
                        visitor.endModel();
                    }

                    visitor.endFile();
                }
            } while (visitor.done() == false);
        }

        public uint groupCount_() { return numGroups; }
        public uint groupCountAllocated() { return numGroupsAllocated; }
        public uint leafCount() { return numLeaves; }
        public uint emptyLeafCount() { return numEmptyLeaves; }
        public uint nonEmptyNonLeafCount() { return numNonEmptyNonLeaves; }
        public uint geometryCount_() { return numGeometries; }
        public uint geometryCountAllocated() { return numGeometriesAllocated; }

        public char errorString() { return error_str; }
        public void setErrorString(string str)
        {
            var l = str.Length;
            error_str = strings.intern(str, str + l);
        }

        public Group getFirstRoot() { return roots.first; }
        public Connection getFirstConnection() { return connections.first; }
        public DebugLine getFirstDebugLine() { return debugLines.first; }

        public Arena arena;
        public Arena arenaTriangulation;
        public Stats stats = default;
        public Connectivity conn = default;

        public StringInterning strings;

        public void updateCounts()
        {
            numGroups = 0;
            numLeaves = 0;
            numEmptyLeaves = 0;
            numNonEmptyNonLeaves = 0;
            numGeometries = 0;
            for (var root = roots.first; root != null; root = root.next)
            {
                updateCountsRecurse(root);
            }
        }

        public abstract void forwardGroupIdToGeometries();
        private uint numGroups = 0;
        private uint numGroupsAllocated = 0;
        private uint numLeaves = 0;
        private uint numEmptyLeaves = 0;
        private uint numNonEmptyNonLeaves = 0;
        private uint numGeometries = 0;
        private uint numGeometriesAllocated = 0;

        private char error_str = default;

        protected void updateCountsRecurse(Group group)
        {
            for (var child = group.groups.first; child != null; child = child.next)
            {
                updateCountsRecurse(child);
            }

            numGroups++;
            if (group.groups.first == null)
            {
                numLeaves++;
            }

            if (group.kind == Group.Kind.Group)
            {
                if (group.groups.first == null && group.group.geometries.first == null)
                {
                    numEmptyLeaves++;
                }

                if (group.groups.first != null && group.group.geometries.first != null)
                {
                    numNonEmptyNonLeaves++;
                }

                for (var geo = group.group.geometries.first; geo != null; geo = geo.next)
                {
                    numGeometries++;
                }
            }
        }

        private ListHeader<Group> roots;
        private ListHeader<DebugLine> debugLines;
        private ListHeader<Connection> connections;

    }
}
