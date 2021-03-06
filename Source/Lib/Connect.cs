using Lib.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Lib
{
    public class Connect
    {
        public static uint[] colors =
          {
            0x0000AA,
            0x00AA00,
            0x00AAAA,
            0xAA0000,
            0xAA00AA,
            0xAA5500
          };

        public static void connect(Store store, Logger logger)
        {

            var context = new Context();
            context.store = store;
            context.logger = logger;

            context.anchors_max = 6 * store.geometryCountAllocated();
            context.anchors.accommodate(context.anchors_max);


            var stopwatch = Stopwatch.StartNew();
            context.anchors_n = 0;
            for (var root = store.getFirstRoot(); root != null; root = root.next)
            {
                for (var model = root.groups.first; model != null; model = model.next)
                {
                    for (var group = model.groups.first; group != null; group = group.next)
                    {
                        recurse(context, group);
                    }
                }
            }
            for (uint i = 0; i < context.anchors_n; i++)
            {
                var a = context.anchors[i];
                Debug.Assert(a.matched == false);

                var b = a.p + 0.02f * a.d;

                //if (a.geo.kind == Geometry.Kind.Pyramid) {
                //  context.store.addDebugLine(a.p.data, b.data, 0x003300);
                //}
                //else {
                //  context.store.addDebugLine(a.p.data, b.data, 0xff0000);
                //}
            }
            stopwatch.Stop();
            var e0 = stopwatch.ElapsedMilliseconds;

            logger(0, "Matched %u of %u anchors (%lldms).", context.anchors_matched, context.anchors_total, e0);

        }

        public static void connect(Context context, uint off)
        {
            var a = context.anchors.data();
            var a_n = context.anchors_n;
            var e = context.epsilon;
            var ee = e * e;
            Debug.Assert(off <= a_n);

            std.sort(a + off, a + a_n, [](a, b) { return a.p.x < b.p.x; });

            for (uint j = off; j < a_n; j++)
            {
                if (a[j].matched) continue;

                for (uint i = j + 1; i < a_n && a[i].p.x <= a[j].p.x + e; i++)
                {

                    bool canMatch = a[i].matched == false;
                    bool close = Vec3f.distanceSquared(a[j].p, a[i].p) <= ee;
                    bool aligned = Vec3f.dot(a[j].d, a[i].d) < -0.98f;

                    if (canMatch && close && aligned)
                    {

                        var connection = context.store.newConnection();
                        connection.geo[0] = a[j].geo;
                        connection.geo[1] = a[i].geo;
                        connection.offset[0] = a[j].o;
                        connection.offset[1] = a[i].o;
                        connection.p = a[j].p;
                        connection.d = a[j].d;
                        connection.flags = Connection.Flags.None;
                        connection.setFlag(a[i].flags);
                        connection.setFlag(a[j].flags);

                        a[j].geo.connections[a[j].o] = connection;
                        a[i].geo.connections[a[i].o] = connection;

                        a[j].matched = true;
                        a[i].matched = true;
                        context.anchors_matched += 2;

                        //context.store.addDebugLine((a[j].p + 0.03f*a[j].d).data,
                        //                             (a[i].p + 0.03f*a[i].d).data,
                        //                             0x0000ff);
                    }
                }
            }

            // Remove matched anchors.
            for (uint j = off; j < a_n;)
            {
                if (a[j].matched)
                {
                    CommonHelper.swap(ref a[j], ref a[--a_n]);
                }
                else
                {
                    j++;
                }
            }
            Debug.Assert(off <= a_n);

            context.anchors_n = a_n;
        }

        public static void addAnchor(Context context, Geometry geo, Vec3f p, Vec3f d, uint o, Connection.Flags flags)
        {

            var a = new Anchor();
            a.geo = geo;
            a.p = Vec3f.mul(new Mat3x4f(geo.M_3x4), p);
            a.d = Vec3f.normalize(Vec3f.mul(new Mat3f(geo.M_3x4.data), d));
            a.o = o;
            a.flags = flags;

            //context.store.addDebugLine(a.p.data, (a.p + 0.02*a.d).data, 0x008800);

            Debug.Assert(context.anchors_n < context.anchors_max);
            context.anchors[context.anchors_n++] = a;
            context.anchors_total++;
        }

        public static void recurse(Context context, Group group)
        {
            var offset = context.anchors_n;
            for (var child = group.groups.first; child != null; child = child.next)
            {
                recurse(context, child);
            }
            for (var geo = group.group.geometries.first; geo != null; geo = geo.next)
            {
                switch (geo.kind)
                {
                    case Geometry.Kind.Pyramid:
                        {
                            var b = 0.5f * new Vec2f(geo.pyramid.bottom);
                            var t = 0.5f * new Vec2f(geo.pyramid.top);
                            var m = 0.5f * (b + t);
                            var o = 0.5f * new Vec2f(geo.pyramid.offset);

                            var h = 0.5f * geo.pyramid.height;

                            var M = geo.M_3x4;
                            var N = new Mat3f(M.data);

                            Vec3f[] n = new Vec3f[] {
                           new Vec3f(0f, -h,  (-t.y + o.y) - (-b.y - o.y)),
                           new Vec3f(h, 0f, -((t.x + o.x) - (b.x - o.x))),
                           new Vec3f(0f,  h, -((t.y + o.y) - (b.y - o.y))),
                           new Vec3f(-h, 0f,  (-t.x + o.x) - (-b.x - o.x)),
                           new Vec3f(0f, 0f, -1f),
                           new Vec3f(0f, 0f, 1f)
                        };
                            Vec3f[] p = new Vec3f[] {
                          new Vec3f(0f, -m.y, 0f),
                          new Vec3f(m.x, 0f, 0f),
                          new Vec3f(0f, m.y, 0f),
                          new Vec3f(-m.x, 0f, 0f),
                          new Vec3f(-o.x, -o.y, -h),
                          new Vec3f(o.x, o.y, h)
                        };
                            for (uint i = 0; i < 6; i++)
                            {
                                addAnchor(context, geo, p[i], n[i], i, Connection.Flags.HasRectangularSide);
                            }
                            break;
                        }

                    case Geometry.Kind.Box:
                        {
                            var box = geo.box;
                            Vec3f[] n = {
                            new Vec3f(-1,  0,  0), new Vec3f(1,  0,  0),
                            new Vec3f(0, -1,  0), new Vec3f(0,  1,  0),
                            new Vec3f(0,  0, -1), new Vec3f(0,  0,  1)
                        };
                            var xp = 0.5f * box.lengths[0]; var xm = -xp;
                            var yp = 0.5f * box.lengths[1]; var ym = -yp;
                            var zp = 0.5f * box.lengths[2]; var zm = -zp;
                            Vec3f[] p = {
                          new Vec3f(xm, 0f, 0f), new Vec3f(xp, 0f, 0f ),
                          new Vec3f(0f, ym, 0f ), new Vec3f(0f, yp, 0f ),
                          new Vec3f(0f, 0f, zm ), new Vec3f(0f, 0f, zp )
                        };
                            for (uint i = 0; i < 6; i++) addAnchor(context, geo, p[i], n[i], i, Connection.Flags.HasRectangularSide);
                            break;
                        }

                    case Geometry.Kind.RectangularTorus:
                        {
                            var rt = geo.rectangularTorus;
                            var c = (float)Math.Cos(rt.angle);
                            var s = (float)Math.Sin(rt.angle);
                            var m = 0.5f * (rt.inner_radius + rt.outer_radius);
                            Vec3f[] n = new Vec3f[2] { new Vec3f(0, -1, 0f), new Vec3f(-s, c, 0f) };
                            Vec3f[] p = new Vec3f[2] { new Vec3f(geo.circularTorus.offset, 0, 0f), new Vec3f(m * c, m * s, 0f) };
                            for (uint i = 0; i < 2; i++)
                                addAnchor(context, geo, p[i], n[i], i, Connection.Flags.HasRectangularSide);
                            break;
                        }

                    case Geometry.Kind.CircularTorus:
                        {
                            var ct = geo.circularTorus;
                            var c = (float)Math.Cos(ct.angle);
                            var s = (float)Math.Sin(ct.angle);
                            Vec3f[] n = new Vec3f[] { new Vec3f(0, -1, 0f), new Vec3f(-s, c, 0f) };
                            Vec3f[] p = new Vec3f[] { new Vec3f(ct.offset, 0, 0f), new Vec3f(ct.offset * c, ct.offset * s, 0f) };
                            for (uint i = 0; i < 2; i++)
                                addAnchor(context, geo, p[i], n[i], i, Connection.Flags.HasCircularSide);
                            break;
                        }

                    case Geometry.Kind.EllipticalDish:
                    case Geometry.Kind.SphericalDish:
                        {
                            addAnchor(context, geo, new Vec3f(0, 0, 0), new Vec3f(0, 0, -1), 0, Connection.Flags.HasCircularSide);
                            break;
                        }

                    case Geometry.Kind.Snout:
                        {
                            var sn = geo.snout;
                            Vec3f[] n = new Vec3f[2] {
                          new Vec3f((float)(Math.Sin(sn.bshear[0])*Math.Cos(sn.bshear[1])), (float)(Math.Sin(sn.bshear[1])), -(float)(Math.Cos(sn.bshear[0])*Math.Cos(sn.bshear[1]))),
                          new Vec3f(-(float)(Math.Sin(sn.tshear[0])*Math.Cos(sn.tshear[1])), -(float)(Math.Sin(sn.tshear[1])), (float)(Math.Cos(sn.tshear[0])*Math.Cos(sn.tshear[1])))
                        };
                            Vec3f[] p = new Vec3f[] {
                          new Vec3f(-0.5f*sn.offset[0], -0.5f*sn.offset[1], -0.5f*sn.height ),
                          new Vec3f(0.5f*sn.offset[0], 0.5f*sn.offset[1], 0.5f*sn.height )
                        };
                            for (uint i = 0; i < 2; i++)
                                addAnchor(context, geo, p[i], n[i], i, Connection.Flags.HasCircularSide);
                            break;
                        }

                    case Geometry.Kind.Cylinder:
                        {
                            Vec3f[] d = new Vec3f[] { new Vec3f(0, 0, -1f), new Vec3f(0, 0, 1f) };
                            Vec3f[] p = new Vec3f[] { new Vec3f(0, 0, -0.5f * geo.cylinder.height), Vec3f(0, 0, 0.5f * geo.cylinder.height) };
                            for (uint i = 0; i < 2; i++)
                                addAnchor(context, geo, p[i], d[i], i, Connection.Flags.HasCircularSide);
                            break;
                        }

                    case Geometry.Kind.Sphere:
                    case Geometry.Kind.FacetGroup:
                    case Geometry.Kind.Line:
                        break;

                    default:
                        Debug.Assert(false, "Unhandled primitive type");
                        break;
                }
            }
            connect(context, offset);
        }
    }

    public class Anchor
    {
        public Geometry geo = null;
        public Vec3f p;
        public Vec3f d;
        public uint o;
        public Connection.Flags flags;
        public byte matched = 0;
    }

    public class Context
    {
        public Store store;
        public Logger logger;
        public Buffer<Anchor> anchors;
        public float epsilon = 0.001f;
        public uint anchors_n = 0;

        public uint anchors_max = 0;

        public uint anchors_total = 0;
        public uint anchors_matched = 0;
    }
}
