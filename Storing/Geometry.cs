using RvmParser.Storing.Shapes;

namespace RvmParser.Storing
{
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

        #region Public Properties

        public BBox3f BBoxLocal { get; set; } = new BBox3f();
        public BBox3f BBoxWorld { get; set; } = new BBox3f();
        public object ClientData { get; set; }
        public uint Color { get; set; } = 0x202020u;
        public string ColorName { get; set; }
        public Connection[] Connections { get; set; } = { null, null, null, null, null, null };
        public uint ID { get; set; }
        public Kind Kind { get; set; }
        public Mat3x4f M_3x4 { get; set; } = new Mat3x4f();
        public Geometry Next { get; set; } // Next geometry in the list of geometries in group.
        public Geometry NextComp { get; set; }

        // Next geometry in list of geometries of this composite
        public float sampleStartAngle { get; set; } = 0.0f;

        public Triangulation Triangulation { get; set; }

        #endregion Public Properties

        public IShape Shape { get; set; }
    }
}