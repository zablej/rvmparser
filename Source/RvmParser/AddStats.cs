using System.Diagnostics;

public class Stats
{
  public uint group_n = 0;

  public uint geometry_n = 0;
  public uint pyramid_n = 0;
  public uint box_n = 0;
  public uint rectangular_torus_n = 0;
  public uint circular_torus_n = 0;
  public uint elliptical_dish_n = 0;
  public uint spherical_dish_n = 0;
  public uint snout_n = 0;
  public uint cylinder_n = 0;
  public uint sphere_n = 0;
  public uint facetgroup_n = 0;
  public uint facetgroup_triangles_n = 0;
  public uint facetgroup_quads_n = 0;
  public uint facetgroup_polygon_n = 0;
  public uint facetgroup_polygon_n_contours_n = 0;
  public uint facetgroup_polygon_n_vertices_n = 0;
  public uint line_n = 0;
}


public class AddStats : StoreVisitor
{

  public override void init(Store store)
  {
	store.stats = store.arena.alloc<Stats>();
	stats = store.stats;
  }

//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//  override void beginGroup(Group group);

  public override void geometry(Geometry geo)
  {
	stats.geometry_n++;
	switch (geo.kind)
	{
	case Geometry.Kind.Pyramid:
		stats.pyramid_n++;
		break;
	case Geometry.Kind.Box:
		stats.box_n++;
		break;
	case Geometry.Kind.RectangularTorus:
		stats.rectangular_torus_n++;
		break;
	case Geometry.Kind.CircularTorus:
		stats.circular_torus_n++;
		break;
	case Geometry.Kind.EllipticalDish:
		stats.elliptical_dish_n++;
		break;
	case Geometry.Kind.SphericalDish:
		stats.spherical_dish_n++;
		break;
	case Geometry.Kind.Snout:
		stats.snout_n++;
		break;
	case Geometry.Kind.Cylinder:
		stats.cylinder_n++;
		break;
	case Geometry.Kind.Sphere:
		stats.sphere_n++;
		break;
	case Geometry.Kind.FacetGroup:
	  stats.facetgroup_n++;

	  for (uint p = 0; p < geo.facetGroup.polygons_n; p++)
	  {
		var poly = geo.facetGroup.polygons[p];
		if (poly.contours_n == 1 && poly.contours[0].vertices_n == 3)
		{
		  stats.facetgroup_triangles_n++;
		}
		else if (poly.contours_n == 1 && poly.contours[0].vertices_n == 4)
		{
		  stats.facetgroup_quads_n++;
		}
		else
		{
		  stats.facetgroup_polygon_n++;
		  stats.facetgroup_polygon_n_contours_n += geo.facetGroup.polygons[p].contours_n;
		  for (uint c = 0; c < geo.facetGroup.polygons[p].contours_n; c++)
		  {
			stats.facetgroup_polygon_n_vertices_n += geo.facetGroup.polygons[p].contours[c].vertices_n;
		  }
		}
	  }

	  break;
	case Geometry.Kind.Line:
		stats.line_n++;
		break;
	default:
	  Debug.Assert(false && "Unhandled primitive type");
	  break;
	}

  }

  public override bool done()
  {
	return true;
  }

  private Stats stats = null;

}

