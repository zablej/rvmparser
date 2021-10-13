using System;
using System.Collections.Generic;
using System.Diagnostics;

public static class GlobalMembers
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
		var N = new Mat3f(M.data);
		var N_inv = inverse(N);
		var ct = geo.circularTorus;
		var c = Math.Cos(ct.angle);
		var s = Math.Sin(ct.angle);

		var upLocal = normalize(mul(N_inv, upWorld));

		if (offset == 1)
		{
		  // rotate back to xz
		  upLocal = new Vec3f(c * upLocal.x + s * upLocal.y, -s * upLocal.x + c * upLocal.y, upLocal.z);
		}
		geo.sampleStartAngle = Math.Atan2(upLocal.z, upLocal.x);
		if (!!Double.IsInfinity(geo.sampleStartAngle) && !Double.IsNaN(geo.sampleStartAngle))
		{
		  geo.sampleStartAngle = 0.0f;
		}

		var ci = Math.Cos(geo.sampleStartAngle);
		var si = Math.Sin(geo.sampleStartAngle);
		var co = Math.Cos(ct.angle);
		var so = Math.Sin(ct.angle);

		Vec3f upNew = new Vec3f(ci, 0.0f, si);

		Vec3f[] upNewWorld = Arrays.InitializeWithDefaultInstances<Vec3f>(2);
		upNewWorld[0] = mul(N, upNew);
		upNewWorld[1] = mul(N, new Vec3f(c * upNew.x - s * upNew.y, s * upNew.x + c * upNew.y, upNew.z));

		if (true)
		{
		  Vec3f p0 = new Vec3f(ct.radius * ci + ct.offset, 0.0f, ct.radius * si);

		  Vec3f p1 = new Vec3f((ct.radius * ci + ct.offset) * co, (ct.radius * ci + ct.offset) * so, ct.radius * si);


		  var a0 = mul(geo.M_3x4, p0);
		  var b0 = a0 + 1.5f * ct.radius * upNewWorld[0];

		  var a1 = mul(geo.M_3x4, p1);
		  var b1 = a1 + 1.5f * ct.radius * upNewWorld[1];

		  //if (context.front == 1) {
		  //  if (geo->connections[0]) context.store->addDebugLine(a0.data, b0.data, 0x00ffff);
		  //  if (geo->connections[1]) context.store->addDebugLine(a1.data, b1.data, 0x00ff88);
		  //}
		  //else if (offset == 0) {
		  //  if (geo->connections[0]) context.store->addDebugLine(a0.data, b0.data, 0x0000ff);
		  //  if (geo->connections[1]) context.store->addDebugLine(a1.data, b1.data, 0x000088);
		  //}
		  //else {
		  //  if (geo->connections[0]) context.store->addDebugLine(a0.data, b0.data, 0x000088);
		  //  if (geo->connections[1]) context.store->addDebugLine(a1.data, b1.data, 0x0000ff);
		  //}
		}

		for (uint k = 0; k < 2; k++)
		{
		  var con = geo.connections[k];
		  if (con != null && !con.hasFlag(Connection.Flags.HasRectangularSide) && con.temp == 0)
		  {
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: enqueue(context, geo, con, upNewWorld[k]);
			enqueue(context, geo, new Connection(con), upNewWorld[k]);
		  }
		}


	  }


	  public static void handleCylinderSnoutAndDish(Context context, Geometry geo, uint offset, Vec3f upWorld)
	  {
		var M_inv = inverse(new Mat3f(geo.M_3x4.data));

		var upn = normalize(upWorld);

		var upLocal = mul(M_inv, upn);
		upLocal.z = 0.0f; // project to xy-plane

		geo.sampleStartAngle = Math.Atan2(upLocal.y, upLocal.x);
		if (!!Double.IsInfinity(geo.sampleStartAngle) && !Double.IsNaN(geo.sampleStartAngle))
		{
		  geo.sampleStartAngle = 0.0f;
		}

		Vec3f upNewWorld = mul(new Mat3f(geo.M_3x4.data), new Vec3f(Math.Cos(geo.sampleStartAngle), Math.Sin(geo.sampleStartAngle), 0.0f));

		for (uint k = 0; k < 2; k++)
		{
		  var con = geo.connections[k];
		  if (con != null && !con.hasFlag(Connection.Flags.HasRectangularSide) && con.temp == 0)
		  {
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: enqueue(context, geo, con, upNewWorld);
			enqueue(context, geo, new Connection(con), upNewWorld);
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
			  Debug.Assert(false && "Got geometry with non-circular intersection.");
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
			  Debug.Assert(false && "Illegal kind");
			  break;
			}
		  }
		}
	  }


	public static void align(Store store, Logger logger)
	{
	  Context context = new Context();
	  context.logger = logger;
	  context.store = store;
	  var time0 = std::chrono.high_resolution_clock.now();
	  for (var * connection = store.getFirstConnection(); connection != null; connection = connection.next)
	  {
		connection.temp = 0;

		if (connection.flags == Connection.Flags.HasCircularSide)
		{
		  context.circularConnections++;
		}
		context.connections++;
	  }


	  context.queue.accommodate(context.connections);
	  for (var * connection = store.getFirstConnection(); connection != null; connection = connection.next)
	  {
		if (connection.temp || connection.hasFlag(Connection.Flags.HasRectangularSide))
		{
			continue;
		}

		// Create an arbitrary vector in plane of intersection as seed.
		var d = connection.d;
		Vec3f b = new Vec3f();
		if (Math.Abs(d.x) > Math.Abs(d.y) && Math.Abs(d.x) > Math.Abs(d.z))
		{
		  b = new Vec3f(0.0f, 1.0f, 0.0f);
		}
		else
		{
		  b = new Vec3f(1.0f, 0.0f, 0.0f);
		}

		var upWorld = normalize(cross(d, b));
		Debug.Assert(!Double.IsInfinity(lengthSquared(upWorld)) && !Double.IsNaN(lengthSquared(upWorld)));

		context.front = 0;
		context.back = 0;
		enqueue(context, null, connection, upWorld);
		do
		{
		  processItem(context);
		} while (context.front < context.back);

		context.connectedComponents++;
	  }
	  var time1 = std::chrono.high_resolution_clock.now();
	  var e0 = std::chrono.duration_cast<std::chrono.milliseconds>((time1 - time0)).count();

	  logger(0, "%d connected components in %d circular connections (%lldms).", context.connectedComponents, context.circularConnections, e0);
	}

	public static object xmalloc(size_t size)
	{
//C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
	  var rv = malloc(size);
	  if (rv != null)
	  {
		  return rv;
	  }

	  Console.Error.Write("Failed to allocate memory.");
	  Environment.Exit(-1);
	}

	public static object xcalloc(size_t count, size_t size)
	{
//C++ TO C# CONVERTER TODO TASK: The memory management function 'calloc' has no equivalent in C#:
	  var rv = calloc(count, size);
	  if (rv != null)
	  {
		  return rv;
	  }

	  Console.Error.Write("Failed to allocate memory.");
	  Environment.Exit(-1);
	}

	public static object xrealloc(object ptr, size_t size)
	{
//C++ TO C# CONVERTER TODO TASK: The memory management function 'realloc' has no equivalent in C#:
	  var rv = realloc(ptr, size);
	  if (rv != null)
	  {
		  return rv;
	  }

	  Console.Error.Write("Failed to allocate memory.");
	  Environment.Exit(-1);
	}

	public static uint64_t fnv_1a(string bytes, size_t l)
	{
	  uint64_t hash = 0xcbf29ce484222325;
	  for (size_t i = 0; i < l; i++)
	  {
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: hash = hash ^ bytes[i];
		hash.CopyFrom(hash ^ bytes[i]);
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: hash = hash * 0x100000001B3;
		hash.CopyFrom(hash * 0x100000001B3);
	  }
	  return new uint64_t(hash);
	}

	//uint64_t fnv_1a(string bytes, size_t l);Tangible Method Implementation Not Found-fnv_1a
	//uint64_t fnv_1a(string bytes, size_t l);Tangible Method Implementation Not Found-fnv_1a


	//void connect(Store store, Logger logger);Tangible Method Implementation Not Found-connect
	//void align(Store store, Logger logger);Tangible Method Implementation Not Found-align
	//bool exportJson(Store store, Logger logger, string path);Tangible Method Implementation Not Found-exportJson
	//bool discardGroups(Store store, Logger logger, object ptr, size_t size);Tangible Method Implementation Not Found-discardGroups
	//bool exportGLTF(Store store, Logger logger, string path, bool rotateZToY, bool centerModel, bool includeAttributes);Tangible Method Implementation Not Found-exportGLTF
	  public static bool isPow2<T>(T x)
	  {
		return x != 0 && (x & (x - 1)) == 0;
	  }


	  public static uint64_t hash_uint64(uint64_t x)
	  {
		x *= 0xff51afd7ed558ccd;
		x ^= x >> 32;
		return new uint64_t(x);
	  }
	  public static uint32_t[] colors = {0x0000AA, 0x00AA00, 0x00AAAA, 0xAA0000, 0xAA00AA, 0xAA5500};


	  public static void connect(Context context, uint off)
	  {
		var a = context.anchors.data();
		var a_n = context.anchors_n;
		var e = context.epsilon;
		var ee = e * e;
		Debug.Assert(off <= a_n);

//C++ TO C# CONVERTER TODO TASK: The following line could not be converted:
	std::sort(a + off, a + a_n, [](auto & a, auto & b)
	{
			return a.p.x < b.p.x;
	}
		);

		for (uint j = off; j < a_n; j++)
		{
		  if (a[j].matched)
		  {
			  continue;
		  }

		  for (uint i = j + 1; i < a_n && a[i].p.x <= a[j].p.x + e; i++)
		  {

			bool canMatch = a[i].matched == false;
			bool close = distanceSquared(a[j].p, a[i].p) <= ee;
			bool aligned = dot(a[j].d, a[i].d) < -0.98f;

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

			  //context->store->addDebugLine((a[j].p + 0.03f*a[j].d).data,
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
			std::swap(a[j], a[--a_n]);
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

		Anchor a = new Anchor();
		a.geo = geo;
		a.p = mul(new Mat3x4f(geo.M_3x4), p);
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: a.d = normalize(mul(Mat3f(geo->M_3x4.data), d));
		a.d.CopyFrom(normalize(mul(new Mat3f(geo.M_3x4.data), d)));
		a.o = o;
		a.flags = flags;

		//context->store->addDebugLine(a.p.data, (a.p + 0.02*a.d).data, 0x008800);

		Debug.Assert(context.anchors_n < context.anchors_max);
		context.anchors[context.anchors_n++] = a;
		context.anchors_total++;
	  }


	  public static void recurse(Context context, Group group)
	  {
		var offset = context.anchors_n;
		for (var * child = group.groups.first; child != null; child = child.next)
		{
		  recurse(context, child);
		}
		for (var * geo = group.group.geometries.first; geo != null; geo = geo.next)
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

			Vec3f[] n =
			{
				new Vec3f(0.0f, -h, (-t.y + o.y) - (-b.y - o.y)),
				new Vec3f(h, 0.0f, -((t.x + o.x) - (b.x - o.x))),
				new Vec3f(0.0f, h, -((t.y + o.y) - (b.y - o.y))),
				new Vec3f(-h, 0.0f, (-t.x + o.x) - (-b.x - o.x)),
				new Vec3f(0.0f, 0.0f, -1.0f),
				new Vec3f(0.0f, 0.0f, 1.0f)
			};
			Vec3f[] p =
			{
				new Vec3f(0.0f, -m.y, 0.0f),
				new Vec3f(m.x, 0.0f, 0.0f),
				new Vec3f(0.0f, m.y, 0.0f),
				new Vec3f(-m.x, 0.0f, 0.0f),
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
			Vec3f[] n =
			{
				new Vec3f(-1F, 0F, 0F),
				new Vec3f(1F, 0F, 0F),
				new Vec3f(0F, -1F, 0F),
				new Vec3f(0F, 1F, 0F),
				new Vec3f(0F, 0F, -1F),
				new Vec3f(0F, 0F, 1F)
			};
			var xp = 0.5f * box.lengths[0];
			var xm = -xp;
			var yp = 0.5f * box.lengths[1];
			var ym = -yp;
			var zp = 0.5f * box.lengths[2];
			var zm = -zp;
			Vec3f[] p =
			{
				new Vec3f(xm, 0.0f, 0.0f),
				new Vec3f(xp, 0.0f, 0.0f),
				new Vec3f(0.0f, ym, 0.0f),
				new Vec3f(0.0f, yp, 0.0f),
				new Vec3f(0.0f, 0.0f, zm),
				new Vec3f(0.0f, 0.0f, zp)
			};
			for (uint i = 0; i < 6; i++)
			{
				addAnchor(context, geo, p[i], n[i], i, Connection.Flags.HasRectangularSide);
			}
			break;
		  }

		  case Geometry.Kind.RectangularTorus:
		  {
			var rt = geo.rectangularTorus;
			var c = Math.Cos(rt.angle);
			var s = Math.Sin(rt.angle);
			var m = 0.5f * (rt.inner_radius + rt.outer_radius);
			Vec3f[] n =
			{
				new Vec3f(0F, -1F, 0.0f),
				new Vec3f(-s, c, 0.0f)
			};
			Vec3f[] p =
			{
				new Vec3f(geo.circularTorus.offset, 0F, 0.0f),
				new Vec3f(m * c, m * s, 0.0f)
			};
			for (uint i = 0; i < 2; i++)
			{
				addAnchor(context, geo, p[i], n[i], i, Connection.Flags.HasRectangularSide);
			}
			break;
		  }

		  case Geometry.Kind.CircularTorus:
		  {
			var ct = geo.circularTorus;
			var c = Math.Cos(ct.angle);
			var s = Math.Sin(ct.angle);
			Vec3f[] n =
			{
				new Vec3f(0F, -1F, 0.0f),
				new Vec3f(-s, c, 0.0f)
			};
			Vec3f[] p =
			{
				new Vec3f(ct.offset, 0F, 0.0f),
				new Vec3f(ct.offset * c, ct.offset * s, 0.0f)
			};
			for (uint i = 0; i < 2; i++)
			{
				addAnchor(context, geo, p[i], n[i], i, Connection.Flags.HasCircularSide);
			}
			break;
		  }

		  case Geometry.Kind.EllipticalDish:
		  case Geometry.Kind.SphericalDish:
		  {
			addAnchor(context, geo, new Vec3f(0F, 0F, 0F), new Vec3f(0F, 0F, -1F), 0, Connection.Flags.HasCircularSide);
			break;
		  }

		  case Geometry.Kind.Snout:
		  {
			var sn = geo.snout;
			Vec3f[] n =
			{
				new Vec3f(Math.Sin(sn.bshear[0]) * Math.Cos(sn.bshear[1]), Math.Sin(sn.bshear[1]), -Math.Cos(sn.bshear[0]) * Math.Cos(sn.bshear[1])),
				new Vec3f(-Math.Sin(sn.tshear[0]) * Math.Cos(sn.tshear[1]), -Math.Sin(sn.tshear[1]), Math.Cos(sn.tshear[0]) * Math.Cos(sn.tshear[1]))
			};
			Vec3f[] p =
			{
				new Vec3f(-0.5f * sn.offset[0], -0.5f * sn.offset[1], -0.5f * sn.height),
				new Vec3f(0.5f * sn.offset[0], 0.5f * sn.offset[1], 0.5f * sn.height)
			};
			for (uint i = 0; i < 2; i++)
			{
				addAnchor(context, geo, p[i], n[i], i, Connection.Flags.HasCircularSide);
			}
			break;
		  }

		  case Geometry.Kind.Cylinder:
		  {
			Vec3f[] d =
			{
				new Vec3f(0F, 0F, -1.0f),
				new Vec3f(0F, 0F, 1.0f)
			};
			Vec3f[] p =
			{
				new Vec3f(0F, 0F, -0.5f * geo.cylinder.height),
				new Vec3f(0F, 0F, 0.5f * geo.cylinder.height)
			};
			for (uint i = 0; i < 2; i++)
			{
				addAnchor(context, geo, p[i], d[i], i, Connection.Flags.HasCircularSide);
			}
			break;
		  }

		  case Geometry.Kind.Sphere:
		  case Geometry.Kind.FacetGroup:
		  case Geometry.Kind.Line:
			break;

		  default:
			Debug.Assert(false && "Unhandled primitive type");
			break;
		  }
		}
		connect(context, offset);
	  }





	public static void connect(Store store, Logger logger)
	{

	  Context context = new Context();
	  context.store = store;
	  context.logger = logger;

	  context.anchors_max = (uint)(6 * store.geometryCountAllocated());
	  context.anchors.accommodate(context.anchors_max);


	  var time0 = std::chrono.high_resolution_clock.now();
	  context.anchors_n = 0;
	  for (var * root = store.getFirstRoot(); root != null; root = root.next)
	  {
		for (var * model = root.groups.first; model != null; model = model.next)
		{
		  for (var * group = model.groups.first; group != null; group = group.next)
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

		//if (a.geo->kind == Geometry::Kind::Pyramid) {
		//  context.store->addDebugLine(a.p.data, b.data, 0x003300);
		//}
		//else {
		//  context.store->addDebugLine(a.p.data, b.data, 0xff0000);
		//}
	  }
	  var time1 = std::chrono.high_resolution_clock.now();
	  var e0 = std::chrono.duration_cast<std::chrono.milliseconds>((time1 - time0)).count();

	  logger(0, "Matched %u of %u anchors (%lldms).", context.anchors_matched, context.anchors_total, e0);

	}


	  public static void readTagList(Context context, object ptr, size_t size)
	  {
//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
		var * a = (string)ptr;
		var b = a + size;

		uint32_t N = 0;
		while (true)
		{
		  while (a < b && (*a == '\n' || *a == '\r'))
		  {
			  a++;
		  }
		  var c = a;
		  while (a < b && (*a != '\n' && *a != '\r'))
		  {
			  a++;
		  }

		  if (c < a)
		  {
			var d = a - 1;
			while (c < d && (d[-1] != '\t'))
			{
				--d;
			}

			var str = context.store.strings.intern(d, a);
			context.discardTags.insert(uint64_t(str), uint64_t(1 + N++));
		  }
		  else
		  {
			break;
		  }
		}
		context.logger(0, "DiscardGroups: Read %d tags.", N);
	  }


	  public static void pruneChildren(Context context, Group group)
	  {

		ListHeader<Group> groupsNew = new ListHeader<Group>();
		groupsNew.clear();
		for (var * child = group.groups.first; child;)
		{
		  var next = child.next;
		  if (context.discardTags.get(uint64_t(child.group.name)) != null)
		  {
			//context->logger(0, "Discarded %s", child->group.name);
			context.discarded++;
		  }
		  else
		  {
			pruneChildren(context, child);
			groupsNew.insert(child);
		  }
		  child = next;
		}
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: group->groups = groupsNew;
		group.groups.CopyFrom(groupsNew);
	  }



	public static bool discardGroups(Store store, Logger logger, object ptr, size_t size)
	{
	  Context context = new Context();
	  context.store = store;
	  context.logger = logger;
	  readTagList(context, ptr, new size_t(size));

	  for (var * root = store.getFirstRoot(); root != null; root = root.next)
	  {
		for (var * model = root.groups.first; model != null; model = model.next)
		{
		  pruneChildren(context, model);
		}
	  }
	  context.logger(0, "DiscardGroups: Discarded %d groups.", context.discarded);

	  return true;
	}

	  public static bool open_w(FILE[] f, string path)
	  {
	#if _WIN32
		var err = fopen_s(f, path, "w");
		if (err == 0)
		{
			return true;
		}

		string buf = new string(new char[1024]);
		if (strerror_s(buf, sizeof(char), err) != 0)
		{
		  buf = StringFunctions.ChangeCharacter(buf, 0, '\0');
		}
		Console.Error.Write("Failed to open {0} for writing: {1}", path, buf);
	#else
		f[0] = fopen(path, "w");
		if (f[0] != null)
		{
			return true;
		}

		Console.Error.Write("Failed to open {0} for writing.", path);
	#endif
		return false;
	  }

	  public static void wireBoundingBox(FILE @out, ref uint off_v, BBox3f bbox)
	  {
		for (uint i = 0; i < 8; i++)
		{
		  float px = (i & 1) != 0 ? bbox.min[0] : bbox.min[3];
		  float py = (i & 2) != 0 ? bbox.min[1] : bbox.min[4];
		  float pz = (i & 4) != 0 ? bbox.min[2] : bbox.min[5];
		  fprintf(@out, "v %f %f %f\n", px, py, pz);
		}
		fprintf(@out, "l %d %d %d %d %d\n", off_v + 0, off_v + 1, off_v + 3, off_v + 2, off_v + 0);
		fprintf(@out, "l %d %d %d %d %d\n", off_v + 4, off_v + 5, off_v + 7, off_v + 6, off_v + 4);
		fprintf(@out, "l %d %d\n", off_v + 0, off_v + 4);
		fprintf(@out, "l %d %d\n", off_v + 1, off_v + 5);
		fprintf(@out, "l %d %d\n", off_v + 2, off_v + 6);
		fprintf(@out, "l %d %d\n", off_v + 3, off_v + 7);
		off_v += 8;
	  }

	  public static void getMidpoint(ref Vec3f p, Geometry geo)
	  {
		switch (geo.kind)
		{
		case Geometry.Kind.CircularTorus:
		{
		  var ct = geo.circularTorus;
		  var c = Math.Cos(0.5f * ct.angle);
		  var s = Math.Sin(0.5f * ct.angle);
		  p.x = ct.offset * c;
		  p.y = ct.offset * s;
		  p.z = 0.0f;
		  break;
		}
		default:
		  p = new Vec3f(0.0f);
		  break;
		}
		p = mul(geo.M_3x4, p);
	  }


	public static Vec2f operator * (float a, Vec2f b)
	{
		return new Vec2f(a * b.x, a * b.y);
	}

	public static Vec2f operator - (Vec2f a, Vec2f b)
	{
		return new Vec2f(a.x - b.x, a.y - b.y);
	}

	public static Vec2f operator + (Vec2f a, Vec2f b)
	{
		return new Vec2f(a.x + b.x, a.y + b.y);
	}

	public static Vec3f cross(Vec3f a, Vec3f b)
	{
	  return new Vec3f(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
	}

	public static float dot(Vec3f a, Vec3f b)
	{
	  return a.x * b.x + a.y * b.y + a.z * b.z;
	}

	public static Vec3f operator + (Vec3f a, Vec3f b)
	{
		return new Vec3f(a.x + b.x, a.y + b.y, a.z + b.z);
	}

	public static Vec3f operator - (Vec3f a, Vec3f b)
	{
		return new Vec3f(a.x - b.x, a.y - b.y, a.z - b.z);
	}

	public static Vec3f operator * (float a, Vec3f b)
	{
		return new Vec3f(a * b.x, a * b.y, a * b.z);
	}

	public static float lengthSquared(Vec3f a)
	{
		return dot(a, a);
	}

	public static float length(Vec3f a)
	{
		return Math.Sqrt(dot(a, a));
	}

	public static float distanceSquared(Vec3f a, Vec3f b)
	{
		return lengthSquared(a - b);
	}

	public static float distance(Vec3f a, Vec3f b)
	{
		return length(a - b);
	}

	public static Vec3f normalize(Vec3f a)
	{
		return (1.0f / length(a)) * a;
	}

//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'dst', so pointers on this parameter are left unchanged:
	public static void write(float * dst, Vec3f a)
	{
		*dst++ = a.data[0];
		*dst++ = a.data[1];
		*dst++ = a.data[2];
	}

	public static Vec3f max(Vec3f a, Vec3f b)
	{
	  return new Vec3f(a.x > b.x ? a.x : b.x, a.y > b.y ? a.y : b.y, a.z > b.z ? a.z : b.z);
	}

	public static Vec3f min(Vec3f a, Vec3f b)
	{
	  return new Vec3f(a.x < b.x ? a.x : b.x, a.y < b.y ? a.y : b.y, a.z < b.z ? a.z : b.z);
	}

	public static Mat3f inverse(Mat3f M)
	{
	  var c0 = M.cols[0];
	  var c1 = M.cols[1];
	  var c2 = M.cols[2];

	  var r0 = cross(c1, c2);
	  var r1 = cross(c2, c0);
	  var r2 = cross(c0, c1);

	  var invDet = 1.0f / dot(r2, c2);

	  return new Mat3f(invDet * r0.x, invDet * r0.y, invDet * r0.z, invDet * r1.x, invDet * r1.y, invDet * r1.z, invDet * r2.x, invDet * r2.y, invDet * r2.z);
	}

	public static Mat3f mul(Mat3f A, Mat3f B)
	{
	  var A00 = A.cols[0].x;
	  var A10 = A.cols[0].y;
	  var A20 = A.cols[0].z;
	  var A01 = A.cols[1].x;
	  var A11 = A.cols[1].y;
	  var A21 = A.cols[1].z;
	  var A02 = A.cols[2].x;
	  var A12 = A.cols[2].y;
	  var A22 = A.cols[2].z;

	  var B00 = B.cols[0].x;
	  var B10 = B.cols[0].y;
	  var B20 = B.cols[0].z;
	  var B01 = B.cols[1].x;
	  var B11 = B.cols[1].y;
	  var B21 = B.cols[1].z;
	  var B02 = B.cols[2].x;
	  var B12 = B.cols[2].y;
	  var B22 = B.cols[2].z;

	  return new Mat3f(A00 * B00 + A01 * B10 + A02 * B20, A00 * B01 + A01 * B11 + A02 * B21, A00 * B02 + A01 * B12 + A02 * B22, A10 * B00 + A11 * B10 + A12 * B20, A10 * B01 + A11 * B11 + A12 * B21, A10 * B02 + A11 * B12 + A12 * B22, A20 * B00 + A21 * B10 + A22 * B20, A20 * B01 + A21 * B11 + A22 * B21, A20 * B02 + A21 * B12 + A22 * B22);
	}

	public static float getScale(Mat3f M)
	{
	  float sx = length(M.cols[0]);
	  float sy = length(M.cols[1]);
	  float sz = length(M.cols[2]);
	  var t = sx > sy ? sx : sy;
	  return sz > t ? sz : t;
	}

	public static float getScale(Mat3x4f M)
	{
		return getScale(new Mat3f(M.data));
	}

	public static Vec3f mul(Mat3f A, Vec3f x)
	{
	  Vec3f r = new Vec3f();
	  for (uint k = 0; k < 3; k++)
	  {
		r.data[k] = A.data[k] * x.data[0] + A.data[3 + k] * x.data[1] + A.data[6 + k] * x.data[2];
	  }
	  return new Vec3f(r);
	}


	public static Vec3f mul(Mat3x4f A, Vec3f x)
	{
	  Vec3f r = new Vec3f();
	  for (uint k = 0; k < 3; k++)
	  {
		r.data[k] = A.data[k] * x.data[0] + A.data[3 + k] * x.data[1] + A.data[6 + k] * x.data[2] + A.data[9 + k];
	  }
	  return new Vec3f(r);
	}

	public static BBox3f createEmptyBBox3f()
	{
	  return new BBox3f(new Vec3f(float.MaxValue, float.MaxValue, float.MaxValue), new Vec3f(-float.MaxValue, -float.MaxValue, -float.MaxValue));
	}

	public static BBox3f.BBox3f(BBox3f bbox, float margin)
	{
		this.min = bbox.min - new Vec3f(margin);
		this.max = bbox.max + new Vec3f(margin);
	}

	public static void engulf(BBox3f target, Vec3f p)
	{
	  target.min = min(target.min, p);
	  target.max = max(target.max, p);
	}

	public static void engulf(BBox3f target, BBox3f other)
	{
	  target.min = min(target.min, other.min);
	  target.max = max(target.max, other.max);
	}

	public static BBox3f transform(Mat3x4f M, BBox3f bbox)
	{
	  Vec3f[] p = {mul(M, new Vec3f(bbox.min.x, bbox.min.y, bbox.min.z)), mul(M, new Vec3f(bbox.min.x, bbox.min.y, bbox.max.z)), mul(M, new Vec3f(bbox.min.x, bbox.max.y, bbox.min.z)), mul(M, new Vec3f(bbox.min.x, bbox.max.y, bbox.max.z)), mul(M, new Vec3f(bbox.max.x, bbox.min.y, bbox.min.z)), mul(M, new Vec3f(bbox.max.x, bbox.min.y, bbox.max.z)), mul(M, new Vec3f(bbox.max.x, bbox.max.y, bbox.min.z)), mul(M, new Vec3f(bbox.max.x, bbox.max.y, bbox.max.z))};
	  return new BBox3f(min(min(min(p[0], p[1]), min(p[2], p[3])), min(min(p[4], p[5]), min(p[6], p[7]))), max(max(max(p[0], p[1]), max(p[2], p[3])), max(max(p[4], p[5]), max(p[6], p[7]))));
	}

	public static float diagonal(BBox3f b)
	{
		return distance(b.min, b.max);
	}

	public static bool isEmpty(BBox3f b)
	{
		return b.max.x < b.min.x;
	}

	public static bool isNotEmpty(BBox3f b)
	{
		return b.min.x <= b.max.x;
	}

	public static float maxSideLength(BBox3f b)
	{
	  var l = b.max - b.min;
	  var t = l.x > l.y ? l.x : l.y;
	  return l.z > t ? l.z : t;
	}

	public static bool isStrictlyInside(BBox3f a, BBox3f b)
	{
	  var lx = a.min.x <= b.min.x;
	  var ly = a.min.y <= b.min.y;
	  var lz = a.min.z <= b.min.z;
	  var ux = b.max.x <= a.max.x;
	  var uy = b.max.y <= a.max.y;
	  var uz = b.max.z <= a.max.z;
	  return lx && ly && lz && ux && uy && uz;
	}

	public static bool isNotOverlapping(BBox3f a, BBox3f b)
	{
	  var lx = b.max.x < a.min.x;
	  var ly = b.max.y < a.min.y;
	  var lz = b.max.z < a.min.z;
	  var ux = a.max.x < b.min.x;
	  var uy = a.max.y < b.min.y;
	  var uz = a.max.z < b.min.z;
	  return lx || ly || lz || ux || uy || uz;
	}

	public static bool isOverlapping(BBox3f a, BBox3f b)
	{
		return !isNotOverlapping(a, b);
	}





	public static void logger(uint level, string msg, params object[] LegacyParamArray)
	{
	  switch (level)
	  {
	  case 0:
		  Console.Error.Write("[I] ");
		  break;
	  case 1:
		  Console.Error.Write("[W] ");
		  break;
	  case 2:
		  Console.Error.Write("[E] ");
		  break;
	  }

	//  va_list argptr;
  int ParamCount = -1;
	//  va_start(argptr, msg);
	  vfprintf(stderr, msg, argptr);
	//  va_end(argptr);
	  Console.Error.Write("\n");
	}

//C++ TO C# CONVERTER WARNING: The original C++ template specifier was replaced with a C# generic specifier, which may not produce the same behavior:
//ORIGINAL LINE: template<typename F>
	public static bool processFile<F>(string path, F f)
	{
	  bool rv = false;
	#if _WIN32

	  IntPtr h = CreateFileA(path, GENERIC_READ, FILE_SHARE_READ, null, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, null);
	  if (h == INVALID_HANDLE_VALUE)
	  {
		logger(2, "CreateFileA returned INVALID_HANDLE_VALUE");
		rv = false;
	  }
	  else
	  {
		uint hiSize;
		uint loSize = GetFileSize(h, hiSize);
		size_t fileSize = (size_t(hiSize) << 32u) + loSize;

		IntPtr m = CreateFileMappingA(h, 0, PAGE_READONLY, 0, 0, null);
		if (m == INVALID_HANDLE_VALUE)
		{
		  logger(2, "CreateFileMappingA returned INVALID_HANDLE_VALUE");
		  rv = false;
		}
		else
		{
		  object ptr = MapViewOfFile(m, FILE_MAP_READ, 0, 0, 0);
		  if (ptr == null)
		  {
			logger(2, "MapViewOfFile returned INVALID_HANDLE_VALUE");
			rv = false;
		  }
		  else
		  {
			rv = f(ptr, fileSize);
			UnmapViewOfFile(ptr);
		  }
		  CloseHandle(m);
		}
		CloseHandle(h);
	  }

	#else

	  int fd = open(path, O_RDONLY);
	  if (fd == -1)
	  {
		logger(2, "%s: open failed: %s", path, strerror(errno));
	  }
	  else
	  {
		struct stat stat
		{
		};
		if (fstat(fd, stat) != 0)
		{
		  logger(2, "%s: fstat failed: %s", path, strerror(errno));
		}
		else
		{
		  object ptr = mmap(null, stat.st_size, PROT_READ, MAP_PRIVATE | MAP_POPULATE, fd, 0);
		  if (ptr == MAP_FAILED)
		  {
			logger(2, "%s: mmap failed: %s", path, strerror(errno));
		  }
		  else
		  {
			if (madvise(ptr, stat.st_size, MADV_SEQUENTIAL) != 0)
			{
			  logger(1, "%s: madvise(MADV_SEQUENTIAL) failed: %s", path, strerror(errno));
			}
			rv = f(ptr, stat.st_size);
			if (munmap(ptr, stat.st_size) != 0)
			{
			  logger(2, "%s: munmap failed: %s", path, strerror(errno));
			  rv = false;
			}
		  }
		}
	  }

	#endif
	  return rv;
	}

	  public static void printHelp(string argv0)
	  {
		Console.Error.Write(@"
Usage: {0} [options] files

Files with .rvm-suffix will be interpreted as a geometry files, and files with .txt or .att suffix
will be interpreted as attribute files. A rvm file typically has a matching attribute file.

Options:
  --keep-groups=filename.txt          Provide a list of group names to keep. Groups not itself or
                                      with a child in this list will be merged with the first
                                      parent that should be kept.
  --discard-groups=filename.txt       Provide a list of group names to discard, one name per line.
                                      Groups with its name in this list will be discarded along
                                      with its children. Default is no groups are discarded.
  --output-json=<filename.json>       Write hierarchy with attributes to a json file.
  --output-txt=<filename.txt>         Dump all group names to a text file.
  --output-obj=<filenamestem>         Write geometry to an obj file. The suffices .obj and .mtl are
                                      added to the filenamestem.
  --output-gltf=<filename.glb>        Write geometry into a GLTF file.
  --output-gltf-attributes=<bool>     Include rvm attributes in the extra member of nodes. Default
                                      value is true.
  --output-gltf-center=<bool>         Position the model at the bounding box center and store the
                                      position of this origin in the asset's extra field. This is
                                      useful if the model is so far away from the origin that float
                                      precision becomes an issue. Defaults value is false.
  --output-gltf-rotate-z-to-y=<bool>  Add an extra node below the root that adds a clockwise
                                      rotation of 90 degrees about the X axis such that the +Z axis
                                      will map to the +Y axis, which is the up-direction of GLTF-
                                      files. Default value is true.
  --group-bounding-boxes              Include wireframe of boundingboxes of groups in output.
  --color-attribute=key               Specify which attributes that contain color, empty key
                                      implies that material id of group is used.
  --tolerance=value                   Tessellation tolerance, given in world frame. Default value
                                      is 0.1.
  --cull-scale=value                  Cull objects smaller than cull-scale times tolerance. Set to
                                      a negative value to disable culling. Disabled by default.

Post bug reports or questions at https://github.com/cdyk/rvmparser
", argv0);
	  }


	  public static bool parseBool(Logger logger, string arg, string value)
	  {
		string lower;
		foreach (char c in value)
		{
		  lower.push_back(char.ToLower(c));
		}
		if (lower == "true" || lower == "1" || lower == "yes")
		{
		  return true;
		}
		else if (lower == "false" || lower == "0" || lower == "no")
		{
		  return false;
		}
		else
		{
		  logger(2, "Failed to parse bool option '%s'", arg);
		  Environment.Exit(1);
		}
	  }



	static int Main(int argc, string[] args)
	{
	  int rv = 0;
	  bool should_tessellate = false;
	  bool should_colorize = false;

	  float tolerance = 0.1f;
	  float cullScale = -10000.1f;

	  uint chunkTinyVertexThreshold = 0;

	  bool groupBoundingBoxes = false;
	  string discard_groups;
	  string keep_groups;
	  string output_json;
	  string output_txt;
	  string output_gltf;
	  bool output_gltf_rotate_z_to_y = true;
	  bool output_gltf_center = false;
	  bool output_gltf_attributes = true;

	  string output_obj_stem;
	  string color_attribute;


	  List<string> attributeSuffices = new List<string>() {".txt", ".att"};

	  Store store = new Store();


	  string stem;
	  for (int i = 1; i < argc; i++)
	  {
		var arg = new string(args[i]);

		if (arg.substr(0, 2) == "--")
		{

		  if (arg == "--help")
		  {
			printHelp(args[0]);
		  }
		  else if (arg == "--group-bounding-boxes")
		  {
			groupBoundingBoxes = true;
			continue;
		  }

		  var e = arg.find('=');
		  if (e != -1)
		  {
			var key = arg.substr(0, e);
			var val = arg.substr(e + 1);
			if (key == "--keep-groups")
			{
			  keep_groups = val;
			  continue;
			}
			else if (key == "--discard-groups")
			{
			  discard_groups = val;
			  continue;
			}
			else if (key == "--output-json")
			{
			  output_json = val;
			  continue;
			}
			else if (key == "--output-txt")
			{
			  output_txt = val;
			  continue;
			}
			else if (key == "--output-obj")
			{
			  output_obj_stem = val;
			  should_tessellate = true;
			  should_colorize = true;
			  continue;
			}
			else if (key == "--output-gltf")
			{
			  output_gltf = val;
			  should_tessellate = true;
			  should_colorize = true;
			  continue;
			}
			else if (key == "--output-gltf-rotate-z-to-y")
			{
			  output_gltf_rotate_z_to_y = parseBool(logger, arg, val);
			  continue;
			}
			else if (key == "--output-gltf-center")
			{
			  output_gltf_center = parseBool(logger, arg, val);
			  continue;
			}
			else if (key == "--output-gltf-attributes")
			{
			  output_gltf_attributes = parseBool(logger, arg, val);
			  continue;
			}
			else if (key == "--color-attribute")
			{
			  color_attribute = val;
			  continue;
			}
			else if (key == "--tolerance")
			{
			  tolerance = Math.Max(1e-6f, Convert.ToSingle(val));
			  continue;
			}
			else if (key == "--cull-scale")
			{
			  cullScale = Convert.ToSingle(val); // set to negative to disable culling.
			  continue;
			}
			else if (key == "--chunk-tiny")
			{
			  chunkTinyVertexThreshold = Convert.ToUInt32(val);
			  should_tessellate = true;
			  continue;
			}
		  }

		  Console.Error.Write("Unrecognized argument '{0}'", arg.c_str());
		  printHelp(args[0]);
		  return -1;
		}

		var arg_lc = arg;
		foreach (var c in arg_lc)
		{
			c = char.ToLower(c);
		}

		// parse rvm file
		var l = arg_lc.rfind(".rvm");
		if (l != -1)
		{
	  if (processFile(arg, (object ptr, size_t size) =>
	  {
		  return parseRVM(store, ptr, new size_t(size));
	  }))
	  {
			Console.Error.Write("Successfully parsed {0}\n", arg.c_str());
	  }
		  else
		  {
			Console.Error.Write("Failed to parse {0}: {1}\n", arg.c_str(), store.errorString());
			rv = -1;
			break;
		  }
		  continue;
		}

		// parse attributes file
		l = arg_lc.rfind(".txt");
		if (l != -1)
		{
	  if (processFile(arg, (object ptr, size_t size) =>
	  {
		  return parseAtt(store, logger, ptr, new size_t(size));
	  }))
	  {
			Console.Error.Write("Successfully parsed {0}\n", arg.c_str());
	  }
		  else
		  {
			Console.Error.Write("Failed to parse {0}\n", arg.c_str());
			rv = -1;
			break;
		  }
		  continue;
		}
	  }

	  if ((rv == 0) && should_colorize)
	  {
		Colorizer colorizer = new Colorizer(logger, string.IsNullOrEmpty(color_attribute) ? null : color_attribute);
		store.apply(colorizer);
	  }

	  if (rv == 0 && !string.IsNullOrEmpty(discard_groups))
	  {
	if (processFile(discard_groups, (object ptr, size_t size) =>
	{
		return discardGroups(store, logger, ptr, new size_t(size));
	}))
	{
		  logger(0, "Processed %s", discard_groups);
	}
		else
		{
		  logger(2, "Failed to parse %s", discard_groups);
		  rv = -1;
		}
	  }


	  if (rv == 0)
	  {
		connect(store, logger);
		align(store, logger);
	  }

	  if (rv == 0 && (should_tessellate || !string.IsNullOrEmpty(output_json)))
	  {
		AddGroupBBox addGroupBBox = new AddGroupBBox();
		store.apply(addGroupBBox);
	  }

	  if (rv == 0 && should_tessellate)
	  {
		float cullLeafThreshold = -1.0f;
		float cullGeometryThreshold = -1.0f;
		uint maxSamples = 100;

		var time0 = std::chrono.high_resolution_clock.now();
		Tessellator tessellator = new Tessellator(logger, tolerance, cullLeafThreshold, cullGeometryThreshold, maxSamples);
		store.apply(tessellator);
		var time1 = std::chrono.high_resolution_clock.now();
		var e0 = std::chrono.duration_cast<std::chrono.milliseconds>((time1 - time0)).count();
		logger(0, "Tessellated %u items of %u into %llu vertices and %llu triangles (tol=%f, %lluk, %lldms)", tessellator.tessellated, tessellator.processed, tessellator.vertices, tessellator.triangles, tolerance, (4 * 3 * tessellator.vertices + 4 * 3 * tessellator.triangles) / 1024, e0);
	  }

	  bool do_flatten = false;
	  Flatten flatten = new Flatten(store);
	  if (rv == 0 && !string.IsNullOrEmpty(keep_groups))
	  {
	if (processFile(keep_groups, (object ptr, size_t size) =>
	{
		f.setKeep(ptr, size);
		return true;
	}))
	{
		  Console.Error.Write("Processed {0}\n", keep_groups);
		  do_flatten = true;

	}
		else
		{
		  Console.Error.Write("Failed to parse {0}\n", keep_groups);
		  rv = -1;
		}
	  }

	  if (rv == 0 && chunkTinyVertexThreshold != 0)
	  {
		ChunkTiny chunkTiny = new ChunkTiny(flatten, chunkTinyVertexThreshold);
		store.apply(chunkTiny);
		do_flatten = true;
	  }

	  if (do_flatten)
	  {
		var storeNew = flatten.run();
		store = null;
		store = storeNew;
	  }


	  if (rv == 0 && !string.IsNullOrEmpty(output_json))
	  {
		var time0 = std::chrono.high_resolution_clock.now();
		if (exportJson(store, logger, output_json))
		{
		  var time1 = std::chrono.high_resolution_clock.now();
		  var e = std::chrono.duration_cast<std::chrono.milliseconds>((time1 - time0)).count();
		  logger(0, "Exported json into %s (%lldms)", output_json, e);
		}
		else
		{
		  logger(2, "Failed to export obj file.\n");
		  rv = -1;
		}
	  }

	  if (rv == 0 && !string.IsNullOrEmpty(output_txt))
	  {

	#if _WIN32
		FILE @out = null;
		if (fopen_s(@out, output_txt, "w") == 0)
		{
	#else
		FILE @out = fopen(output_txt, "w");
		if (@out != null)
		{
	#endif

		  DumpNames dumpNames = new DumpNames();
		  dumpNames.setOutput(@out);
		  store.apply(dumpNames);
		  fclose(@out);
		}
		else
		{
		  logger(2, "Failed to open %s for writing", output_txt);
		  rv = -1;
		}
	  }

	  if (rv == 0 && !string.IsNullOrEmpty(output_obj_stem))
	  {
		Debug.Assert(should_tessellate);

		var time0 = std::chrono.high_resolution_clock.now();
		ExportObj exportObj = new ExportObj();
		exportObj.groupBoundingBoxes = groupBoundingBoxes;
		if (exportObj.open((output_obj_stem + ".obj").c_str(), (output_obj_stem + ".mtl").c_str()))
		{
		  store.apply(exportObj);

		  var time1 = std::chrono.high_resolution_clock.now();
		  var e = std::chrono.duration_cast<std::chrono.milliseconds>((time1 - time0)).count();
		  logger(0, "Exported obj into %s(.obj|.mtl) (%lldms)", output_obj_stem, e);
		}
		else
		{
		  logger(2, "Failed to export obj file.\n");
		  rv = -1;
		}
	  }

	  if (rv == 0 && !string.IsNullOrEmpty(output_gltf))
	  {
		Debug.Assert(should_tessellate);
		var time0 = std::chrono.high_resolution_clock.now();
		if (exportGLTF(store, logger, output_gltf, output_gltf_rotate_z_to_y, output_gltf_center, output_gltf_attributes))
		{
		  long e = std::chrono.duration_cast<std::chrono.milliseconds>((std::chrono.high_resolution_clock.now() - time0)).count();
		  logger(0, "Exported gltf into %s (%lldms)", output_gltf, e);
		}
		else
		{
		  logger(2, "Failed to export gltf into %s", output_gltf);
		  rv = -1;
		}
	  }

	  AddStats addStats = new AddStats();
	  store.apply(addStats);
	  var stats = store.stats;
	  if (stats != null)
	  {
		logger(0, "Stats:");
		logger(0, "    Groups                 %d", stats.group_n);
		logger(0, "    Geometries             %d (grp avg=%.1f)", stats.geometry_n, stats.geometry_n / (float)stats.group_n);
		logger(0, "        Pyramids           %d", stats.pyramid_n);
		logger(0, "        Boxes              %d", stats.box_n);
		logger(0, "        Rectangular tori   %d", stats.rectangular_torus_n);
		logger(0, "        Circular tori      %d", stats.circular_torus_n);
		logger(0, "        Elliptical dish    %d", stats.elliptical_dish_n);
		logger(0, "        Spherical dish     %d", stats.spherical_dish_n);
		logger(0, "        Snouts             %d", stats.snout_n);
		logger(0, "        Cylinders          %d", stats.cylinder_n);
		logger(0, "        Spheres            %d", stats.sphere_n);
		logger(0, "        Facet groups       %d", stats.facetgroup_n);
		logger(0, "            triangles      %d", stats.facetgroup_triangles_n);
		logger(0, "            quads          %d", stats.facetgroup_quads_n);
		logger(0, "            polygons       %d (fgrp avg=%.1f)", stats.facetgroup_polygon_n, (stats.facetgroup_polygon_n / (float)stats.facetgroup_n));
		logger(0, "                contours   %d (poly avg=%.1f)", stats.facetgroup_polygon_n_contours_n, (stats.facetgroup_polygon_n_contours_n / (float)stats.facetgroup_polygon_n));
		logger(0, "                vertices   %d (cont avg=%.1f)", stats.facetgroup_polygon_n_vertices_n, (stats.facetgroup_polygon_n_vertices_n / (float)stats.facetgroup_polygon_n_contours_n));
		logger(0, "        Lines              %d", stats.line_n);
	  }

	  store = null;

	  return rv;
	}



	  public static bool handleNew(Context ctx, string id_a, string id_b)
	  {
		if (ctx.stack_c <= ctx.stack_p + 1)
		{
		  ctx.stack_c = (uint)(2 * ctx.stack_c);
		  ctx.stack = (StackItem)xrealloc(ctx.stack, sizeof(StackItem) * ctx.stack_c);
		}

		var id = ctx.store.strings.intern(id_a, id_b);

		Group group = null;
		if (ctx.stack_p == 0)
		{

		  if (id != ctx.headerInfo)
		  {
			group = ctx.store.findRootGroup(id);
			if (ctx.create && group == null)
			{
			  var model = ctx.store.getDefaultModel();
			  group = ctx.store.newGroup(model, Group.Kind.Group);
			  group.group.name = id;
			  //ctx->logger(1, "@%d: Failed to find root group '%s' id=%p", ctx->line, id, id);
			}
		  }
		}
		else
		{

		  var parent = ctx.stack[ctx.stack_p - 1].group;
		  if (parent != null)
		  {
			for (var * child = parent.groups.first; child; child = child.next)
			{
			  if (child.group.name == id)
			  {
				group = child;
				break;
			  }
			}
		  }
		  if (ctx.create && group == null)
		  {
			group = ctx.store.newGroup(parent, Group.Kind.Group);
			group.group.name = id;
			//ctx->logger(1, "@%d: Failed to find child group '%s' id=%p", ctx->line, id, id);
		  }
		}

		//ctx->logger(0, "@%d: new '%s'", ctx->line, id);
		Debug.Assert(id);
		ctx.stack[ctx.stack_p++] = new StackItem() {id = id, group = group};
		return true;
	  }

	  public static bool handleEnd(Context ctx)
	  {
		if (ctx.stack_p == 0)
		{
		  ctx.logger(2, "@%d: More END-tags and than NEW-tags.", ctx.line);
		  return false;
		}
		//ctx->logger(0, "@%d: end", ctx->line);
		ctx.stack_p--;
		return true;
	  }

	  public static bool handleAttribute(Context ctx, string key_a, string key_b, string value_a, string value_b)
	  {
		Debug.Assert(ctx.stack_p);
		var grp = ctx.stack[ctx.stack_p - 1].group;
		if (grp == null)
		{
			return true; // Inside skipped group like headerinfo
		}

		var key = ctx.store.strings.intern(key_a, key_b);
		var att = ctx.store.getAttribute(grp, key);
		if (att == null)
		{
		  att = ctx.store.newAttribute(grp, key);
		}
		att.val = ctx.store.strings.intern(value_a, value_b);

		//ctx->logger(0, "@%d: att ('%s', '%s')", ctx->line, key, value);
		return true;
	  }


//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'p', so pointers on this parameter are left unchanged:
	  public static string getEndOfLine(char * p, string end)
	  {
		while (p < end && (*p != '\n' && *p != '\r'))
		{
			p = p.Substring(1);
		}
		return p;
	  }

//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'p', so pointers on this parameter are left unchanged:
	  public static string skipEndOfLine(char * p, string end)
	  {
		while (p < end && (*p == '\n' || *p == '\r'))
		{
			p = p.Substring(1);
		}
		return p;
	  }

//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'p', so pointers on this parameter are left unchanged:
	  public static string skipSpace(char * p, string end)
	  {
		while (p < end && (*p == ' ' || *p == '\t'))
		{
			p = p.Substring(1);
		}
		return p;
	  }

//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'p', so pointers on this parameter are left unchanged:
	  public static string reverseSkipSpace(string start, char * p)
	  {
		while (start < p && (p[-1] == ' ' || p[-1] == '\t'))
		{
			p--;
		}
		return p;
	  }

//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'p', so pointers on this parameter are left unchanged:
	  public static string parseIndentation(ref uint spaces, ref uint tabs, char * p, string end)
	  {
		spaces = 0;
		tabs = 0;
		for (; p < end; p++)
		{
		  if (*p == ' ')
		  {
			  spaces++;
		  }
		  else if (*p == '\t')
		  {
			  tabs++;
		  }
		  else
		  {
			  break;
		  }
		}
		return p;
	  }

//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'p', so pointers on this parameter are left unchanged:
	  public static string findAssign(char * p, string end)
	  {
		for (; p < end; p++)
		{
		  if (*p == '\r' || *p == '\n')
		  {
			  return p;
		  }
		  if (p.Substring(1) < end && (p[0] == ':' && p[1] == '='))
		  {
			  return p;
		  }
		}
		return p;
	  }

//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'p', so pointers on this parameter are left unchanged:
	  public static string findSep(char * p, string end)
	  {
		for (; p < end; p++)
		{
		  if (*p == '\r' || *p == '\n')
		  {
			  return p;
		  }
		  if (p.Substring(4) < end && (p[0] == '&' && p[1] == 'e' && p[2] == 'n' && p[3] == 'd' && p[4] == '&'))
		  {
			  return p;
		  }
		}
		return p;
	  }

	  public static bool matchNew(string p, string end)
	  {
		return (p.Substring(3) < end) && p[0] == 'N' && p[1] == 'E' && p[2] == 'W' && (p[3] == ' ' || p[3] == '\t');
	  }

	  public static bool matchEnd(string p, string end)
	  {
		return (p.Substring(2) < end) && p[0] == 'E' && p[1] == 'N' && p[2] == 'D';
	  }




	public static bool parseAtt(Store store, Logger logger, object ptr, uint size, bool create)
	{
	  string buf = new string(new char[1024]);
	  Context ctx = new Context() {store = store, logger = logger, headerInfo = store.strings.intern("Header Information"), buf = buf, buf_size = sizeof(char)};

	  ctx.stack_c = 1024;
	  ctx.stack = (StackItem)xmalloc(sizeof(StackItem) * ctx.stack_c);
	  ctx.create = create;

	  var[] p = (string)(ptr);
	  var end = p + size;
	  p = getEndOfLine(p, end);
	  p = skipEndOfLine(p, end);

	  for (ctx.line = 1; p < end; ctx.line++)
	  {
		p = parseIndentation(ref ctx.spaces, ref ctx.tabs, p, end);
		if (matchNew(p, end))
		{
		  var a = skipSpace(p + 4, end);
		  p = getEndOfLine(a, end);
		  if (!handleNew(ctx, a, reverseSkipSpace(a, p)))
		  {
			  goto error;
		  }
		}
		else if (matchEnd(p, end))
		{
		  if (!handleEnd(ctx))
		  {
			  goto error;
		  }
		  p = getEndOfLine(p, end);
		}
		else
		{
		  while (true)
		  {
			var key_a = p;
			p = findAssign(p, end);
			if (p == end || p[0] != ':')
			{
			  logger(2, "@%d: Failed to find ':=' token.\n", ctx.line);
			  goto error;
			}
			var key_b = reverseSkipSpace(key_a, p);
			p = skipSpace(p + 2, end);

			var value_a = p;
			p = findSep(p, end);
			var[] value_b = reverseSkipSpace(value_a, p);
			if (2 <= (value_b - value_a) && value_a[0] == '\'' && value_b[-1] == '\'')
			{
			  value_a++;
			  value_b--;
			}
			if (!handleAttribute(ctx, key_a, key_b, value_a, value_b))
			{
				goto error;
			}

			if (p + 5 < end && p[0] == '&')
			{
			  p = skipSpace(p + 5, end);
			}
			else
			{
			  break;
			}
		  }
		}
		if ((p < end) && (*p != '\n' && *p != '\r'))
		{
		  logger(2, "@%d: Line scanning did not terminate at end of line", ctx.line);
		  goto error;
		}
		p = skipEndOfLine(p, end);
	  }

	  if (ctx.stack_p != 0)
	  {
		logger(2, "@%d: More NEW-tags and than END-tags.", ctx.line);
		return false;
	  }


	  ctx.stack = null;
	  store.updateCounts();
	  return true;

	error:
	  ctx.stack = null;
	  store.updateCounts();
	  return false;
	}


	  public static string read_uint8(ref byte rv, string curr_ptr, string end_ptr)
	  {
//C++ TO C# CONVERTER TODO TASK: There is no equivalent to 'reinterpret_cast' in C#:
		auto[] q = reinterpret_cast<byte>(curr_ptr);
		rv = q[0];
		return curr_ptr.Substring(1);
	  }

	  public static string read_uint32_be(ref uint rv, string curr_ptr, string end_ptr)
	  {
//C++ TO C# CONVERTER TODO TASK: There is no equivalent to 'reinterpret_cast' in C#:
		var[] q = reinterpret_cast<byte>(curr_ptr);
		rv = (uint)(q[0] << 24 | q[1] << 16 | q[2] << 8 | q[3]);
		return curr_ptr.Substring(4);
	  }
	//  {
	//	float f;
	//	uint u;
	//  };
	  public static string read_float32_be(ref float rv, string curr_ptr, string end_ptr)
	  {

//C++ TO C# CONVERTER TODO TASK: There is no equivalent to 'reinterpret_cast' in C#:
		var[] q = reinterpret_cast<byte>(curr_ptr);
		u = q[0] << 24 | q[1] << 16 | q[2] << 8 | q[3];
		rv = f;
		return curr_ptr.Substring(4);
	  }

	  public static uint id(string str)
	  {
		return (uint)(str[3] << 24 | str[2] << 16 | str[1] << 8 | str[0]);
	  }

	  public static string read_string(string[] dst, Store store, string curr_ptr, string end_ptr)
	  {
		uint len;
		curr_ptr = read_uint32_be(ref len, curr_ptr, end_ptr);

		uint l = (uint)(4 * len);
		for (uint i = 0; i < l; i++)
		{
		  if (curr_ptr[i] == 0)
		  {
			l = i;
			break;
		  }
		}
		dst[0] = store.strings.intern(curr_ptr, curr_ptr.Substring(l));
		return curr_ptr.Substring(4) * len;
	  }

//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'curr_ptr', so pointers on this parameter are left unchanged:
	  public static string parse_chunk_header(ref string id, ref uint next_chunk_offset, ref uint dunno, char * curr_ptr, string end_ptr)
	  {
		uint i = 0;
		for (i = 0; i < 4 && curr_ptr.Substring(4) <= end_ptr; i++)
		{
		  Debug.Assert(curr_ptr[0] == 0);
		  Debug.Assert(curr_ptr[1] == 0);
		  Debug.Assert(curr_ptr[2] == 0);
		  id[i] = curr_ptr[3];
		  curr_ptr += 4;
		}
		for (; i < 4; i++)
		{
		  id[i] = ' ';
		}
		if (curr_ptr.Substring(8) <= end_ptr)
		{
		  curr_ptr = read_uint32_be(ref next_chunk_offset, curr_ptr, end_ptr);
		  curr_ptr = read_uint32_be(ref dunno, curr_ptr, end_ptr);
		}
		else
		{
		  next_chunk_offset = (uint)(~0);
		  dunno = (uint)(~0);
//C++ TO C# CONVERTER TODO TASK: The following line has a C format specifier which cannot be directly translated to C#:
//ORIGINAL LINE: fprintf(stderr, "Chunk '%s' EOF after %zd bytes\n", id, end_ptr - curr_ptr);
		  Console.Error.Write("Chunk '{0}' EOF after %zd bytes\n", id, end_ptr - curr_ptr);
		  curr_ptr = end_ptr;
		}
		return curr_ptr;
	  }

	  public static bool verifyOffset(Context ctx, string chunk_type, string base_ptr, string curr_ptr, uint expected_next_chunk_offset)
	  {
		uint current_offset = (uint)(curr_ptr - base_ptr);
		if (current_offset == expected_next_chunk_offset)
		{
		  return true;
		}
		else
		{
		  snprintf(ctx.buf, ctx.buf_size, "After chunk %s, expected offset %#x, current offset is %#zx", chunk_type, expected_next_chunk_offset, current_offset);
		  ctx.store.setErrorString(ctx.buf);
		  return false;
		}
	  }

	  public static string parse_head(Context ctx, string base_ptr, string curr_ptr, string end_ptr, uint expected_next_chunk_offset)
	  {
		Debug.Assert(ctx.group_stack.Count == 0);
		var g = ctx.store.newGroup(null, Group.Kind.File);
		ctx.group_stack.Add(g);

		uint version;
		curr_ptr = read_uint32_be(ref version, curr_ptr, end_ptr);
		curr_ptr = read_string(g.file.info, ctx.store, curr_ptr, end_ptr);
		curr_ptr = read_string(g.file.note, ctx.store, curr_ptr, end_ptr);
		curr_ptr = read_string(g.file.date, ctx.store, curr_ptr, end_ptr);
		curr_ptr = read_string(g.file.user, ctx.store, curr_ptr, end_ptr);
		if (2 <= version)
		{
		  curr_ptr = read_string(g.file.encoding, ctx.store, curr_ptr, end_ptr);
		}
		else
		{
		  g.file.encoding = ctx.store.strings.intern("");
		}

		if (!verifyOffset(ctx, "HEAD", base_ptr, curr_ptr, expected_next_chunk_offset))
		{
			return null;
		}

		return curr_ptr;
	  }

	  public static string parse_modl(Context ctx, string base_ptr, string curr_ptr, string end_ptr, uint expected_next_chunk_offset)
	  {
		Debug.Assert(ctx.group_stack.Count > 0);
		var g = ctx.store.newGroup(new List<Group>(ctx.group_stack[ctx.group_stack.Count - 1]), Group.Kind.Model);
		ctx.group_stack.Add(g);

		uint version;
		curr_ptr = read_uint32_be(ref version, curr_ptr, end_ptr);

		curr_ptr = read_string(g.model.project, ctx.store, curr_ptr, end_ptr);
		curr_ptr = read_string(g.model.name, ctx.store, curr_ptr, end_ptr);

		//fprintf(stderr, "modl project='%s', name='%s'\n", g->model.project, g->model.name);

		if (!verifyOffset(ctx, "MODL", base_ptr, curr_ptr, expected_next_chunk_offset))
		{
			return null;
		}

		return curr_ptr;
	  }

	  public static string parse_prim(Context ctx, string base_ptr, string curr_ptr, string end_ptr, uint expected_next_chunk_offset)
	  {
		Debug.Assert(ctx.group_stack.Count > 0);
		if (ctx.group_stack[ctx.group_stack.Count - 1].kind != Group.Kind.Group)
		{
		  ctx.store.setErrorString("In PRIM, parent chunk is not CNTB");
		  return null;
		}

		uint version;
		uint kind;
		curr_ptr = read_uint32_be(ref version, curr_ptr, end_ptr);
		curr_ptr = read_uint32_be(ref kind, curr_ptr, end_ptr);

		var g = ctx.store.newGeometry(new List<Group>(ctx.group_stack[ctx.group_stack.Count - 1]));

		for (uint i = 0; i < 12; i++)
		{
		  curr_ptr = read_float32_be(ref g.M_3x4.data[i], curr_ptr, end_ptr);
		}
		for (uint i = 0; i < 6; i++)
		{
		  curr_ptr = read_float32_be(ref g.bboxLocal.data[i], curr_ptr, end_ptr);
		}
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: g->bboxWorld = transform(g->M_3x4, g->bboxLocal);
		g.bboxWorld.CopyFrom(transform(g.M_3x4, g.bboxLocal));

		switch (kind)
		{
		case 1:
		  g.kind = Geometry.Kind.Pyramid;
		  curr_ptr = read_float32_be(ref g.pyramid.bottom[0], curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.pyramid.bottom[1], curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.pyramid.top[0], curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.pyramid.top[1], curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.pyramid.offset[0], curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.pyramid.offset[1], curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.pyramid.height, curr_ptr, end_ptr);
		  break;

		case 2:
		  g.kind = Geometry.Kind.Box;
		  curr_ptr = read_float32_be(ref g.box.lengths[0], curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.box.lengths[1], curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.box.lengths[2], curr_ptr, end_ptr);
		  break;

		case 3:
		  g.kind = Geometry.Kind.RectangularTorus;
		  curr_ptr = read_float32_be(ref g.rectangularTorus.inner_radius, curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.rectangularTorus.outer_radius, curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.rectangularTorus.height, curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.rectangularTorus.angle, curr_ptr, end_ptr);
		  break;

		case 4:
		  g.kind = Geometry.Kind.CircularTorus;
		  curr_ptr = read_float32_be(ref g.circularTorus.offset, curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.circularTorus.radius, curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.circularTorus.angle, curr_ptr, end_ptr);
		  break;

		case 5:
		  g.kind = Geometry.Kind.EllipticalDish;
		  curr_ptr = read_float32_be(ref g.ellipticalDish.baseRadius, curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.ellipticalDish.height, curr_ptr, end_ptr);
		  break;

		case 6:
		  g.kind = Geometry.Kind.SphericalDish;
		  curr_ptr = read_float32_be(ref g.sphericalDish.baseRadius, curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.sphericalDish.height, curr_ptr, end_ptr);
		  break;

		case 7:
		  g.kind = Geometry.Kind.Snout;
		  curr_ptr = read_float32_be(ref g.snout.radius_b, curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.snout.radius_t, curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.snout.height, curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.snout.offset[0], curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.snout.offset[1], curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.snout.bshear[0], curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.snout.bshear[1], curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.snout.tshear[0], curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.snout.tshear[1], curr_ptr, end_ptr);
		  break;

		case 8:
		  g.kind = Geometry.Kind.Cylinder;
		  curr_ptr = read_float32_be(ref g.cylinder.radius, curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.cylinder.height, curr_ptr, end_ptr);
		  break;

		case 9:
		  g.kind = Geometry.Kind.Sphere;
		  curr_ptr = read_float32_be(ref g.sphere.diameter, curr_ptr, end_ptr);
		  break;

		case 10:
		  g.kind = Geometry.Kind.Line;
		  curr_ptr = read_float32_be(ref g.line.a, curr_ptr, end_ptr);
		  curr_ptr = read_float32_be(ref g.line.b, curr_ptr, end_ptr);
		  break;

		case 11:
		  g.kind = Geometry.Kind.FacetGroup;

		  curr_ptr = read_uint32_be(ref g.facetGroup.polygons_n, curr_ptr, end_ptr);
		  g.facetGroup.polygons = (Polygon)ctx.store.arena.alloc(sizeof(Polygon) * g.facetGroup.polygons_n);

		  for (uint pi = 0; pi < g.facetGroup.polygons_n; pi++)
		  {
			var poly = g.facetGroup.polygons[pi];

			curr_ptr = read_uint32_be(ref poly.contours_n, curr_ptr, end_ptr);
			poly.contours = (Contour)ctx.store.arena.alloc(sizeof(Contour) * poly.contours_n);
			for (uint gi = 0; gi < poly.contours_n; gi++)
			{
			  var cont = poly.contours[gi];

			  curr_ptr = read_uint32_be(ref cont.vertices_n, curr_ptr, end_ptr);
			  cont.vertices = (float)ctx.store.arena.alloc(3 * sizeof(float) * cont.vertices_n);
			  cont.normals = (float)ctx.store.arena.alloc(3 * sizeof(float) * cont.vertices_n);

			  for (uint vi = 0; vi < cont.vertices_n; vi++)
			  {
				for (uint i = 0; i < 3; i++)
				{
				  curr_ptr = read_float32_be(ref cont.vertices[3 * vi + i], curr_ptr, end_ptr);
				}
				for (uint i = 0; i < 3; i++)
				{
				  curr_ptr = read_float32_be(ref cont.normals[3 * vi + i], curr_ptr, end_ptr);
				}
			  }
			}
		  }
		  break;

		default:
		  snprintf(ctx.buf, ctx.buf_size, "In PRIM, unknown primitive kind %d", kind);
		  ctx.store.setErrorString(ctx.buf);
		  return null;
		}

		if (!verifyOffset(ctx, "PRIM", base_ptr, curr_ptr, expected_next_chunk_offset))
		{
			return null;
		}

		return curr_ptr;
	  }

	  public static string parse_cntb(Context ctx, string base_ptr, string curr_ptr, string end_ptr, uint expected_next_chunk_offset)
	  {
		Debug.Assert(ctx.group_stack.Count > 0);
		var g = ctx.store.newGroup(new List<Group>(ctx.group_stack[ctx.group_stack.Count - 1]), Group.Kind.Group);
		ctx.group_stack.Add(g);

		uint version;
		curr_ptr = read_uint32_be(ref version, curr_ptr, end_ptr);
		curr_ptr = read_string(g.group.name, ctx.store, curr_ptr, end_ptr);

		//fprintf(stderr, "group '%s' %p\n", g->group.name, g->group.name);

		// Translation seems to be a reference point that can be used as a local frame for objects in the group.
		// The transform is not relative to this reference point.
		for (uint i = 0; i < 3; i++)
		{
		  curr_ptr = read_float32_be(ref g.group.translation[i], curr_ptr, end_ptr);
		  g.group.translation[i] *= 0.001f;
		}

		curr_ptr = read_uint32_be(ref g.group.material, curr_ptr, end_ptr);

		if (!verifyOffset(ctx, "CNTB", base_ptr, curr_ptr, expected_next_chunk_offset))
		{
			return null;
		}

		// process children
		char[] chunk_id = {0, 0, 0, 0, 0};
		var l = curr_ptr;
		uint dunno;
		curr_ptr = parse_chunk_header(ref chunk_id, ref expected_next_chunk_offset, ref dunno, curr_ptr, end_ptr);
		var id_chunk_id = id(chunk_id);
		while (curr_ptr < end_ptr && id_chunk_id != id("CNTE"))
		{
		  switch (id_chunk_id)
		  {
		  case id("CNTB"):
			curr_ptr = parse_cntb(ctx, base_ptr, curr_ptr, end_ptr, expected_next_chunk_offset);
			if (curr_ptr == null)
			{
				return curr_ptr;
			}
			break;
		  case id("PRIM"):
			curr_ptr = parse_prim(ctx, base_ptr, curr_ptr, end_ptr, expected_next_chunk_offset);
			if (curr_ptr == null)
			{
				return curr_ptr;
			}
			break;
		  default:
			snprintf(ctx.buf, ctx.buf_size, "In CNTB, unknown chunk id %s", chunk_id);
			ctx.store.setErrorString(ctx.buf);
			return null;
		  }
		  l = curr_ptr;
		  curr_ptr = parse_chunk_header(ref chunk_id, ref expected_next_chunk_offset, ref dunno, curr_ptr, end_ptr);
		  id_chunk_id = id(chunk_id);
		}

		if (id_chunk_id == id("CNTE"))
		{
		  uint version;
		  curr_ptr = read_uint32_be(ref version, curr_ptr, end_ptr);
		}

		ctx.group_stack.RemoveAt(ctx.group_stack.Count - 1);

		//ctx->v->EndGroup();
		return curr_ptr;
	  }

//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'curr_ptr', so pointers on this parameter are left unchanged:
	  public static string parse_colr(Context ctx, string base_ptr, char * curr_ptr, string end_ptr, uint expected_next_chunk_offset)
	  {
		Debug.Assert(ctx.group_stack.Count > 0);
		if (ctx.group_stack[ctx.group_stack.Count - 1].kind != Group.Kind.Model)
		{
		  ctx.store.setErrorString("Model chunk unfinished.");
		  return null;
		}
		auto g = ctx.store.newColor(new List<Group>(ctx.group_stack[ctx.group_stack.Count - 1]));
		curr_ptr = read_uint32_be(ref g.colorKind, curr_ptr, end_ptr);
		curr_ptr = read_uint32_be(ref g.colorIndex, curr_ptr, end_ptr);
		for (uint i = 0; i < 3; i++)
		{
		  curr_ptr = read_uint8(ref g.rgb[i], curr_ptr, end_ptr);
		}
		curr_ptr += 1;

		if (!verifyOffset(ctx, "COLR", base_ptr, curr_ptr, expected_next_chunk_offset))
		{
			return null;
		}

		return curr_ptr;
	  }


	public static bool parseRVM(Store store, object ptr, uint size)
	{
	  string buf = new string(new char[1024]);
	  Context ctx = new Context() {store = store, buf = buf, buf_size = sizeof(char)};

//C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent to pointers to value types:
//ORIGINAL LINE: const char* base_ptr = reinterpret_cast<const char*>(ptr);
//C++ TO C# CONVERTER TODO TASK: There is no equivalent to 'reinterpret_cast' in C#:
	  char base_ptr = reinterpret_cast<const char>(ptr);
	  string curr_ptr = base_ptr;
	  string end_ptr = curr_ptr.Substring(size);

	  uint expected_next_chunk_offset;
	  uint dunno;


	  char[] chunk_id = {0, 0, 0, 0, 0};
	  curr_ptr = parse_chunk_header(ref chunk_id, ref expected_next_chunk_offset, ref dunno, curr_ptr, end_ptr);
	  if (id(chunk_id) != id("HEAD"))
	  {
		snprintf(ctx.buf, ctx.buf_size, "Expected chunk HEAD, got %s", chunk_id);
		store.setErrorString(buf);
		return false;
	  }
	  curr_ptr = parse_head(ctx, base_ptr, curr_ptr, end_ptr, expected_next_chunk_offset);
	  if (curr_ptr == null)
	  {
		  return false;
	  }

	  curr_ptr = parse_chunk_header(ref chunk_id, ref expected_next_chunk_offset, ref dunno, curr_ptr, end_ptr);
	  if (id(chunk_id) != id("MODL"))
	  {
		snprintf(ctx.buf, ctx.buf_size, "Expected chunk MODL, got %s",chunk_id);
		store.setErrorString(buf);
		return false;
	  }
	  curr_ptr = parse_modl(ctx, base_ptr, curr_ptr, end_ptr, expected_next_chunk_offset);
	  if (curr_ptr == null)
	  {
		  return false;
	  }

	  curr_ptr = parse_chunk_header(ref chunk_id, ref expected_next_chunk_offset, ref dunno, curr_ptr, end_ptr);
	  var id_chunk_id = id(chunk_id);
	  while (curr_ptr < end_ptr && id_chunk_id != id("END:"))
	  {
		switch (id_chunk_id)
		{
		case id("CNTB"):
		  curr_ptr = parse_cntb(ctx, base_ptr, curr_ptr, end_ptr, expected_next_chunk_offset);
		  if (curr_ptr == null)
		  {
			  return false;
		  }
		  break;
		case id("PRIM"):
		  curr_ptr = parse_prim(ctx, base_ptr, curr_ptr, end_ptr, expected_next_chunk_offset);
		  if (curr_ptr == null)
		  {
			  return false;
		  }
		  break;
		case id("COLR"):
		  curr_ptr = parse_colr(ctx, base_ptr, curr_ptr, end_ptr, expected_next_chunk_offset);
		  if (curr_ptr == null)
		  {
			  return false;
		  }
		  break;
		default:
		  snprintf(ctx.buf, ctx.buf_size, "Unrecognized chunk %s", chunk_id);
		  store.setErrorString(buf);
		  return false;
		}
		if (curr_ptr < end_ptr)
		{
		  curr_ptr = parse_chunk_header(ref chunk_id, ref expected_next_chunk_offset, ref dunno, curr_ptr, end_ptr);
		  id_chunk_id = id(chunk_id);
		}
	  }

	  Debug.Assert(ctx.group_stack.Count == 2);
	  ctx.group_stack.RemoveAt(ctx.group_stack.Count - 1);
	  ctx.group_stack.RemoveAt(ctx.group_stack.Count - 1);

	  store.updateCounts();

	  return true;
	}



	  public static void insert<T>(ListHeader<T> list, T item)
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

	  public static void storeGroupIndexInGeometriesRecurse(Group group)
	  {
		for (var * child = group.groups.first; child != null; child = child.next)
		{
		  storeGroupIndexInGeometriesRecurse(child);
		}
		if (group.kind == Group.Kind.Group)
		{
		  for (var * geo = group.group.geometries.first; geo != null; geo = geo.next)
		  {
			geo.id = group.group.id;
			if (geo.triangulation)
			{
			  geo.triangulation.id = group.group.id;
			}
		  }
		}
	  }



	  public static readonly float pi = M_PI;
	  public static readonly float half_pi = 0.5 * M_PI;

	  public static readonly float pi = M_PI;
	  public static readonly float half_pi = 0.5 * M_PI;
	  public static readonly float one_over_pi = 1.0 / M_PI;
	  public static readonly float twopi = 2.0 * M_PI;

	  public static Interface getInterface(Geometry[] geo, uint o)
	  {
		Interface @interface = new Interface();
		var connection = geo.connections[o];
		var ix = connection.geo[0] == geo ? 1 : 0;
		var scale = getScale(geo.M_3x4);
		switch (geo.kind)
		{
		case Geometry.Kind.Pyramid:
		{
		  var bx = 0.5f * geo.pyramid.bottom[0];
		  var by = 0.5f * geo.pyramid.bottom[1];
		  var tx = 0.5f * geo.pyramid.top[0];
		  var ty = 0.5f * geo.pyramid.top[1];
		  var ox = 0.5f * geo.pyramid.offset[0];
		  var oy = 0.5f * geo.pyramid.offset[1];
		  var h2 = 0.5f * geo.pyramid.height;
		  Vec3f[][] quad =
		  {
			  new Vec3f[]
			  {
				  new Vec3f(-bx - ox, -by - oy, -h2),
				  new Vec3f(bx - ox, -by - oy, -h2),
				  new Vec3f(bx - ox, by - oy, -h2),
				  new Vec3f(-bx - ox, by - oy, -h2)
			  },
			  new Vec3f[]
			  {
				  new Vec3f(-tx + ox, -ty + oy, h2),
				  new Vec3f(tx + ox, -ty + oy, h2),
				  new Vec3f(tx + ox, ty + oy, h2),
				  new Vec3f(-tx + ox, ty + oy, h2)
			  }
		  };

		  @interface.kind = Interface.Kind.Square;
		  if (o < 4)
		  {
			uint oo = (o + 1) & 3;
			@interface.square.p[0] = mul(geo.M_3x4, quad[0][o]);
			@interface.square.p[1] = mul(geo.M_3x4, quad[0][oo]);
			@interface.square.p[2] = mul(geo.M_3x4, quad[1][oo]);
			@interface.square.p[3] = mul(geo.M_3x4, quad[1][o]);
		  }
		  else
		  {
			for (uint k = 0; k < 4; k++)
			{
				@interface.square.p[k] = mul(geo.M_3x4, quad[o - 4][k].data);
			}
		  }
		  break;
		}
		case Geometry.Kind.Box:
		{
		  var box = geo.box;
		  var xp = 0.5f * box.lengths[0];
		  var xm = -xp;
		  var yp = 0.5f * box.lengths[1];
		  var ym = -yp;
		  var zp = 0.5f * box.lengths[2];
		  var zm = -zp;
		  Vec3f[][] V =
		  {
			  new Vec3f[]
			  {
				  new Vec3f(xm, ym, zp),
				  new Vec3f(xm, yp, zp),
				  new Vec3f(xm, yp, zm),
				  new Vec3f(xm, ym, zm)
			  },
			  new Vec3f[]
			  {
				  new Vec3f(xp, ym, zm),
				  new Vec3f(xp, yp, zm),
				  new Vec3f(xp, yp, zp),
				  new Vec3f(xp, ym, zp)
			  },
			  new Vec3f[]
			  {
				  new Vec3f(xp, ym, zm),
				  new Vec3f(xp, ym, zp),
				  new Vec3f(xm, ym, zp),
				  new Vec3f(xm, ym, zm)
			  },
			  new Vec3f[]
			  {
				  new Vec3f(xm, yp, zm),
				  new Vec3f(xm, yp, zp),
				  new Vec3f(xp, yp, zp),
				  new Vec3f(xp, yp, zm)
			  },
			  new Vec3f[]
			  {
				  new Vec3f(xm, yp, zm),
				  new Vec3f(xp, yp, zm),
				  new Vec3f(xp, ym, zm),
				  new Vec3f(xm, ym, zm)
			  },
			  new Vec3f[]
			  {
				  new Vec3f(xm, ym, zp),
				  new Vec3f(xp, ym, zp),
				  new Vec3f(xp, yp, zp),
				  new Vec3f(xm, yp, zp)
			  }
		  };
		  for (uint k = 0; k < 4; k++)
		  {
			  @interface.square.p[k] = mul(geo.M_3x4, V[o][k].data);
		  }
		  break;
		}
		case Geometry.Kind.RectangularTorus:
		{
		  var tor = geo.rectangularTorus;
		  var h2 = 0.5f * tor.height;
		  float[][] square =
		  {
			  new float[] {tor.outer_radius, -h2},
			  new float[] {tor.inner_radius, -h2},
			  new float[] {tor.inner_radius, h2},
			  new float[] {tor.outer_radius, h2}
		  };
		  if (o == 0)
		  {
			for (uint k = 0; k < 4; k++)
			{
			  @interface.square.p[k] = mul(geo.M_3x4, new Vec3f(square[k][0], 0.0f, square[k][1]));
			}
		  }
		  else
		  {
			for (uint k = 0; k < 4; k++)
			{
			  @interface.square.p[k] = mul(geo.M_3x4, new Vec3f(square[k][0] * Math.Cos(tor.angle), square[k][0] * Math.Sin(tor.angle), square[k][1]));
			}
		  }
		  break;
		}
		case Geometry.Kind.CircularTorus:
		  @interface.kind = Interface.Kind.Circular;
		  @interface.circular.radius = scale * geo.circularTorus.radius;
		  break;

		case Geometry.Kind.EllipticalDish:
		  @interface.kind = Interface.Kind.Circular;
		  @interface.circular.radius = scale * geo.ellipticalDish.baseRadius;
		  break;

		case Geometry.Kind.SphericalDish:
		{
		  float r_circ = geo.sphericalDish.baseRadius;
		  var h = geo.sphericalDish.height;
		  float r_sphere = (r_circ * r_circ + h * h) / (2.0f * h);
		  @interface.kind = Interface.Kind.Circular;
		  @interface.circular.radius = scale * r_sphere;
		  break;
		}
		case Geometry.Kind.Snout:
		  @interface.kind = Interface.Kind.Circular;
		  @interface.circular.radius = scale * (connection.offset[ix] == 0 ? geo.snout.radius_b : geo.snout.radius_t);
		  break;
		case Geometry.Kind.Cylinder:
		  @interface.kind = Interface.Kind.Circular;
		  @interface.circular.radius = scale * geo.cylinder.radius;
		  break;
		case Geometry.Kind.Sphere:
		case Geometry.Kind.Line:
		case Geometry.Kind.FacetGroup:
		  @interface.kind = Interface.Kind.Undefined;
		  break;

		default:
		  Debug.Assert(false && "Unhandled primitive type");
		  break;
		}
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: return interface;
		return new Interface(@interface);
	  }

	  public static bool doInterfacesMatch(Geometry[] geo, Connection con)
	  {
		bool isFirst = geo == con.geo[0];

		var thisGeo = con.geo[isFirst ? 0 : 1];
		var thisOffset = con.offset[isFirst ? 0 : 1];
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: auto thisIFace = getInterface(thisGeo, thisOffset);
		var thisIFace = getInterface(new Geometry(thisGeo), thisOffset);

		var thatGeo = con.geo[isFirst ? 1 : 0];
		var thatOffset = con.offset[isFirst ? 1 : 0];
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: auto thatIFace = getInterface(thatGeo, thatOffset);
		var thatIFace = getInterface(new Geometry(thatGeo), thatOffset);


		if (thisIFace.kind != thatIFace.kind)
		{
			return false;
		}

		if (thisIFace.kind == Interface.Kind.Circular)
		{

		  return thisIFace.circular.radius <= 1.05f * thatIFace.circular.radius;

		}
		else
		{
		  for (uint j = 0; j < 4; j++)
		  {
			bool found = false;
			for (uint i = 0; i < 4; i++)
			{
			  if (distanceSquared(thisIFace.square.p[j], thatIFace.square.p[i]) < 0.001f * 0.001f)
			  {
				found = true;
			  }
			}
			if (!found)
			{
				return false;
			}
		  }
		  return true;
		}
	  }


	  // FIXME: replace use of these with stuff from linalg.
	  public static void sub3(float[] dst, float[] a, float[] b)
	  {
		dst[0] = a[0] - b[0];
		dst[1] = a[1] - b[1];
		dst[2] = a[2] - b[2];
	  }

	  public static void cross3(float[] dst, float[] a, float[] b)
	  {
		dst[0] = a[1] * b[2] - a[2] * b[1];
		dst[1] = a[2] * b[0] - a[0] * b[2];
		dst[2] = a[0] * b[1] - a[1] * b[0];
	  }

	  public static float dot3(float[] a, float[] b)
	  {
		return a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
	  }

	  public static uint triIndices(uint[] indices, uint l, uint o, uint v0, uint v1, uint v2)
	  {
		indices[l++] = o + v0;
		indices[l++] = o + v1;
		indices[l++] = o + v2;
		return l;
	  }


	  public static uint quadIndices(uint[] indices, uint l, uint o, uint v0, uint v1, uint v2, uint v3)
	  {
		indices[l++] = o + v0;
		indices[l++] = o + v1;
		indices[l++] = o + v2;

		indices[l++] = o + v2;
		indices[l++] = o + v3;
		indices[l++] = o + v0;
		return l;
	  }

	  public static uint vertex(float[] normals, float[] vertices, uint l, float[] n, float[] p)
	  {
		normals[l] = n[0];
		vertices[l++] = p[0];
		normals[l] = n[1];
		vertices[l++] = p[1];
		normals[l] = n[2];
		vertices[l++] = p[2];
		return l;
	  }

	  public static uint vertex(float[] normals, float[] vertices, uint l, float nx, float ny, float nz, float px, float py, float pz)
	  {
		normals[l] = nx;
		vertices[l++] = px;
		normals[l] = ny;
		vertices[l++] = py;
		normals[l] = nz;
		vertices[l++] = pz;
		return l;
	  }

	  public static uint tessellateCircle(uint[] indices, uint l, uint[] t, uint[] src, uint N)
	  {
		while (3 <= N)
		{
		  uint m = 0;
		  uint i;
		  for (i = 0; i + 2 < N; i += 2)
		  {
			indices[l++] = src[i];
			indices[l++] = src[i + 1];
			indices[l++] = src[i + 2];
			t[m++] = src[i];
		  }
		  for (; i < N; i++)
		  {
			t[m++] = src[i];
		  }
		  N = m;
		  std::swap(t, src);
		}
		return l;

	  }
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
	//bool parseAtt(Store store, Logger logger, object ptr, uint size, bool create = false);

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
	//bool parseRVM(Store store, object ptr, uint size);

}