using System;
using System.Collections.Generic;
using System.Diagnostics;



public class TriangulationFactory
{
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//  TriangulationFactory(Store store, Logger logger, float tolerance, uint minSamples, uint maxSamples);

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//  uint sagittaBasedSegmentCount(float arc, float radius, float scale);

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//  float sagittaBasedError(float arc, float radius, float scale, uint samples);

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//  Triangulation pyramid(Arena arena, Geometry geo, float scale);

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//  Triangulation box(Arena arena, Geometry geo, float scale);

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//  Triangulation rectangularTorus(Arena arena, Geometry geo, float scale);

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//  Triangulation circularTorus(Arena arena, Geometry geo, float scale);

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//  Triangulation snout(Arena arena, Geometry geo, float scale);

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//  Triangulation cylinder(Arena arena, Geometry geo, float scale);

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//  Triangulation facetGroup(Arena arena, Geometry geo, float scale);

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//  Triangulation sphereBasedShape(Arena arena, Geometry geo, float radius, float arc, float shift_z, float scale_z, float scale);

  public uint discardedCaps = 0;

  private Store store;
  private Logger logger;
  private float tolerance = 0.0f / 0.0f;
  private uint minSamples = 3;
  private uint maxSamples = 100;

  private List<float> vertices = new List<float>();
  private List<Vec3f> vec3 = new List<Vec3f>();
  private List<float> normals = new List<float>();
  private List<uint> indices = new List<uint>();

  private List<uint> u0 = new List<uint>();
  private List<uint> u1 = new List<uint>();
  private List<uint> u2 = new List<uint>();
  private List<float> t0 = new List<float>();
  private List<float> t1 = new List<float>();
  private List<float> t2 = new List<float>();

}

public partial class Tessellator : StoreVisitor, System.IDisposable
{
//C++ TO C# CONVERTER TODO TASK: C# has no equivalent to ' = delete':
//  Tessellator() = delete;
//C++ TO C# CONVERTER TODO TASK: C# has no equivalent to ' = delete':
//  Tessellator(const Tessellator&) = delete;
  public Tessellator(Logger logger, float tolerance, float cullLeafThreshold, float cullGeometryThreshold, uint maxSamples)
  {
	  this.logger = logger;
	  this.tolerance = tolerance;
	  this.maxSamples = maxSamples;
	  this.cullLeafThresholdScaled = tolerance * cullLeafThreshold;
	  this.cullGeometryThresholdScaled = tolerance * cullGeometryThreshold;
  }

//C++ TO C# CONVERTER TODO TASK: C# has no equivalent to ' = delete':
//  Tessellator& operator =(const Tessellator&) = delete;

  public void Dispose()
  {
	if (factory != null)
	{
		factory.Dispose();
	}
  }

  public override void init(Store store)
  {
	this.store = store;

	factory = new TriangulationFactory(store, logger, tolerance, 3, maxSamples), store.arenaTriangulation.clear();

	cache.items = (CacheItem)arena.alloc(sizeof(CacheItem) * store.geometryCountAllocated());
	cache.fill = 0;

	stack = (StackItem)arena.alloc(sizeof(StackItem) * store.groupCountAllocated());
	stack_p = 0;
  }

  public override void beginGroup(Group group)
  {
	StackItem item = new StackItem();
	if (!GlobalMembers.isEmpty(group.group.bboxWorld))
	{
	  item.groupError = GlobalMembers.diagonal(group.group.bboxWorld);
	}
	stack[stack_p++] = item;
  }

  public override void EndGroup()
  {
	Debug.Assert(stack_p);
	stack_p--;
  }

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//  override void geometry(Geometry geometry);

  public override void endModel()
  {
	logger(0, "Discarded %u caps.", factory.discardedCaps);
  }


  public uint leafCulled = 0;
  public uint geometryCulled = 0;
  public uint tessellated = 0;
  public uint processed = 0;

  public ulong vertices = 0;
  public ulong triangles = 0;

  protected class CacheItem
  {
	public CacheItem next;
	public Geometry[] src;
	public Triangulation tri;
  }

  protected class StackItem
  {
	public float groupError; // Error induced if group is omitted.
  }

  protected float tolerance = 0.0f;
  protected uint maxSamples = 100;
  protected float cullLeafThresholdScaled = 0.0f / 0.0f;
  protected float cullGeometryThresholdScaled = 0.0f / 0.0f;
  protected Arena arena = new Arena();
  protected TriangulationFactory factory = null;
  protected Logger logger;

  protected Store store = null;

//C++ TO C# CONVERTER NOTE: Classes must be named in C#, so the following class has been named by the converter:
  protected class AnonymousClass
  {
	public Map map = new Map();
	public CacheItem[] items;
	public uint fill;
  }
  protected AnonymousClass cache = new AnonymousClass();

  protected StackItem[] stack = null;
  protected uint stack_p = 0;

  protected Triangulation getTriangulation(Geometry geo)
  {
	var a = offsetof(Geometry, kind);
	var n = sizeof(Geometry) - a;

	var hash = fnv_1a((string)geo + a, n);
	if (hash == 0)
	{
		hash = 1;
	}

	var firstItem = (CacheItem)cache.map.get(hash);
	for (var * item = firstItem; item != null; item = item.next)
	{
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcmp' has no equivalent in C#:
	  if (memcmp((string)geo + a, (string)item.src + a, n) == 0)
	  {
		return item.tri;
	  }
	}

	// FIXME: create tri.

	var item = cache.items[cache.fill++];
	item.next = firstItem;
	item.src = geo;
	item.tri = null;
	cache.map.insert(hash, (ulong)item);
	return item.tri;
  }

