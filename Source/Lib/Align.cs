using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Lib
{
    public class QueueItem
    {
        public Geometry from;
        public Connection connection;
        public Vec3f upWorld;
    };

    public class Align
    {
        public class Context
        {
            public Buffer<QueueItem> queue;
            public Logger logger = null;
            public Store store = null;
            public uint front = 0;
            public uint back = 0;
            public uint connectedComponents = 0;
            public uint circularConnections = 0;
            public uint connections = 0;

        };

        public static void enqueue(Context context, Geometry from, Connection connection, Vec3f upWorld)
        {
            connection.temp = 1;

            Debug.Assert(context.back < context.connections);
            context.queue[context.back].from = from;
            context.queue[context.back].connection = connection;
            context.queue[context.back].upWorld = upWorld;

            context.back++;
        }

        public static void handleCircularTorus(Context context, Geometry geo, uint offset, Vec3f upWorld)
        {
            var M = geo.M_3x4;
            var N = new Mat3f(M.data);
            var N_inv = LinAlgOps.inverse(N);
            var ct = geo.circularTorus;
            var c = Math.Cos(ct.angle);
            var s = Math.Sin(ct.angle);

            var upLocal = Vec3f.normalize(Vec3f.mul(N_inv, upWorld));

            if (offset == 1)
            {
                // rotate back to xz
                upLocal = new Vec3f((float)(c * upLocal.x + s * upLocal.y),
                                (float)(-s * upLocal.x + c * upLocal.y),
                                upLocal.z);
            }
            geo.sampleStartAngle = (float)Math.Atan2(upLocal.z, upLocal.x);
            if (float.IsInfinity(geo.sampleStartAngle))
            {
                geo.sampleStartAngle = 0;
            }

            var ci = (float)Math.Cos(geo.sampleStartAngle);
            var si = (float)Math.Sin(geo.sampleStartAngle);
            var co = (float)Math.Cos(ct.angle);
            var so = (float)Math.Sin(ct.angle);

            Vec3f upNew = new Vec3f(ci, 0f, si);

            var upNewWorld = new Vec3f[2];
            upNewWorld[0] = Vec3f.mul(N, upNew);
            upNewWorld[1] = Vec3f.mul(N, new Vec3f((float)(c * upNew.x - s * upNew.y),
                                         (float)(s * upNew.x + c * upNew.y),
                                         upNew.z));


            var p0 = new Vec3f(ct.radius * ci + ct.offset,
                               0f,
                               ct.radius * si);

            var p1 = new Vec3f((ct.radius* ci +ct.offset) *co,
               (ct.radius * ci + ct.offset) * so,
               ct.radius* si);


            var a0 = Vec3f.mul(geo.M_3x4, p0);
            var b0 = a0 + 1.5f * ct.radius * upNewWorld[0];

            var a1 = Vec3f.mul(geo.M_3x4, p1);
            var b1 = a1 + 1.5f * ct.radius * upNewWorld[1];

            //if (context.front == 1) {
            //  if (geo.connections[0]) context.store.addDebugLine(a0.data, b0.data, 0x00ffff);
            //  if (geo.connections[1]) context.store.addDebugLine(a1.data, b1.data, 0x00ff88);
            //}
            //else if (offset == 0) {
            //  if (geo.connections[0]) context.store.addDebugLine(a0.data, b0.data, 0x0000ff);
            //  if (geo.connections[1]) context.store.addDebugLine(a1.data, b1.data, 0x000088);
            //}
            //else {
            //  if (geo.connections[0]) context.store.addDebugLine(a0.data, b0.data, 0x000088);
            //  if (geo.connections[1]) context.store.addDebugLine(a1.data, b1.data, 0x0000ff);
            //}


            for (uint k = 0; k < 2; k++)
            {
                var con = geo.connections[k];
                if (con != null && !con.hasFlag(Connection.Flags.HasRectangularSide) && con.temp == 0)
                {
                    enqueue(context, geo, con, upNewWorld[k]);
                }
            }


        }


        public static void handleCylinderSnoutAndDish(Context context, Geometry geo, uint offset, Vec3f upWorld)
        {
            var M_inv = LinAlgOps.inverse(new Mat3f(geo.M_3x4.data));

            var upn = Vec3f.normalize(upWorld);

            var upLocal = Vec3f.mul(M_inv, upn);
            upLocal.z = 0f;  // project to xy-plane

            geo.sampleStartAngle = (float)Math.Atan2(upLocal.y, upLocal.x);
            if (float.IsInfinity(geo.sampleStartAngle))
            {
                geo.sampleStartAngle = 0f;
            }

            Vec3f upNewWorld = Vec3f.mul(new Mat3f(geo.M_3x4.data), new Vec3f((float)Math.Cos(geo.sampleStartAngle),
                                                                 (float)Math.Sin(geo.sampleStartAngle),
                                                                 0));

            for (uint k = 0; k < 2; k++)
            {
                var con = geo.connections[k];
                if (con != null && !con.hasFlag(Connection.Flags.HasRectangularSide) && con.temp == 0)
                {
                    enqueue(context, geo, con, upNewWorld);
                }
            }
        }

        public static void processItem(Context context)
        {
            var item = context.queue[context.front++];

            for (uint i = 0; i < 2; i++)
            {
                if (item.from != item.connection.geo[i])
                {
                    var geo = item.connection.geo[i];
                    switch (geo.kind)
                    {

                        case Geometry.Kind.Pyramid:
                        case Geometry.Kind.Box:
                        case Geometry.Kind.RectangularTorus:
                        case Geometry.Kind.Sphere:
                        case Geometry.Kind.Line:
                        case Geometry.Kind.FacetGroup:
                            Debug.Assert(false, "Got geometry with non-circular intersection.");
                            break;

                        case Geometry.Kind.Snout:
                        case Geometry.Kind.EllipticalDish:
                        case Geometry.Kind.SphericalDish:
                        case Geometry.Kind.Cylinder:
                            handleCylinderSnoutAndDish(context, geo, item.connection.offset[i], item.upWorld);
                            break;

                        case Geometry.Kind.CircularTorus:
                            handleCircularTorus(context, geo, item.connection.offset[i], item.upWorld);
                            break;

                        default:
                            Debug.Assert(false, "Illegal kind");
                            break;
                    }
                }
            }
        }

        public static void align(Store store, Logger logger)
        {
            var context = new Context();
            context.logger = logger;
            context.store = store;
            var stopwatch = Stopwatch.StartNew();
            for (var connection = store.getFirstConnection(); connection != null; connection = connection.next)
            {
                connection.temp = 0;

                if (connection.flags == Connection.Flags.HasCircularSide)
                {
                    context.circularConnections++;
                }
                context.connections++;
            }


            context.queue.accommodate(context.connections);
            for (var connection = store.getFirstConnection(); connection != null; connection = connection.next)
            {
                if (connection.temp != 0 || connection.hasFlag(Connection.Flags.HasRectangularSide))
                    continue;

                // Create an arbitrary vector in plane of intersection as seed.
                var d = connection.d;
                Vec3f b;
                if (Math.Abs(d.x) > Math.Abs(d.y) && Math.Abs(d.x) > Math.Abs(d.z))
                {
                    b = new Vec3f(0, 1, 0);
                }
                else
                {
                    b = new Vec3f(1, 0, 0);
                }

                var upWorld = Vec3f.normalize(Vec3f.cross(d, b));
                Debug.Assert(!float.IsInfinity(Vec3f.lengthSquared(upWorld)));

                context.front = 0;
                context.back = 0;
                enqueue(context, null, connection, upWorld);
                do
                {
                    processItem(context);
                } while (context.front < context.back);

                context.connectedComponents++;
            }
            stopwatch.Stop();
            var e0 = stopwatch.ElapsedMilliseconds;

            logger(0, "%d connected components in %d circular connections (%lldms).", context.connectedComponents, context.circularConnections, e0);
        }
    }
}
