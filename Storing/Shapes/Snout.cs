namespace RvmParser.Storing.Shapes
{
    public class Snout : IShape
    {
        #region Public Properties

        public float[] BShear { get; } = new float[2];
        public float Height { get; set; }
        public float[] Offset { get; } = new float[2];
        public float RadiusB { get; set; }
        public float RadiusT { get; set; }
        public float[] TShear { get; } = new float[2];

        #endregion Public Properties
    }
}