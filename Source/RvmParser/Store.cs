using System.Diagnostics;

//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//struct Group;
//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//struct Geometry;

public class Contour
{
  public float[] vertices;
  public float[] normals;
  public uint vertices_n;
}

public class Polygon
{
  public Contour[] contours;
  public uint contours_n;
}

public class Triangulation
{
  public float[] vertices = 0;
  public float[] normals = 0;
//C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent to pointers to value types:
//ORIGINAL LINE: float* texCoords = null;
  public float texCoords = null;
  public uint[] indices = 0;
  public uint vertices_n = 0;
  public uint triangles_n = 0;
  public int id = 0;
  public float error = 0.0f;
}

public class Color
{
  public Color next = null;
  public uint colorKind;
  public uint colorIndex;
  public byte[] rgb = new byte[3];
}

public class Connection
{
  public enum Flags : byte
  {
	None = 0,
	HasCircularSide = 1 << 0,
	HasRectangularSide = 1 << 1
  }

  public Connection next = null;
  public Geometry[] geo = {null, null};
  public uint[] offset = new uint[2];
  public Vec3f p = new Vec3f();
  public Vec3f d = new Vec3f();
  public uint temp;
  public Flags flags = Flags.None;

  public void setFlag(Flags flag)
  {
	  flags = (Flags)((byte)flags | (byte)flag);
  }
  public bool hasFlag(Flags flag)
  {
	  return ((byte)flags & (byte)flag) != 0;
  }

}


public class Geometry
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
  public Geometry next = null; // Next geometry in the list of geometries in group.
  public Triangulation triangulation = null;
  public Geometry next_comp = null; // Next geometry in list of geometries of this composite

  public Connection[] connections = {null, null, null, null, null, null};
  public string colorName = null;
  public object clientData = null;
  public uint color = 0x202020u;

  public Kind kind;
  public uint id;

  public Mat3x4f M_3x4 = new Mat3x4f();
  public BBox3f bboxLocal = new BBox3f();
  public BBox3f bboxWorld = new BBox3f();
  public float sampleStartAngle = 0.0f;
//C++ TO C# CONVERTER TODO TASK: Unions are not supported in C#:
//  union
//  {
//	struct
//	{
//	  float bottom[2];
//	  float top[2];
//	  float offset[2];
//	  float height;
//	}
//	pyramid;
//	struct
//	{
//	  float lengths[3];
//	}
//	box;
//	struct
//	{
//	  float inner_radius;
//	  float outer_radius;
//	  float height;
//	  float angle;
//	}
//	rectangularTorus;
//	struct
//	{
//	  float offset;
//	  float radius;
//	  float angle;
//	}
//	circularTorus;
//	struct
//	{
//	  float baseRadius;
//	  float height;
//	}
//	ellipticalDish;
//	struct
//	{
//	  float baseRadius;
//	  float height;
//	}
//	sphericalDish;
//	struct
//	{
//	  float offset[2];
//	  float bshear[2];
//	  float tshear[2];
//	  float radius_b;
//	  float radius_t;
//	  float height;
//	}
//	snout;
//	struct
//	{
//	  float radius;
//	  float height;
//	}
//	cylinder;
//	struct
//	{
//	  float diameter;
//	}
//	sphere;
//	struct
//	{
//	  float a, b;
//	}
//	line;
//	struct
//	{
//	  struct Polygon* polygons;
//	  uint polygons_n;
//	}
//	facetGroup;
//  };
}

//C++ TO C# CONVERTER WARNING: The original C++ template specifier was replaced with a C# generic specifier, which may not produce the same behavior:
//ORIGINAL LINE: template<typename T>
public class ListHeader <T>
{
  public T first;
  public T last;

  public void clear()
  {
	first = last = null;
  }

  public void insert(T item)
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
}

public class Attribute
{
  public Attribute next = null;
  public string key = null;
  public string val = null;
}


public class Group
{
  public Group()
  {
  }

  public enum Kind
  {
	File,
	Model,
	Group
  }

