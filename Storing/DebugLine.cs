namespace RvmParser.Storing
{
    public class DebugLine
    {
        #region Public Properties

        public float[] A { get; } = new float[3];
        public float[] B { get; } = new float[3];
        public uint Color { get; set; } = 0xff0000u;
        public DebugLine Next { get; set; }

        #endregion Public Properties
    }
}