using System;
using System.Diagnostics;

public class ExportObj : StoreVisitor, System.IDisposable
{
  public bool groupBoundingBoxes = false;

  public void Dispose()
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
	if (!GlobalMembers.open_w(@out, path_obj))
	{
		return false;
	}
	if (!GlobalMembers.open_w(mtl, path_mtl))
	{
		return false;
	}

	string mtllib = path_mtl;
	var l = mtllib.LastIndexOfAny((Convert.ToString("/\\")).ToCharArray());
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
	Debug.Assert(@out);
	Debug.Assert(mtl);

	this.store = store;
	conn = store.conn;

	stack.accommodate(store.groupCountAllocated());

	string colorName = new string(new char[6]);
	for (var * line = store.getFirstDebugLine(); line != null; line = line.next)
	{

	  for (uint k = 0; k < 6; k++)
	  {
		var v = (line.color >> (4 * k)) & 0xf;
		if (v < 10)
		{
			colorName = StringFunctions.ChangeCharacter(colorName, k, '0' + v);
		}
		else
		{
			colorName = StringFunctions.ChangeCharacter(colorName, k, 'a' + v - 10);
		}
	  }
	  var name = store.strings.intern(colorName[0], colorName[6]);
	  if (definedColors.get(uint64_t(name)) == null)
	  {
		definedColors.insert(uint64_t(name), 1);
		var r = (1.0f / 255.0f) * ((line.color >> 16) & 0xFF);
		var g = (1.0f / 255.0f) * ((line.color >> 8) & 0xFF);
		var b = (1.0f / 255.0f) * ((line.color) & 0xFF);
		fprintf(mtl, "newmtl %s\n", name);
		fprintf(mtl, "Ka %f %f %f\n", (2.0f / 3.0f) * r, (2.0f / 3.0f) * g, (2.0f / 3.0f) * b);
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

	//fprintf(out, "usemtl red_line\n");

	//auto l = 0.05f;
	//if (anchors && conn) {
	//  for (unsigned i = 0; i < conn->anchor_n; i++) {
	//    auto * p = conn->p + 3 * i;
	//    auto * n = conn->anchors[i].n;

	//    fprintf(out, "v %f %f %f\n", p[0], p[1], p[2]);
	//    fprintf(out, "v %f %f %f\n", p[0] + l * n[0], p[1] + l * n[1], p[2] + l * n[2]);
	//    fprintf(out, "l %d %d\n", (int)off_v, (int)off_v + 1);
	//    off_v += 2;
	//  }
	//}

	fprintf(@out, "# End of file\n");
  }

  public override void beginModel(Group group)
  {
	fprintf(@out, "# Model project=%s, name=%s\n", group.model.project, group.model.name);
  }

  public override void endModel()
  {
  }

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//  override void beginGroup(Group group);

  public override void EndGroup()
  {
	Debug.Assert(stack_p);
	stack_p--;
  }

  public override void geometry(Geometry geometry)
  {
	var M = geometry.M_3x4;

	if (geometry.colorName == null)
	{
	  geometry.colorName = store.strings.intern("default");
	}

	//if (geometry->kind == Geometry::Kind::Box) {
	//  geometry->colorName = store->strings.intern("blah-red");
	//  geometry->color = 0x880088;
	//}
	//if (geometry->kind == Geometry::Kind::Pyramid) {
	//  geometry->colorName = store->strings.intern("blah-green");
	//  geometry->color = 0x008800;
	//}
	//if (geometry->kind == Geometry::Kind::RectangularTorus) {
	//  geometry->colorName = store->strings.intern("blah-blue");
	//  geometry->color = 0x000088;
	//}
	//if (geometry->kind == Geometry::Kind::FacetGroup) {
	//  geometry->colorName = store->strings.intern("blah-redgg");
	//  geometry->color = 0x888800;
	//}

	if (definedColors.get(uint64_t(geometry.colorName)) == null)
	{
	  definedColors.insert(uint64_t(geometry.colorName), 1);

	  var r = (1.0f / 255.0f) * ((geometry.color >> 16) & 0xFF);
	  var g = (1.0f / 255.0f) * ((geometry.color >> 8) & 0xFF);
	  var b = (1.0f / 255.0f) * ((geometry.color) & 0xFF);

	  fprintf(mtl, "newmtl %s\n", geometry.colorName);
	  fprintf(mtl, "Ka %f %f %f\n", (2.0f / 3.0f) * r, (2.0f / 3.0f) * g, (2.0f / 3.0f) * b);
	  fprintf(mtl, "Kd %f %f %f\n", r,g, b);
	  fprintf(mtl, "Ks 0.5 0.5 0.5\n");
	}

	fprintf(@out, "usemtl %s\n", geometry.colorName);

	var scale = 1.0f;

	if (geometry.kind == Geometry.Kind.Line)
	{
	  var a = scale * GlobalMembers.mul(geometry.M_3x4, new Vec3f(geometry.line.a, 0F, 0F));
	  var b = scale * GlobalMembers.mul(geometry.M_3x4, new Vec3f(geometry.line.b, 0F, 0F));
	  fprintf(@out, "v %f %f %f\n", a.x, a.y, a.z);
	  fprintf(@out, "v %f %f %f\n", b.x, b.y, b.z);
	  fprintf(@out, "l -1 -2\n");
	  off_v += 2;
	}
	else
	{
	  Debug.Assert(geometry.triangulation);
	  var tri = geometry.triangulation;

	  if (tri.indices != 0)
	  {
		//fprintf(out, "g\n");
		if (geometry.triangulation.error != 0.0f)
		{
		  fprintf(@out, "# error=%f\n", geometry.triangulation.error);
		}
		for (size_t i = 0; i < 3 * tri.vertices_n; i += 3)
		{

		  var p = scale * GlobalMembers.mul(geometry.M_3x4, new Vec3f(tri.vertices + i));
		  Vec3f n = GlobalMembers.normalize(GlobalMembers.mul(new Mat3f(geometry.M_3x4.data), new Vec3f(tri.normals + i)));
		  if (!!Double.IsInfinity(n.x) && !Double.IsNaN(n.x) || !!Double.IsInfinity(n.y) && !Double.IsNaN(n.y) || !!Double.IsInfinity(n.z) && !Double.IsNaN(n.z))
		  {
			n = new Vec3f(1.0f, 0.0f, 0.0f);
		  }
		  fprintf(@out, "v %f %f %f\n", p.x, p.y, p.z);
		  fprintf(@out, "vn %f %f %f\n", n.x, n.y, n.z);
		}
		if (tri.texCoords != 0)
		{
		  for (size_t i = 0; i < tri.vertices_n; i++)
		  {
			Vec2f vt = new Vec2f(tri.texCoords + 2 * i);
			fprintf(@out, "vt %f %f\n", vt.x, vt.y);
		  }
		}
		else
		{
		  for (size_t i = 0; i < tri.vertices_n; i++)
		  {
			var p = scale * GlobalMembers.mul(geometry.M_3x4, new Vec3f(tri.vertices + 3 * i));
			fprintf(@out, "vt %f %f\n", 0 * p.x, 0 * p.y);
		  }

		  for (size_t i = 0; i < 3 * tri.triangles_n; i += 3)
		  {
			var a = tri.indices[i + 0];
			var b = tri.indices[i + 1];
			var c = tri.indices[i + 2];
			fprintf(@out, "f %d/%d/%d %d/%d/%d %d/%d/%d\n", a + off_v, a + off_t, a + off_n, b + off_v, b + off_t, b + off_n, c + off_v, c + off_t, c + off_n);
		  }
		}

		off_v += tri.vertices_n;
		off_n += tri.vertices_n;
		off_t += tri.vertices_n;
	  }
	}

	//if (primitiveBoundingBoxes) {
	//  fprintf(out, "usemtl magenta\n");

	//  for (unsigned i = 0; i < 8; i++) {
	//    float px = (i & 1) ? geometry->bbox[0] : geometry->bbox[3];
	//    float py = (i & 2) ? geometry->bbox[1] : geometry->bbox[4];
	//    float pz = (i & 4) ? geometry->bbox[2] : geometry->bbox[5];

	//    float Px = M[0] * px + M[3] * py + M[6] * pz + M[9];
	//    float Py = M[1] * px + M[4] * py + M[7] * pz + M[10];
	//    float Pz = M[2] * px + M[5] * py + M[8] * pz + M[11];

	//    fprintf(out, "v %f %f %f\n", Px, Py, Pz);
	//  }
	//  fprintf(out, "l %d %d %d %d %d\n",
	//          off_v + 0, off_v + 1, off_v + 3, off_v + 2, off_v + 0);
	//  fprintf(out, "l %d %d %d %d %d\n",
	//          off_v + 4, off_v + 5, off_v + 7, off_v + 6, off_v + 4);
	//  fprintf(out, "l %d %d\n", off_v + 0, off_v + 4);
	//  fprintf(out, "l %d %d\n", off_v + 1, off_v + 5);
	//  fprintf(out, "l %d %d\n", off_v + 2, off_v + 6);
	//  fprintf(out, "l %d %d\n", off_v + 3, off_v + 7);
	//  off_v += 8;
	//}


	//for (unsigned k = 0; k < 6; k++) {
	//  auto other = geometry->conn_geo[k];
	//  if (geometry < other) {
	//    fprintf(out, "usemtl blue_line\n");
	//    float p[3];
	//    getMidpoint(p, geometry);
	//    fprintf(out, "v %f %f %f\n", p[0], p[1], p[2]);
	//    getMidpoint(p, other);
	//    fprintf(out, "v %f %f %f\n", p[0], p[1], p[2]);
	//    fprintf(out, "l %d %d\n", off_v, off_v + 1);

	//    off_v += 2;
	//  }
	//}

  }

  private FILE @out = null;
  private FILE mtl = null;
  private Map definedColors = new Map();
  private Store store = null;
  private readonly Buffer<char> stack = new Buffer<char>();
  private uint stack_p = 0;
  private uint off_v = 1;
  private uint off_n = 1;
  private uint off_t = 1;
  private Connectivity conn = null;
  private float[] curr_translation = {0F, 0F, 0F};

  private bool anchors = false;
  private bool primitiveBoundingBoxes = false;
  private bool compositeBoundingBoxes = false;

}

//C++ TO C# CONVERTER NOTE: C# does not allow anonymous namespaces:
//namespace



//C++ TO C# CONVERTER NOTE: C# does not allow anonymous namespaces:
//namespace