  protected virtual void process(Geometry geometry)
  {
  }
}


//C++ TO C# CONVERTER NOTE: C# does not allow anonymous namespaces:
//namespace


//C++ TO C# CONVERTER WARNING: The original C++ declaration of the following method implementation was not found:
//ORIGINAL LINE: void Tessellator::geometry(Geometry* geo)


public partial class Tessellator
{
	public void geometry(Geometry geo)
	{
	  Debug.Assert(stack_p);
    
	  // No need to tessellate lines.
	  if (geo.kind == Geometry.Kind.Line)
	  {
		geo.triangulation = null;
		return;
	  }
	  processed++;
    
	  var scale = getScale(geo.M_3x4);
    
	  // Group error less than threshold, skip tessellation and record error.
	  if (stack[stack_p - 1].groupError < cullLeafThresholdScaled)
	  {
		geo.triangulation = store.arenaTriangulation.alloc<Triangulation>();
		geo.triangulation.error = stack[stack_p - 1].groupError;
		return;
	  }
	  else
	  {
		var scaledDiagonal = diagonal(geo.bboxWorld);
		if (scaledDiagonal < cullGeometryThresholdScaled)
		{
		  geo.triangulation = store.arenaTriangulation.alloc<Triangulation>();
		  geo.triangulation.error = scaledDiagonal;
		  geometryCulled++;
		  return;
		}
	  }
    
    
	  Triangulation tri = null;
	  switch (geo.kind)
	  {
	  case Geometry.Kind.Pyramid:
		tri = factory.pyramid(store.arenaTriangulation, geo, scale);
		break;
    
	  case Geometry.Kind.Box:
		tri = factory.box(store.arenaTriangulation, geo, scale);
		break;
    
	  case Geometry.Kind.RectangularTorus:
		tri = factory.rectangularTorus(store.arenaTriangulation, geo, scale);
		break;
    
	  case Geometry.Kind.CircularTorus:
		tri = factory.circularTorus(store.arenaTriangulation, geo, scale);
		break;
    
	  case Geometry.Kind.EllipticalDish:
		tri = factory.sphereBasedShape(store.arenaTriangulation, geo, geo.ellipticalDish.baseRadius, half_pi, 0.0f, geo.ellipticalDish.height / geo.ellipticalDish.baseRadius, scale);
		break;
    
	  case Geometry.Kind.SphericalDish:
	  {
		float r_circ = geo.sphericalDish.baseRadius;
		var h = geo.sphericalDish.height;
		float r_sphere = (r_circ * r_circ + h * h) / (2.0f * h);
		float sinval = Math.Min(1.0f, Math.Max(-1.0f, r_circ / r_sphere));
		float arc = Math.Asin(sinval);
		if (r_circ < h)
		{
			arc = pi - arc;
		}
		tri = factory.sphereBasedShape(store.arenaTriangulation, geo, r_sphere, arc, h - r_sphere, 1.0f, scale);
		break;
	  }
	  case Geometry.Kind.Snout:
		tri = factory.snout(store.arenaTriangulation, geo, scale);
		break;
    
	  case Geometry.Kind.Cylinder:
		tri = factory.cylinder(store.arenaTriangulation, geo, scale);
		break;
    
	  case Geometry.Kind.Sphere:
		tri = factory.sphereBasedShape(store.arenaTriangulation, geo, 0.5f * geo.sphere.diameter, pi, 0.0f, 1.0f, scale);
		break;
    
	  case Geometry.Kind.FacetGroup:
		tri = factory.facetGroup(store.arenaTriangulation, geo, scale);
		break;
    
	  case Geometry.Kind.Line: // Handled at start of function.
	  default:
		Debug.Assert(false && "Unhandled primitive type");
		break;
	  }
    
	  geo.triangulation = tri;
	  vertices += (ulong)tri.vertices_n;
	  triangles += (ulong)tri.triangles_n;
    
	  BBox3f box = createEmptyBBox3f();
	  for (uint i = 0; i < geo.triangulation.vertices_n; i++)
	  {
		engulf(box, new Vec3f(geo.triangulation.vertices + 3 * i));
	  }
	  //assert(geo->bbox_l.min[0] < box.min[0] + 0.1f*box.maxSideLength());
	  //assert(geo->bbox_l.min[1] < box.min[1] + 0.1f*box.maxSideLength());
	  //assert(geo->bbox_l.min[2] < box.min[2] + 0.1f*box.maxSideLength());
    
	  //assert(box.max[0] - 0.1f*box.maxSideLength() < geo->bbox_l.max[0]);
	  //assert(box.max[1] - 0.1f*box.maxSideLength() < geo->bbox_l.max[1]);
	  //assert(box.max[2] - 0.1f*box.maxSideLength() < geo->bbox_l.max[2]);
    
	  geo.triangulation.id = (int)geo.id;
	  process(geo);
    
	  tessellated++;
	}
}