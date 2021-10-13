using System;
using System.Diagnostics;

//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define TESS_UNDEF (~(TESSindex)0)
//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define TESS_NOTUSED(v) do { (void)(1 ? (void)0 : ( (void)(v) ) ); } while(0)


//C++ TO C# CONVERTER NOTE: C# does not allow anonymous namespaces:
//namespace

  public class Interface
  {
	public enum Kind
	{
	  Undefined,
	  Square,
	  Circular
	}
	public Kind kind = Kind.Undefined;

//C++ TO C# CONVERTER TODO TASK: Unions are not supported in C#:
//	union
//	{
//	  struct
//	  {
//		Vec3f p[4];
//	  }
//	  square;
//	  struct
//	  {
//		float radius;
//	  }
//	  circular;
//	};

  }



//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
//ORIGINAL LINE: TriangulationFactory::TriangulationFactory(Store* store, Logger logger, float tolerance, uint minSamples, uint maxSamples) : store(store), logger(logger), tolerance(tolerance), minSamples(minSamples), maxSamples(std::max(minSamples, maxSamples))


//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
//ORIGINAL LINE: uint TriangulationFactory::sagittaBasedSegmentCount(float arc, float radius, float scale)


//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
//ORIGINAL LINE: float TriangulationFactory::sagittaBasedError(float arc, float radius, float scale, uint segments)




//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
//ORIGINAL LINE: Triangulation* TriangulationFactory::pyramid(Arena* arena, const Geometry* geo, float scale)


//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
//ORIGINAL LINE: Triangulation* TriangulationFactory::box(Arena* arena, const Geometry* geo, float scale)


//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
//ORIGINAL LINE: Triangulation* TriangulationFactory::rectangularTorus(Arena* arena, const Geometry* geo, float scale)


//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
//ORIGINAL LINE: Triangulation* TriangulationFactory::circularTorus(Arena* arena, const Geometry* geo, float scale)


//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
//ORIGINAL LINE: Triangulation* TriangulationFactory::snout(Arena* arena, const Geometry* geo, float scale)


//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
//ORIGINAL LINE: Triangulation* TriangulationFactory::cylinder(Arena* arena, const Geometry* geo, float scale)


//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
//ORIGINAL LINE: Triangulation* TriangulationFactory::sphereBasedShape(Arena* arena, const Geometry* geo, float radius, float arc, float shift_z, float scale_z, float scale)


//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
//ORIGINAL LINE: Triangulation* TriangulationFactory::facetGroup(Arena* arena, const Geometry* geo, float scale)



public partial class TriangulationFactory
{
	public TriangulationFactory(Store store, Logger logger, float tolerance, uint minSamples, uint maxSamples)
	{
		this.store = store;
		this.logger = logger;
		this.tolerance = tolerance;
		this.minSamples = minSamples;
		this.maxSamples = Math.Max(minSamples, maxSamples);
	}
	public uint sagittaBasedSegmentCount(float arc, float radius, float scale)
	{
	  float samples = arc / Math.Acos(Math.Max(-1.0f, 1.0f - tolerance / (scale * radius)));
	  return Math.Min(maxSamples, (uint)Math.Max((float)minSamples, Math.Ceiling(samples)));
	}
	public float sagittaBasedError(float arc, float radius, float scale, uint segments)
	{
	  var s = scale * radius * (1.0f - Math.Cos(arc / segments)); // Length of sagitta
	  //assert(s <= tolerance);
	  return s;
	}
	public Triangulation pyramid(Arena arena, Geometry geo, float scale)
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
    
	  Vec3f[] n =
	  {
		  new Vec3f(0.0f, -h2, (quad[1][0][1] - quad[0][0][1])),
		  new Vec3f(h2, 0.0f, -(quad[1][1][0] - quad[0][1][0])),
		  new Vec3f(0.0f, h2, -(quad[1][2][1] - quad[0][2][1])),
		  new Vec3f(-h2, 0.0f, (quad[1][3][0] - quad[0][3][0])),
		  new Vec3f(0F, 0F, -1F),
		  new Vec3f(0F, 0F, 1F)
	  };
    
	  bool[] cap = {true, true, true, true, 1e-7f <= Math.Min(Math.Abs(geo.pyramid.bottom[0]), Math.Abs(geo.pyramid.bottom[1])), 1e-7f <= Math.Min(Math.Abs(geo.pyramid.top[0]), Math.Abs(geo.pyramid.top[1]))};
    
	  for (uint i = 0; i < 6; i++)
	  {
		var con = geo.connections[i];
		if (cap[i] == false || con == null || con.flags != Connection.Flags.HasRectangularSide)
		{
			continue;
		}
    
	//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
	//ORIGINAL LINE: if (doInterfacesMatch(geo, con))
		if (doInterfacesMatch(geo, new Connection(con)))
		{
		  cap[i] = false;
		  discardedCaps++;
		  //store->addDebugLine(con->p.data, (con->p.data + 0.05f*con->d).data, 0xff0000);
		}
	  }
    
	  uint caps = 0;
	  for (uint i = 0; i < 6; i++)
	  {
		  if (cap[i])
		  {
			  caps++;
		  }
	  }
    
	  var tri = arena.alloc<Triangulation>();
	  tri.error = 0.0f;
    
	  tri.vertices_n = (uint)(4 * caps);
	  tri.triangles_n = (uint)(2 * caps);
    
	  tri.vertices = (float)arena.alloc(3 * sizeof(float) * tri.vertices_n);
	  tri.normals = (float)arena.alloc(3 * sizeof(float) * tri.vertices_n);
    