  public enum Flags
  {
	None = 0,
	ClientFlagStart = 1
  }

  public Group next = null;
  public ListHeader<Group> groups = new ListHeader<Group>();
  public ListHeader<Attribute> attributes = new ListHeader<Attribute>();

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
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool hasFlag(Flags flag) const
  public bool hasFlag(Flags flag)
  {
	  return ((uint)flags & (uint)flag) != 0;
  }

//C++ TO C# CONVERTER TODO TASK: Unions are not supported in C#:
//  union
//  {
//	struct
//	{
//	  const char* info;
//	  const char* note;
//	  const char* date;
//	  const char* user;
//	  const char* encoding;
//	}
//	file;
//	struct
//	{
//	  ListHeader<Color> colors;
//	  const char* project;
//	  const char* name;
//	}
//	model;
//	struct
//	{
//	  ListHeader<Geometry> geometries;
//	  const char* name;
//	  BBox3f bboxWorld;
//	  uint material;
//	  int id = 0;
//	  float translation[3];
//	  uint clientTag; // For use by passes to stuff temporary info
//	}
//	group;
//  };

}


public class DebugLine
{
  public DebugLine next = null;
  public float[] a = new float[3];
  public float[] b = new float[3];
  public uint color = 0xff0000u;
}


//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class StoreVisitor;

public class Store
{
  public Store()
  {
	roots.clear();
	debugLines.clear();
	connections.clear();
	setErrorString("");
  }

  public Color newColor(Group parent)
  {
	Debug.Assert(parent != null);
	Debug.Assert(parent.kind == Group.Kind.Model);
	auto color = arena.alloc<Color>();
	color.next = null;
	GlobalMembers.insert(parent.model.colors, color);
	return color;
  }

  public Geometry newGeometry(Group parent)
  {
	Debug.Assert(parent != null);
	Debug.Assert(parent.kind == Group.Kind.Group);

	var geo = arena.alloc<Geometry>();
	geo.next = null;
	geo.id = numGeometriesAllocated++;

	GlobalMembers.insert(parent.group.geometries, geo);
	return geo;
  }

  public Geometry cloneGeometry(Group parent, Geometry src)
  {
	var dst = newGeometry(parent);
	dst.kind = src.kind;
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: dst->M_3x4 = src->M_3x4;
	dst.M_3x4.CopyFrom(src.M_3x4);
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: dst->bboxLocal = src->bboxLocal;
	dst.bboxLocal.CopyFrom(src.bboxLocal);
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: dst->bboxWorld = src->bboxWorld;
	dst.bboxWorld.CopyFrom(src.bboxWorld);
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
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		memcpy(dst.snout, src.snout, sizeof(src.snout));
		break;
	  case Geometry.Kind.FacetGroup:
		dst.facetGroup.polygons_n = src.facetGroup.polygons_n;
		dst.facetGroup.polygons = (Polygon)arena.alloc(sizeof(Polygon) * dst.facetGroup.polygons_n);
		for (uint k = 0; k < dst.facetGroup.polygons_n; k++)
		{
		  var dst_poly = dst.facetGroup.polygons[k];
		  var src_poly = src.facetGroup.polygons[k];
		  dst_poly.contours_n = src_poly.contours_n;
		  dst_poly.contours = (Contour)arena.alloc(sizeof(Contour) * dst_poly.contours_n);
		  for (uint i = 0; i < dst_poly.contours_n; i++)
		  {
			var dst_cont = dst_poly.contours[i];
			var src_cont = src_poly.contours[i];
			dst_cont.vertices_n = src_cont.vertices_n;
			dst_cont.vertices = (float)arena.dup(src_cont.vertices, 3 * sizeof(float) * dst_cont.vertices_n);
			dst_cont.normals = (float)arena.dup(src_cont.normals, 3 * sizeof(float) * dst_cont.vertices_n);
		  }
		}
		break;
	  default:
		Debug.Assert(false && "Geometry has invalid kind.");
		break;
	}

