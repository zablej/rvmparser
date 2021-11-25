using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Common
{
    public class QueueItem
    {
        public Geometry from;
        public Connection connection;
        public Vec3f upWorld;
    };

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

    public class Align
    {
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
            var N = Mat3f(M.data);
            var N_inv = inverse(N);
            var ct = geo.circularTorus;
            var c = std.cos(ct.angle);
            var s = std.sin(ct.angle);

            var upLocal = normalize(mul(N_inv, upWorld));

            if (offset == 1)
            {
                // rotate back to xz
                upLocal = Vec3f(c * upLocal.x + s * upLocal.y,
                                -s * upLocal.x + c * upLocal.y,
                                upLocal.z);
            }
            geo.sampleStartAngle = std.atan2(upLocal.z, upLocal.x);
            if (!std.isfinite(geo.sampleStartAngle))
            {
                geo.sampleStartAngle = 0;
            }

            var ci = std.cos(geo.sampleStartAngle);
            var si = std.sin(geo.sampleStartAngle);
            var co = std.cos(ct.angle);
            var so = std.sin(ct.angle);

            Vec3f upNew(ci, 0.f, si);

            var upNewWorld = new Vec3f[2];
            upNewWorld[0] = mul(N, upNew);
            upNewWorld[1] = mul(N, Vec3f(c * upNew.x - s * upNew.y,
                                         s * upNew.x + c * upNew.y,
                                         upNew.z));

            if (true)
            {
                Vec3f p0(ct.radius* ci +ct.offset,
               0.f,
               ct.radius* si);

                Vec3f p1((ct.radius* ci +ct.offset) *co,
               (ct.radius * ci + ct.offset) * so,
               ct.radius* si);


                var a0 = mul(geo.M_3x4, p0);
                var b0 = a0 + 1.5f * ct.radius * upNewWorld[0];

                var a1 = mul(geo.M_3x4, p1);
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
            }

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
            var M_inv = inverse(Mat3f(geo.M_3x4.data));

            var upn = normalize(upWorld);

            var upLocal = mul(M_inv, upn);
            upLocal.z = 0.f;  // project to xy-plane

            geo.sampleStartAngle = std.atan2(upLocal.y, upLocal.x);
            if (!std.isfinite(geo.sampleStartAngle))
            {
                geo.sampleStartAngle = 0.f;
            }

            Vec3f upNewWorld = mul(Mat3f(geo.M_3x4.data), Vec3f(std.cos(geo.sampleStartAngle),
                                                                 std.sin(geo.sampleStartAngle),
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
            var time0 = std.chrono.high_resolution_clock.now();
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
                if (std.abs(d.x) > std.abs(d.y) && std.abs(d.x) > std.abs(d.z))
                {
                    b = Vec3f(0, 1, 0);
                }
                else
                {
                    b = Vec3f(1, 0, 0);
                }

                var upWorld = normalize(cross(d, b));
                Debug.Assert(std.isfinite(lengthSquared(upWorld)));

                context.front = 0;
                context.back = 0;
                enqueue(context, null, connection, upWorld);
                do
                {
                    processItem(context);
                } while (context.front < context.back);

                context.connectedComponents++;
            }
            var time1 = std.chrono.high_resolution_clock.now();
            var e0 = std.chrono.duration_cast<std.chrono.milliseconds>((time1 - time0)).count();

            logger(0, "%d connected components in %d circular connections (%lldms).", context.connectedComponents, context.circularConnections, e0);
        }
    }
}