	  uint l = 0;
	  for (uint i = 0; i < 4; i++)
	  {
		if (cap[i] == false)
		{
			continue;
		}
		uint ii = (i + 1) & 3;
		l = vertex(tri.normals, tri.vertices, l, n[i].data, quad[0][i].data);
		l = vertex(tri.normals, tri.vertices, l, n[i].data, quad[0][ii].data);
		l = vertex(tri.normals, tri.vertices, l, n[i].data, quad[1][ii].data);
		l = vertex(tri.normals, tri.vertices, l, n[i].data, quad[1][i].data);
	  }
	  if (cap[4])
	  {
		for (uint i = 0; i < 4; i++)
		{
		  l = vertex(tri.normals, tri.vertices, l, n[4].data, quad[0][i].data);
		}
	  }
	  if (cap[5])
	  {
		for (uint i = 0; i < 4; i++)
		{
		  l = vertex(tri.normals, tri.vertices, l, n[5].data, quad[1][i].data);
		}
	  }
	  Debug.Assert(l == 3 * tri.vertices_n);
    
	  l = 0;
	  uint o = 0;
	  tri.indices = (uint)arena.alloc(3 * sizeof(uint) * tri.triangles_n);
	  for (uint i = 0; i < 4; i++)
	  {
		if (cap[i] == false)
		{
			continue;
		}
		l = quadIndices(tri.indices, l, o, 0, 1, 2, 3);
		o += 4;
	  }
	  if (cap[4])
	  {
		l = quadIndices(tri.indices, l, o, 3, 2, 1, 0);
		o += 4;
	  }
	  if (cap[5])
	  {
		l = quadIndices(tri.indices, l, o, 0, 1, 2, 3);
		o += 4;
	  }
	  Debug.Assert(l == 3 * tri.triangles_n);
	  Debug.Assert(o == tri.vertices_n);
    
