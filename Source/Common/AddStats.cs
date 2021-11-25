using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Common
{
    public struct Stats
    {
        public uint group_n;

        public uint geometry_n;
        public uint pyramid_n;
        public uint box_n;
        public uint rectangular_torus_n;
        public uint circular_torus_n;
        public uint elliptical_dish_n;
        public uint spherical_dish_n;
        public uint snout_n;
        public uint cylinder_n;
        public uint sphere_n;
        public uint facetgroup_n;
        public uint facetgroup_triangles_n;
        public uint facetgroup_quads_n;
        public uint facetgroup_polygon_n;
        public uint facetgroup_polygon_n_contours_n;
        public uint facetgroup_polygon_n_vertices_n;
        public uint line_n;
    }

    public class AddStats : StoreVisitor
    {
        public override void init(Store store)
        {
            store.stats = new Stats();
            stats = store.stats;
        }

        public override void beginGroup(Group group)
        {
            stats.group_n++;
        }

        public override void geometry(Geometry geo)
        {
            stats.geometry_n++;
            switch (geo.kind)
            {
                case Geometry.Kind.Pyramid: stats.pyramid_n++; break;
                case Geometry.Kind.Box: stats.box_n++; break;
                case Geometry.Kind.RectangularTorus: stats.rectangular_torus_n++; break;
                case Geometry.Kind.CircularTorus: stats.circular_torus_n++; break;
                case Geometry.Kind.EllipticalDish: stats.elliptical_dish_n++; break;
                case Geometry.Kind.SphericalDish: stats.spherical_dish_n++; break;
                case Geometry.Kind.Snout: stats.snout_n++; break;
                case Geometry.Kind.Cylinder: stats.cylinder_n++; break;
                case Geometry.Kind.Sphere: stats.sphere_n++; break;
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
                case Geometry.Kind.Line: stats.line_n++; break;
                default:
                    Debug.Assert(false, "Unhandled primitive type");
                    break;
            }
        }

        public override bool done()
        {
            return true;
        }

        private Stats stats;

};
}
