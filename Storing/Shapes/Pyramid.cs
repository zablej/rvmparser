namespace RvmParser.Storing.Shapes
{
    public class Pyramid : IShape
    {
        #region Public Properties

        public float[] Bottom { get; } = new float[2];
        public float Height { get; set; }
        public float[] Offset { get; } = new float[2];
        public float[] Top { get; } = new float[2];

        #endregion Public Properties
    }
}