	  return tri;
	}
	public Triangulation box(Arena arena, Geometry geo, float scale)
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
    
	  Vec3f[] N =
	  {
		  new Vec3f(-1F, 0F, 0F),
		  new Vec3f(1F, 0F, 0F),
		  new Vec3f(0F, -1F, 0F),
		  new Vec3f(0F, 1F, 0F),
		  new Vec3f(0F, 0F, -1F),
		  new Vec3f(0F, 0F, 1F)
	  };
    
	  bool[] faces = {1e-5 <= box.lengths[0], 1e-5 <= box.lengths[0], 1e-5 <= box.lengths[1], 1e-5 <= box.lengths[1], 1e-5 <= box.lengths[2], 1e-5 <= box.lengths[2]};
	  for (uint i = 0; i < 6; i++)
	  {
		var con = geo.connections[i];
		if (faces[i] == false || con == null || con.flags != Connection.Flags.HasRectangularSide)
		{
			continue;
		}
    
	//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
	//ORIGINAL LINE: if (doInterfacesMatch(geo, con))
		if (doInterfacesMatch(geo, new Connection(con)))
		{
		  faces[i] = false;
		  discardedCaps++;
		  //store->addDebugLine(con->p.data, (con->p.data + 0.05f*con->d).data, 0xff0000);
		}
    
	  }
    
	  uint faces_n = 0;
	  for (uint i = 0; i < 6; i++)
	  {
		if (faces[i])
		{
			faces_n++;
		}
	  }
    
	  Triangulation tri = arena.alloc<Triangulation>();
	  tri.error = 0.0f;
    
	  if (faces_n != 0)
	  {
		tri.vertices_n = (uint)(4 * faces_n);
		tri.vertices = (float)arena.alloc(3 * sizeof(float) * tri.vertices_n);
		tri.normals = (float)arena.alloc(3 * sizeof(float) * tri.vertices_n);
    
		tri.triangles_n = (uint)(2 * faces_n);
		tri.indices = (uint)arena.alloc(3 * sizeof(uint) * tri.triangles_n);
    
		uint o = 0;
		uint i_v = 0;
		uint i_p = 0;
		for (uint f = 0; f < 6; f++)
		{
		  if (!faces[f])
		  {
			  continue;
		  }
    
		  for (uint i = 0; i < 4; i++)
		  {
			i_v = vertex(tri.normals, tri.vertices, i_v, N[f].data, V[f][i].data);
		  }
		  i_p = quadIndices(tri.indices, i_p, o, 0, 1, 2, 3);
    
		  o += 4;
		}
		Debug.Assert(i_v == 3 * tri.vertices_n);
		Debug.Assert(i_p == 3 * tri.triangles_n);
		Debug.Assert(o == tri.vertices_n);
	  }
    
	  return tri;
	}
	public Triangulation rectangularTorus(Arena arena, Geometry geo, float scale)
	{
	  //if (cullTiny && std::max(tor.outer_radius-tor.inner_radius, tor.height)*scale < tolerance) {
	  //  tri->error = std::max(tor.outer_radius - tor.inner_radius, tor.height)*scale;
	  //  return;
	  //}
    
	  var tor = geo.rectangularTorus;
	  var segments = sagittaBasedSegmentCount(tor.angle, tor.outer_radius, scale);
	  var samples = segments + 1; // Assumed to be open, add extra sample.
    
	  var tri = arena.alloc<Triangulation>();
	  tri.error = sagittaBasedError(tor.angle, tor.outer_radius, scale, segments);
    
	  bool shell = true;
	  bool[] cap = {true, true};
    
	  for (uint i = 0; i < 2; i++)
	  {
		var con = geo.connections[i];
		if (con != null && con.flags == Connection.Flags.HasRectangularSide)
		{
	//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
	//ORIGINAL LINE: if (doInterfacesMatch(geo, con))
		  if (doInterfacesMatch(geo, new Connection(con)))
		  {
			cap[i] = false;
			discardedCaps++;
			//store->addDebugLine(con->p.data, (con->p.data + 0.05f*con->d).data, 0xff0000);
		  }
		}
	  }
    
	  var h2 = 0.5f * tor.height;
	  float[][] square =
	  {
		  new float[] {tor.outer_radius, -h2},
		  new float[] {tor.inner_radius, -h2},
		  new float[] {tor.inner_radius, h2},
		  new float[] {tor.outer_radius, h2}
	  };
    
	  // Not closed
	  t0.resize(2 * samples + 1);
	  for (uint i = 0; i < samples; i++)
	  {
		t0[2 * i + 0] = Math.Cos((tor.angle / segments) * i);
		t0[2 * i + 1] = Math.Sin((tor.angle / segments) * i);
	  }
    
	  uint l = 0;
    
	  tri.vertices_n = (uint)((shell ? 4 * 2 * samples : 0) + (cap[0] ? 4 : 0) + (cap[1] ? 4 : 0));
	  tri.vertices = (float)arena.alloc(3 * sizeof(float) * tri.vertices_n);
	  tri.normals = (float)arena.alloc(3 * sizeof(float) * tri.vertices_n);
    
	  if (shell)
	  {
		for (uint i = 0; i < samples; i++)
		{
		  float[][] n =
		  {
			  new float[] {0.0f, 0.0f, -1.0f},
			  new float[] {-t0[2 * i + 0], -t0[2 * i + 1], 0.0f},
			  new float[] {0.0f, 0.0f, 1.0f},
			  new float[] {t0[2 * i + 0], t0[2 * i + 1], 0.0f}
		  };
    
		  for (uint k = 0; k < 4; k++)
		  {
			uint kk = (k + 1) & 3;
    
			tri.normals[l] = n[k][0];
			tri.vertices[l++] = square[k][0] * t0[2 * i + 0];
			tri.normals[l] = n[k][1];
			tri.vertices[l++] = square[k][0] * t0[2 * i + 1];
			tri.normals[l] = n[k][2];
			tri.vertices[l++] = square[k][1];
    
			tri.normals[l] = n[k][0];
			tri.vertices[l++] = square[kk][0] * t0[2 * i + 0];
			tri.normals[l] = n[k][1];
			tri.vertices[l++] = square[kk][0] * t0[2 * i + 1];
			tri.normals[l] = n[k][2];
			tri.vertices[l++] = square[kk][1];
		  }
		}
	  }
	  if (cap[0])
	  {
		for (uint k = 0; k < 4; k++)
		{
		  tri.normals[l] = 0.0f;
		  tri.vertices[l++] = square[k][0] * t0[0];
		  tri.normals[l] = -1.0f;
		  tri.vertices[l++] = square[k][0] * t0[1];
		  tri.normals[l] = 0.0f;
		  tri.vertices[l++] = square[k][1];
		}
	  }
	  if (cap[1])
	  {
		for (uint k = 0; k < 4; k++)
		{
		  tri.normals[l] = -t0[2 * (samples - 1) + 1];
		  tri.vertices[l++] = square[k][0] * t0[2 * (samples - 1) + 0];
		  tri.normals[l] = t0[2 * (samples - 1) + 0];
		  tri.vertices[l++] = square[k][0] * t0[2 * (samples - 1) + 1];
		  tri.normals[l] = 0.0f;
		  tri.vertices[l++] = square[k][1];
		}
	  }
	  Debug.Assert(l == 3 * tri.vertices_n);
    
	  l = 0;
	  uint o = 0;
    
	  tri.triangles_n = (uint)((shell ? 4 * 2 * (samples - 1) : 0) + (cap[0] ? 2 : 0) + (cap[1] ? 2 : 0));
	  tri.indices = (uint)arena.alloc(3 * sizeof(uint) * tri.triangles_n);
    
	  if (shell)
	  {
		for (uint i = 0; i + 1 < samples; i++)
		{
		  for (uint k = 0; k < 4; k++)
		  {
			tri.indices[l++] = (uint)(4 * 2 * (i + 0) + 0 + 2 * k);
			tri.indices[l++] = (uint)(4 * 2 * (i + 0) + 1 + 2 * k);
			tri.indices[l++] = (uint)(4 * 2 * (i + 1) + 0 + 2 * k);
    
			tri.indices[l++] = (uint)(4 * 2 * (i + 1) + 0 + 2 * k);
			tri.indices[l++] = (uint)(4 * 2 * (i + 0) + 1 + 2 * k);
			tri.indices[l++] = (uint)(4 * 2 * (i + 1) + 1 + 2 * k);
		  }
		}
		o += 4 * 2 * samples;
	  }
	  if (cap[0])
	  {
		tri.indices[l++] = o + 0;
		tri.indices[l++] = o + 2;
		tri.indices[l++] = o + 1;
		tri.indices[l++] = o + 2;
		tri.indices[l++] = o + 0;
		tri.indices[l++] = o + 3;
		o += 4;
	  }
	  if (cap[1])
	  {
		tri.indices[l++] = o + 0;
		tri.indices[l++] = o + 1;
		tri.indices[l++] = o + 2;
		tri.indices[l++] = o + 2;
		tri.indices[l++] = o + 3;
		tri.indices[l++] = o + 0;
		o += 4;
	  }
	  Debug.Assert(o == tri.vertices_n);
	  Debug.Assert(l == 3 * tri.triangles_n);
    
	  return tri;
	}
	public Triangulation circularTorus(Arena arena, Geometry geo, float scale)
	{
	  //if (cullTiny && ct.radius*scale < tolerance) {
	  //  tri->error = ct.radius*scale;
	  //  return;
	  //}
    
	  var ct = geo.circularTorus;
	  uint segments_l = sagittaBasedSegmentCount(ct.angle, ct.offset + ct.radius, scale); // large radius, toroidal direction
	  uint segments_s = sagittaBasedSegmentCount(twopi, ct.radius, scale); // small radius, poloidal direction
    
	  var tri = arena.alloc<Triangulation>();
	  tri.error = Math.Max(sagittaBasedError(ct.angle, ct.offset + ct.radius, scale, segments_l), sagittaBasedError(twopi, ct.radius, scale, segments_s));
    
	  uint samples_l = segments_l + 1; // Assumed to be open, add extra sample
	  uint samples_s = segments_s; // Assumed to be closed
    
	  bool shell = true;
	  bool[] cap = {true, true};
	  for (uint i = 0; i < 2; i++)
	  {
		var con = geo.connections[i];
		if (con != null && con.flags == Connection.Flags.HasCircularSide)
		{
	//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
	//ORIGINAL LINE: if (doInterfacesMatch(geo, con))
		  if (doInterfacesMatch(geo, new Connection(con)))
		  {
			cap[i] = false;
			discardedCaps++;
		  }
		  else
		  {
			//store->addDebugLine(con->p.data, (con->p.data + 0.05f*con->d).data, 0x00ffff);
		  }
		}
	  }
    
	  t0.resize(2 * samples_l);
	  for (uint i = 0; i < samples_l; i++)
	  {
		t0[2 * i + 0] = Math.Cos((ct.angle / (samples_l - 1.f)) * i);
		t0[2 * i + 1] = Math.Sin((ct.angle / (samples_l - 1.f)) * i);
	  }
    
	  t1.resize(2 * samples_s);
	  for (uint i = 0; i < samples_s; i++)
	  {
		t1[2 * i + 0] = Math.Cos((twopi / samples_s) * i + geo.sampleStartAngle);
		t1[2 * i + 1] = Math.Sin((twopi / samples_s) * i + geo.sampleStartAngle);
	  }
    
    
	  tri.vertices_n = ((shell ? samples_l : 0) + (cap[0] ? 1 : 0) + (cap[1] ? 1 : 0)) * samples_s;
	  tri.vertices = (float)arena.alloc(3 * sizeof(float) * tri.vertices_n);
	  tri.normals = (float)arena.alloc(3 * sizeof(float) * tri.vertices_n);
    
	  tri.triangles_n = (uint)((shell ? 2 * (samples_l - 1) * samples_s : 0) + (samples_s - 2) * ((cap[0] ? 1 : 0) + (cap[1] ? 1 : 0)));
	  tri.indices = (uint)arena.alloc(3 * sizeof(uint) * tri.triangles_n);
    
	  // generate vertices
	  uint l = 0;
    
	  if (shell)
	  {
		//Vec3f n(cos(twopi *v) * cos(ct.angle * u),
		//        cos(twopi *v) * sin(ct.angle * u),
		//        std::sin(twopi *v));
		//Vec3f p((ct.radius * cos(twopi *v) + ct.offset) * cos(ct.angle * u),
		//        (ct.radius * cos(twopi *v) + ct.offset) * sin(ct.angle * u),
		//        ct.radius * sin(twopi *v));
		for (uint u = 0; u < samples_l; u++)
		{
		  for (uint v = 0; v < samples_s; v++)
		  {
			tri.normals[l] = t1[2 * v + 0] * t0[2 * u + 0];
			tri.vertices[l++] = ((ct.radius * t1[2 * v + 0] + ct.offset) * t0[2 * u + 0]);
			tri.normals[l] = t1[2 * v + 0] * t0[2 * u + 1];
			tri.vertices[l++] = ((ct.radius * t1[2 * v + 0] + ct.offset) * t0[2 * u + 1]);
			tri.normals[l] = t1[2 * v + 1];
			tri.vertices[l++] = ct.radius * t1[2 * v + 1];
		  }
		}
	  }
	  if (cap[0])
	  {
		for (uint v = 0; v < samples_s; v++)
		{
		  tri.normals[l] = 0.0f;
		  tri.vertices[l++] = ((ct.radius * t1[2 * v + 0] + ct.offset) * t0[0]);
		  tri.normals[l] = -1.0f;
		  tri.vertices[l++] = ((ct.radius * t1[2 * v + 0] + ct.offset) * t0[1]);
		  tri.normals[l] = 0.0f;
		  tri.vertices[l++] = ct.radius * t1[2 * v + 1];
		}
	  }
	  if (cap[1])
	  {
		uint m = (uint)(2 * (samples_l - 1));
		for (uint v = 0; v < samples_s; v++)
		{
		  tri.normals[l] = -t0[m + 1];
		  tri.vertices[l++] = ((ct.radius * t1[2 * v + 0] + ct.offset) * t0[m + 0]);
		  tri.normals[l] = t0[m + 0];
		  tri.vertices[l++] = ((ct.radius * t1[2 * v + 0] + ct.offset) * t0[m + 1]);
		  tri.normals[l] = 0.0f;
		  tri.vertices[l++] = ct.radius * t1[2 * v + 1];
		}
	  }
	  Debug.Assert(l == 3 * tri.vertices_n);
    
	  // generate indices
	  l = 0;
	  uint o = 0;
	  if (shell)
	  {
		for (uint u = 0; u + 1 < samples_l; u++)
		{
		  for (uint v = 0; v + 1 < samples_s; v++)
		  {
			tri.indices[l++] = samples_s * (u + 0) + (v + 0);
			tri.indices[l++] = samples_s * (u + 1) + (v + 0);
			tri.indices[l++] = samples_s * (u + 1) + (v + 1);
    
			tri.indices[l++] = samples_s * (u + 1) + (v + 1);
			tri.indices[l++] = samples_s * (u + 0) + (v + 1);
			tri.indices[l++] = samples_s * (u + 0) + (v + 0);
		  }
		  tri.indices[l++] = samples_s * (u + 0) + (samples_s - 1);
		  tri.indices[l++] = samples_s * (u + 1) + (samples_s - 1);
		  tri.indices[l++] = samples_s * (u + 1) + 0;
		  tri.indices[l++] = samples_s * (u + 1) + 0;
		  tri.indices[l++] = samples_s * (u + 0) + 0;
		  tri.indices[l++] = samples_s * (u + 0) + (samples_s - 1);
		}
		o += samples_l * samples_s;
	  }
    
	  u1.resize(samples_s);
	  u2.resize(samples_s);
	  if (cap[0])
	  {
		for (uint i = 0; i < samples_s; i++)
		{
		  u1[i] = o + i;
		}
		l = tessellateCircle(tri.indices, l, u2.data(), u1.data(), samples_s);
		o += samples_s;
	  }
	  if (cap[1])
	  {
		for (uint i = 0; i < samples_s; i++)
		{
		  u1[i] = o + (samples_s - 1) - i;
		}
		l = tessellateCircle(tri.indices, l, u2.data(), u1.data(), samples_s);
		o += samples_s;
	  }
	  Debug.Assert(l == 3 * tri.triangles_n);
	  Debug.Assert(o == tri.vertices_n);
    
	  return tri;
	}
	public Triangulation snout(Arena arena, Geometry geo, float scale)
	{
	  //if (cullTiny && radius_max*scale < tolerance) {
	  //  tri->error = radius_max *scale;
	  //  return;
	  //}
    
	  var sn = geo.snout;
	  var radius_max = Math.Max(sn.radius_b, sn.radius_t);
	  uint segments = sagittaBasedSegmentCount(twopi, radius_max, scale);
	  uint samples = segments; // assumed to be closed
    
	  var tri = arena.alloc<Triangulation>();
	  tri.error = sagittaBasedError(twopi, radius_max, scale, segments);
    
	  bool shell = true;
	  bool[] cap = {true, true};
	  float[] radii = {geo.snout.radius_b, geo.snout.radius_t};
	  for (uint i = 0; i < 2; i++)
	  {
		var con = geo.connections[i];
		if (con != null && con.flags == Connection.Flags.HasCircularSide)
		{
	//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
	//ORIGINAL LINE: if (doInterfacesMatch(geo, con))
		  if (doInterfacesMatch(geo, new Connection(con)))
		  {
			cap[i] = false;
			discardedCaps++;
		  }
		  else
		  {
			//store->addDebugLine(con->p.data, (con->p.data + 0.05f*con->d).data, 0x00ffff);
		  }
		}
	  }
    
	  t0.resize(2 * samples);
	  for (uint i = 0; i < samples; i++)
	  {
		t0[2 * i + 0] = Math.Cos((twopi / samples) * i + geo.sampleStartAngle);
		t0[2 * i + 1] = Math.Sin((twopi / samples) * i + geo.sampleStartAngle);
	  }
	  t1.resize(2 * samples);
	  for (uint i = 0; i < 2 * samples; i++)
	  {
		t1[i] = sn.radius_b * t0[i];
	  }
	  t2.resize(2 * samples);
	  for (uint i = 0; i < 2 * samples; i++)
	  {
		t2[i] = sn.radius_t * t0[i];
	  }
    
	  float h2 = 0.5f * sn.height;
	  uint l = 0;
	  var ox = 0.5f * sn.offset[0];
	  var oy = 0.5f * sn.offset[1];
	  float[] mb = {Math.Tan(sn.bshear[0]), Math.Tan(sn.bshear[1])};
	  float[] mt = {Math.Tan(sn.tshear[0]), Math.Tan(sn.tshear[1])};
    
	  tri.vertices_n = (uint)((shell ? 2 * samples : 0) + (cap[0] ? samples : 0) + (cap[1] ? samples : 0));
	  tri.vertices = (float)arena.alloc(3 * sizeof(float) * tri.vertices_n);
	  tri.normals = (float)arena.alloc(3 * sizeof(float) * tri.vertices_n);
    
	  tri.triangles_n = (uint)((shell ? 2 * samples : 0) + (cap[0] ? samples - 2 : 0) + (cap[1] ? samples - 2 : 0));
	  tri.indices = (uint)arena.alloc(3 * sizeof(uint) * tri.triangles_n);
    
	  if (shell)
	  {
		for (uint i = 0; i < samples; i++)
		{
		  float xb = t1[2 * i + 0] - ox;
		  float yb = t1[2 * i + 1] - oy;
		  float zb = -h2 + mb[0] * t1[2 * i + 0] + mb[1] * t1[2 * i + 1];
    
		  float xt = t2[2 * i + 0] + ox;
		  float yt = t2[2 * i + 1] + oy;
		  float zt = h2 + mt[0] * t2[2 * i + 0] + mt[1] * t2[2 * i + 1];
    
		  float s = (sn.offset[0] * t0[2 * i + 0] + sn.offset[1] * t0[2 * i + 1]);
		  float nx = t0[2 * i + 0];
		  float ny = t0[2 * i + 1];
		  float nz = -(sn.radius_t - sn.radius_b + s) / sn.height;
    
		  l = vertex(tri.normals, tri.vertices, l, nx, ny, nz, xb, yb, zb);
		  l = vertex(tri.normals, tri.vertices, l, nx, ny, nz, xt, yt, zt);
		}
	  }
	  if (cap[0])
	  {
		var nx = Math.Sin(sn.bshear[0]) * Math.Cos(sn.bshear[1]);
		var ny = Math.Sin(sn.bshear[1]);
		var nz = -Math.Cos(sn.bshear[0]) * Math.Cos(sn.bshear[1]);
		for (uint i = 0; cap[0] && i < samples; i++)
		{
		  l = vertex(tri.normals, tri.vertices, l, nx, ny, nz, t1[2 * i + 0] - ox, t1[2 * i + 1] - oy, -h2 + mb[0] * t1[2 * i + 0] + mb[1] * t1[2 * i + 1]);
		}
	  }
	  if (cap[1])
	  {
		var nx = -Math.Sin(sn.tshear[0]) * Math.Cos(sn.tshear[1]);
		var ny = -Math.Sin(sn.tshear[1]);
		var nz = Math.Cos(sn.tshear[0]) * Math.Cos(sn.tshear[1]);
		for (uint i = 0; i < samples; i++)
		{
		  l = vertex(tri.normals, tri.vertices, l, nx, ny, nz, t2[2 * i + 0] + ox, t2[2 * i + 1] + oy, h2 + mt[0] * t2[2 * i + 0] + mt[1] * t2[2 * i + 1]);
		}
	  }
	  Debug.Assert(l == 3 * tri.vertices_n);
    
	  l = 0;
	  uint o = 0;
	  if (shell)
	  {
		for (uint i = 0; i < samples; i++)
		{
		  uint ii = (i + 1) % samples;
		  l = quadIndices(tri.indices, l, 0, (uint)(2 * i), (uint)(2 * ii), (uint)(2 * ii + 1), (uint)(2 * i + 1));
		}
		o += 2 * samples;
	  }
    
	  u1.resize(samples);
	  u2.resize(samples);
	  if (cap[0])
	  {
		for (uint i = 0; i < samples; i++)
		{
		  u1[i] = o + (samples - 1) - i;
		}
		l = tessellateCircle(tri.indices, l, u2.data(), u1.data(), samples);
		o += samples;
	  }
	  if (cap[1])
	  {
		for (uint i = 0; i < samples; i++)
		{
		  u1[i] = o + i;
		}
		l = tessellateCircle(tri.indices, l, u2.data(), u1.data(), samples);
		o += samples;
	  }
	  Debug.Assert(l == 3 * tri.triangles_n);
	  Debug.Assert(o == tri.vertices_n);
    
	  return tri;
	}
	public Triangulation cylinder(Arena arena, Geometry geo, float scale)
	{
	  //if (cullTiny && cy.radius*scale < tolerance) {
	  //  tri->error = cy.radius * scale;
	  //  return;
	  //}
	  var cy = geo.cylinder;
	  uint segments = sagittaBasedSegmentCount(twopi, cy.radius, scale);
	  uint samples = segments; // Assumed to be closed
    
	  var tri = arena.alloc<Triangulation>();
	  tri.error = sagittaBasedError(twopi, cy.radius, scale, segments);
    
	  bool shell = true;
	  bool[] cap = {true, true};
	  for (uint i = 0; i < 2; i++)
	  {
		var con = geo.connections[i];
		if (con != null && con.flags == Connection.Flags.HasCircularSide)
		{
	//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
	//ORIGINAL LINE: if (doInterfacesMatch(geo, con))
		  if (doInterfacesMatch(geo, new Connection(con)))
		  {
			cap[i] = false;
			discardedCaps++;
		  }
		  else
		  {
			//store->addDebugLine(con->p.data, (con->p.data + 0.05f*con->d).data, 0x00ffff);
		  }
		}
	  }
    
	  tri.vertices_n = (uint)((shell ? 2 * samples : 0) + (cap[0] ? samples : 0) + (cap[1] ? samples : 0));
	  tri.vertices = (float)arena.alloc(3 * sizeof(float) * tri.vertices_n);
	  tri.normals = (float)arena.alloc(3 * sizeof(float) * tri.vertices_n);
    
	  tri.triangles_n = (uint)((shell ? 2 * samples : 0) + (cap[0] ? samples - 2 : 0) + (cap[1] ? samples - 2 : 0));
	  tri.indices = (uint)arena.alloc(3 * sizeof(uint) * tri.triangles_n);
    
	  t0.resize(2 * samples);
	  for (uint i = 0; i < samples; i++)
	  {
		t0[2 * i + 0] = Math.Cos((twopi / samples) * i + geo.sampleStartAngle);
		t0[2 * i + 1] = Math.Sin((twopi / samples) * i + geo.sampleStartAngle);
	  }
	  t1.resize(2 * samples);
	  for (uint i = 0; i < 2 * samples; i++)
	  {
		t1[i] = cy.radius * t0[i];
	  }
    
	  float h2 = 0.5f * cy.height;
	  uint l = 0;
    
	  if (shell)
	  {
		for (uint i = 0; i < samples; i++)
		{
		  l = vertex(tri.normals, tri.vertices, l, t0[2 * i + 0], t0[2 * i + 1], 0F, t1[2 * i + 0], t1[2 * i + 1], -h2);
		  l = vertex(tri.normals, tri.vertices, l, t0[2 * i + 0], t0[2 * i + 1], 0F, t1[2 * i + 0], t1[2 * i + 1], h2);
		}
	  }
	  if (cap[0])
	  {
		for (uint i = 0; cap[0] && i < samples; i++)
		{
		  l = vertex(tri.normals, tri.vertices, l, 0F, 0F, -1F, t1[2 * i + 0], t1[2 * i + 1], -h2);
		}
	  }
	  if (cap[1])
	  {
		for (uint i = 0; i < samples; i++)
		{
		  l = vertex(tri.normals, tri.vertices, l, 0F, 0F, 1F, t1[2 * i + 0], t1[2 * i + 1], h2);
		}
	  }
	  Debug.Assert(l == 3 * tri.vertices_n);
    
	  l = 0;
	  uint o = 0;
	  if (shell)
	  {
		for (uint i = 0; i < samples; i++)
		{
		  uint ii = (i + 1) % samples;
		  l = quadIndices(tri.indices, l, 0, (uint)(2 * i), (uint)(2 * ii), (uint)(2 * ii + 1), (uint)(2 * i + 1));
		}
		o += 2 * samples;
	  }
	  u1.resize(samples);
	  u2.resize(samples);
	  if (cap[0])
	  {
		for (uint i = 0; i < samples; i++)
		{
		  u1[i] = o + (samples - 1) - i;
		}
		l = tessellateCircle(tri.indices, l, u2.data(), u1.data(), samples);
		o += samples;
	  }
	  if (cap[1])
	  {
		for (uint i = 0; i < samples; i++)
		{
		  u1[i] = o + i;
		}
		l = tessellateCircle(tri.indices, l, u2.data(), u1.data(), samples);
		o += samples;
	  }
	  Debug.Assert(l == 3 * tri.triangles_n);
	  Debug.Assert(o == tri.vertices_n);
    
	  return tri;
	}
	public Triangulation sphereBasedShape(Arena arena, Geometry geo, float radius, float arc, float shift_z, float scale_z, float scale)
	{
	  uint segments = sagittaBasedSegmentCount(twopi, radius, scale);
	  uint samples = segments; // Assumed to be closed
    
	  var tri = arena.alloc<Triangulation>();
	  tri.error = sagittaBasedError(twopi, radius, scale, samples);
    
	  bool is_sphere = false;
	  if (pi - 1e-3 <= arc)
	  {
		arc = pi;
		is_sphere = true;
	  }
    
	  uint min_rings = 3; // arc <= half_pi ? 2 : 3;
	  uint rings = (uint)Math.Max((float)min_rings, scale_z * samples * arc * (1.0f / twopi));
    
	  u0.resize(rings);
	  t0.resize(2 * rings);
	  var theta_scale = arc / (rings - 1);
	  for (uint r = 0; r < rings; r++)
	  {
		float theta = theta_scale * r;
		t0[2 * r + 0] = Math.Cos(theta);
		t0[2 * r + 1] = Math.Sin(theta);
		u0[r] = (uint)Math.Max(3.0f, t0[2 * r + 1] * samples); // samples in this ring
	  }
	  u0[0] = 1;
	  if (is_sphere)
	  {
		u0[rings - 1] = 1;
	  }
    
	  uint s = 0;
	  for (uint r = 0; r < rings; r++)
	  {
		s += u0[r];
	  }
    
    
	  tri.vertices_n = s;
	  tri.vertices = (float)arena.alloc(3 * sizeof(float) * tri.vertices_n);
	  tri.normals = (float)arena.alloc(3 * sizeof(float) * tri.vertices_n);
    
	  uint l = 0;
	  for (uint r = 0; r < rings; r++)
	  {
		var nz = t0[2 * r + 0];
		var z = radius * scale_z * nz + shift_z;
		var w = t0[2 * r + 1];
		var n = u0[r];
    
		var phi_scale = twopi / n;
		for (uint i = 0; i < n; i++)
		{
		  var phi = phi_scale * i + geo.sampleStartAngle;
		  var nx = w * Math.Cos(phi);
		  var ny = w * Math.Sin(phi);
		  l = vertex(tri.normals, tri.vertices, l, nx, ny, nz / scale_z, radius * nx, radius * ny, z);
		}
	  }
	  Debug.Assert(l == 3 * tri.vertices_n);
    
	  uint o_c = 0;
	  indices.clear();
	  for (uint r = 0; r + 1 < rings; r++)
	  {
		var n_c = u0[r];
		var n_n = u0[r + 1];
		var o_n = o_c + n_c;
    
		if (n_c < n_n)
		{
		  for (uint i_n = 0; i_n < n_n; i_n++)
		  {
			uint ii_n = (i_n + 1);
			uint i_c = (n_c * (i_n + 1)) / n_n;
			uint ii_c = (n_c * (ii_n + 1)) / n_n;
    
			i_c %= n_c;
			ii_c %= n_c;
			ii_n %= n_n;
    
			if (i_c != ii_c)
			{
			  indices.push_back(o_c + i_c);
			  indices.push_back(o_n + ii_n);
			  indices.push_back(o_c + ii_c);
			}
			Debug.Assert(i_n != ii_n);
			indices.push_back(o_c + i_c);
			indices.push_back(o_n + i_n);
			indices.push_back(o_n + ii_n);
		  }
		}
		else
		{
		  for (uint i_c = 0; i_c < n_c; i_c++)
		  {
			var ii_c = (i_c + 1);
			uint i_n = (n_n * (i_c + 0)) / n_c;
			uint ii_n = (n_n * (ii_c + 0)) / n_c;
    
			i_n %= n_n;
			ii_n %= n_n;
			ii_c %= n_c;
    
			Debug.Assert(i_c != ii_c);
			indices.push_back(o_c + i_c);
			indices.push_back(o_n + ii_n);
			indices.push_back(o_c + ii_c);
    
			if (i_n != ii_n)
			{
			  indices.push_back(o_c + i_c);
			  indices.push_back(o_n + i_n);
			  indices.push_back(o_n + ii_n);
			}
		  }
		}
		o_c = o_n;
	  }
    
	  tri.triangles_n = (uint)(indices.size() / 3);
	  tri.indices = (uint)arena.dup(indices.data(), 3 * sizeof(uint) * tri.triangles_n);
    
	  return tri;
	}
	public Triangulation facetGroup(Arena arena, Geometry geo, float scale)
	{
	  var fg = geo.facetGroup;
    
	  vertices.clear();
	  normals.clear();
	  indices.clear();
	  for (uint p = 0; p < fg.polygons_n; p++)
	  {
		var poly = fg.polygons[p];
    
		if (poly.contours_n == 1 && poly.contours[0].vertices_n == 3)
		{
		  var cont = poly.contours[0];
		  var vo = (uint)vertices.size() / 3;
    
		  vertices.resize(vertices.size() + 3 * 3);
		  normals.resize(vertices.size());
    
	//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		  memcpy(vertices.data() + 3 * vo, cont.vertices, 3 * 3 * sizeof(float));
	//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		  memcpy(normals.data() + 3 * vo, cont.normals, 3 * 3 * sizeof(float));
    
		  indices.push_back(vo + 0);
		  indices.push_back(vo + 1);
		  indices.push_back(vo + 2);
		}
		else if (poly.contours_n == 1 && poly.contours[0].vertices_n == 4)
		{
		  var cont = poly.contours[0];
		  var V = cont.vertices;
		  var vo = (uint)vertices.size() / 3;
    
		  vertices.resize(vertices.size() + 3 * 4);
		  normals.resize(vertices.size());
    
	//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		  memcpy(vertices.data() + 3 * vo, cont.vertices, 3 * 4 * sizeof(float));
	//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		  memcpy(normals.data() + 3 * vo, cont.normals, 3 * 4 * sizeof(float));
    
		  // find least folding diagonal
    
		  float[] v01 = new float[3];
		  float[] v12 = new float[3];
		  float[] v23 = new float[3];
		  float[] v30 = new float[3];
		  sub3(v01, V + 3 * 1, V + 3 * 0);
		  sub3(v12, V + 3 * 2, V + 3 * 1);
		  sub3(v23, V + 3 * 3, V + 3 * 2);
		  sub3(v30, V + 3 * 0, V + 3 * 3);
    
		  float[] n0 = new float[3];
		  float[] n1 = new float[3];
		  float[] n2 = new float[3];
		  float[] n3 = new float[3];
		  cross3(n0, v01, v30);
		  cross3(n1, v12, v01);
		  cross3(n2, v23, v12);
		  cross3(n3, v30, v23);
    
		  if (dot3(n0, n2) < dot3(n1, n3))
		  {
			indices.push_back(vo + 0);
			indices.push_back(vo + 1);
			indices.push_back(vo + 2);
    
			indices.push_back(vo + 2);
			indices.push_back(vo + 3);
			indices.push_back(vo + 0);
		  }
		  else
		  {
			indices.push_back(vo + 3);
			indices.push_back(vo + 0);
			indices.push_back(vo + 1);
    
			indices.push_back(vo + 1);
			indices.push_back(vo + 2);
			indices.push_back(vo + 3);
		  }
		}
		else
		{
    
		  bool anyData = false;
    
		  BBox3f bbox = createEmptyBBox3f();
		  for (uint c = 0; c < poly.contours_n; c++)
		  {
			for (uint i = 0; i < poly.contours[c].vertices_n; i++)
			{
			  var p = new Vec3f(poly.contours[c].vertices + 3 * i);
			  engulf(bbox, p);
			}
		  }
		  var m = 0.5f * (new Vec3f(bbox.min) + new Vec3f(bbox.max));
    
		  var tess = tessNewTess(null);
		  for (uint c = 0; c < poly.contours_n; c++)
		  {
			var cont = poly.contours[c];
			if (cont.vertices_n < 3)
			{
			  logger(1, "Ignoring degenerate contour with %d vertices.", cont.vertices_n);
			  continue;
			}
			vec3.resize(cont.vertices_n);
			for (uint i = 0; i < cont.vertices_n; i++)
			{
			  vec3[i] = new Vec3f(cont.vertices + 3 * i) - m;
			}
			tessAddContour(new TESStesselator(tess), 3, vec3.data(), 3 * sizeof(float), cont.vertices_n);
			//tessAddContour(tess, 3, cont.vertices, 3 * sizeof(float), cont.vertices_n);
			anyData = true;
		  }
    
		  if (anyData == false)
		  {
			logger(1, "Ignoring polygon with no valid contours.");
		  }
		  else
		  {
			if (tessTesselate(new TESStesselator(tess), (int)TessWindingRule.TESS_WINDING_ODD, (int)TessElementType.TESS_POLYGONS, 3, 3, null) != 0)
			{
			  var vo = (uint)vertices.size() / 3;
			  var vn = (uint)tessGetVertexCount(new TESStesselator(tess));
    
			  vertices.resize(vertices.size() + 3 * vn);
    
			  var src = tessGetVertices(new TESStesselator(tess));
			  for (uint i = 0; i < vn; i++)
			  {
				var p = new Vec3f((float)(src + 3 * i)) + m;
				write(vertices.data() + 3 * (vo + i), p);
			  }
    
			  //std::memcpy(vertices.data() + 3 * vo, tessGetVertices(tess), 3 * vn * sizeof(float));
    
			  var[] remap = tessGetVertexIndices(new TESStesselator(tess));
			  normals.resize(vertices.size());
			  for (uint i = 0; i < vn; i++)
			  {
				if (remap[i] != (~(int)0))
				{
				  uint ix = remap[i];
				  for (uint c = 0; c < poly.contours_n; c++)
				  {
					var cont = poly.contours[c];
					if (ix < cont.vertices_n)
					{
					  normals[3 * (vo + i) + 0] = cont.normals[3 * ix + 0];
					  normals[3 * (vo + i) + 1] = cont.normals[3 * ix + 1];
					  normals[3 * (vo + i) + 2] = cont.normals[3 * ix + 2];
					  break;
					}
					ix -= cont.vertices_n;
				  }
				}
			  }
    
			  var io = (uint)indices.size();
			  var elements = tessGetElements(new TESStesselator(tess));
			  var elements_n = (uint)tessGetElementCount(new TESStesselator(tess));
			  for (uint e = 0; e < elements_n; e++)
			  {
				var ix = elements + 3 * e;
				if ((ix[0] != (~(int)0)) && (ix[1] != (~(int)0)) && (ix[2] != (~(int)0)))
				{
				  indices.push_back(ix[0] + vo);
				  indices.push_back(ix[1] + vo);
				  indices.push_back(ix[2] + vo);
				}
			  }
			}
		  }
    
		  tessDeleteTess(new TESStesselator(tess));
		}
	  }
    
	  Debug.Assert(vertices.size() == normals.size());
    
	  Triangulation tri = arena.alloc<Triangulation>();
	  tri.error = 0.0f;
    
	  if (!indices.empty())
	  {
		tri.vertices_n = (uint)(vertices.size() / 3);
		tri.triangles_n = (uint)(indices.size() / 3);
    
		tri.vertices = (float)arena.dup(vertices.data(), sizeof(float) * 3 * tri.vertices_n);
		tri.normals = (float)arena.dup(normals.data(), sizeof(float) * 3 * tri.vertices_n);
		tri.indices = (uint)arena.dup(indices.data(), sizeof(uint) * 3 * tri.triangles_n);
	  }
    
	  return tri;
	}
}