	if (src.colorName)
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
		dtri.vertices = (float)arena.dup(stri.vertices, 3 * sizeof(float) * dtri.vertices_n);
		dtri.normals = (float)arena.dup(stri.normals, 3 * sizeof(float) * dtri.vertices_n);
		dtri.texCoords = (float)arena.dup(stri.texCoords, 2 * sizeof(float) * dtri.vertices_n);
	  }
	  if (stri.triangles_n != 0)
	  {
		dtri.triangles_n = stri.triangles_n;
		dtri.indices = (uint)arena.dup(stri.indices, 3 * sizeof(uint) * dtri.triangles_n);
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
	var grp = arena.alloc<Group>();
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memset' has no equivalent in C#:
	memset(grp, 0, sizeof(Group));

	if (parent == null)
	{
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: insert(roots, grp);
	  GlobalMembers.insert(roots, new Group(grp));
	}
	else
	{
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: insert(parent->groups, grp);
	  GlobalMembers.insert(parent.groups, new Group(grp));
	}

	grp.kind = kind;
	numGroupsAllocated++;
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: return grp;
	return new Group(grp);
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
	  for (uint k = 0; k < 3; k++)
	  {
		  dst.group.translation[k] = src.group.translation[k];
	  }
	  break;
	default:
	  Debug.Assert(false && "Group has invalid kind.");
	  break;
	}

	for (var * src_att = src.attributes.first; src_att != null; src_att = src_att.next)
	{
	  var dst_att = newAttribute(dst, strings.intern(src_att.key));
	  dst_att.val = strings.intern(src_att.val);
	}

	return dst;
  }

  public Group findRootGroup(string name)
  {
	for (var * file = roots.first; file != null; file = file.next)
	{
	  Debug.Assert(file.kind == Group.Kind.File);
	  for (var * model = file.groups.first; model != null; model = model.next)
	  {
		//fprintf(stderr, "model '%s'\n", model->model.name);
		Debug.Assert(model.kind == Group.Kind.Model);
		for (var * group = model.groups.first; group != null; group = group.next)
		{
		  //fprintf(stderr, "group '%s' %p\n", group->group.name, (void*)group->group.name);
		  if (group.group.name == name)
		  {
			  return group;
		  }
		}
	  }
	}
	return null;
  }

  public Attribute getAttribute(Group group, string key)
  {
	for (var * attribute = group.attributes.first; attribute != null; attribute = attribute.next)
	{
	  if (attribute.key == key)
	  {
		  return attribute;
	  }
	}
	return null;
  }

  public Attribute newAttribute(Group group, string key)
  {
	var attribute = arena.alloc<Attribute>();
	attribute.key = key;
	GlobalMembers.insert(group.attributes, attribute);
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
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: insert(debugLines, line);
	GlobalMembers.insert(debugLines, new DebugLine(line));
  }

  public Connection newConnection()
  {
	var connection = arena.alloc<Connection>();
	GlobalMembers.insert(connections, connection);
	return connection;
  }

  public void apply(StoreVisitor visitor)
  {
	visitor.init(this);
	do
	{
	  for (var * file = roots.first; file != null; file = file.next)
	  {
		Debug.Assert(file.kind == Group.Kind.File);
		visitor.beginFile(file);

		for (var * model = file.groups.first; model != null; model = model.next)
		{
		  Debug.Assert(model.kind == Group.Kind.Model);
		  visitor.beginModel(model);

		  for (var * group = model.groups.first; group != null; group = group.next)
		  {
			apply(visitor, group);
		  }
		  visitor.endModel();
		}

		visitor.endFile();
	  }
	} while (visitor.done() == false);
  }

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: uint groupCount_() const
  public uint groupCount_()
  {
	  return numGroups;
  }
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: uint groupCountAllocated() const
  public uint groupCountAllocated()
  {
	  return numGroupsAllocated;
  }
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: uint leafCount() const
  public uint leafCount()
  {
	  return numLeaves;
  }
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: uint emptyLeafCount() const
  public uint emptyLeafCount()
  {
	  return numEmptyLeaves;
  }
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: uint nonEmptyNonLeafCount() const
  public uint nonEmptyNonLeafCount()
  {
	  return numNonEmptyNonLeaves;
  }
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: uint geometryCount_() const
  public uint geometryCount_()
  {
	  return numGeometries;
  }
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: uint geometryCountAllocated() const
  public uint geometryCountAllocated()
  {
	  return numGeometriesAllocated;
  }

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: const char* errorString() const
  public string errorString()
  {
	  return error_str;
  }
  public void setErrorString(string str)
  {
	var l = str.Length;
	error_str = strings.intern(str, str + l);
  }

  public Group getFirstRoot()
  {
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: return roots.first;
	  return new Group(roots.first);
  }
  public Connection getFirstConnection()
  {
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: return connections.first;
	  return new Connection(connections.first);
  }
  public DebugLine getFirstDebugLine()
  {
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: return debugLines.first;
	  return new DebugLine(debugLines.first);
  }

  public Arena arena = new Arena();
  public Arena arenaTriangulation = new Arena();
  public Stats stats = null;
  public Connectivity conn = null;

  public StringInterning strings = new StringInterning();

  public void updateCounts()
  {
	numGroups = 0;
	numLeaves = 0;
	numEmptyLeaves = 0;
	numNonEmptyNonLeaves = 0;
	numGeometries = 0;
	for (var * root = roots.first; root != null; root = root.next)
	{
	  updateCountsRecurse(root);
	}

  }

  public void forwardGroupIdToGeometries()
  {
	for (var * root = roots.first; root != null; root = root.next)
	{
	  GlobalMembers.storeGroupIndexInGeometriesRecurse(root);
	}
  }

  private uint numGroups = 0;
  private uint numGroupsAllocated = 0;
  private uint numLeaves = 0;
  private uint numEmptyLeaves = 0;
  private uint numNonEmptyNonLeaves = 0;
  private uint numGeometries = 0;
  private uint numGeometriesAllocated = 0;

  private string error_str = null;

  private void updateCountsRecurse(Group group)
  {

	for (var * child = group.groups.first; child != null; child = child.next)
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

	  for (var * geo = group.group.geometries.first; geo != null; geo = geo.next)
	  {
		numGeometries++;
	  }
	}

  }

  private void apply(StoreVisitor visitor, Group group)
  {
	Debug.Assert(group.kind == Group.Kind.Group);
	visitor.beginGroup(group);

	if (group.attributes.first != null)
	{
	  visitor.beginAttributes(group);
	  for (var * a = group.attributes.first; a != null; a = a.next)
	  {
		visitor.attribute(a.key, a.val);
	  }
	  visitor.endAttributes(group);
	}

	if (group.kind == Group.Kind.Group && group.group.geometries.first != null)
	{
	  visitor.beginGeometries(group);
	  for (var * geo = group.group.geometries.first; geo != null; geo = geo.next)
	  {
		visitor.geometry(geo);
	  }
	  visitor.endGeometries();
	}

	visitor.doneGroupContents(group);

	if (group.groups.first != null)
	{
	  visitor.beginChildren(group);
	  for (var * g = group.groups.first; g != null; g = g.next)
	  {
		apply(visitor, g);
	  }
	  visitor.endChildren();
	}

	visitor.EndGroup();
  }

  private ListHeader<Group> roots = new ListHeader<Group>();
  private ListHeader<DebugLine> debugLines = new ListHeader<DebugLine>();
  private ListHeader<Connection> connections = new ListHeader<Connection>();

}



//C++ TO C# CONVERTER NOTE: C# does not allow anonymous namespaces:
//namespace

//C++ TO C# CONVERTER WARNING: The original C++ template specifier was replaced with a C# generic specifier, which may not produce the same behavior:
//ORIGINAL LINE: template<typename T>


//C++ TO C# CONVERTER NOTE: C# does not allow anonymous namespaces:
//namespace