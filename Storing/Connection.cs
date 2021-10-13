namespace RvmParser.Storing
{
    public class Connection
    {
        #region Public Enums

        public enum Flags : byte
        {
            None = 0,
            HasCircularSide = 1 << 0,
            HasRectangularSide = 1 << 1
        }

        #endregion Public Enums

        #region Public Properties

        public Vec3f D { get; set; } = new Vec3f();
        public Flags Flag { get; set; }
        public Geometry[] Geometry { get; } = { null, null };
        public Connection Next { get; set; }
        public uint[] Offset { get; } = new uint[2];
        public Vec3f P { get; set; } = new Vec3f();
        public uint Temp { get; set; }

        #endregion Public Properties
    }
}