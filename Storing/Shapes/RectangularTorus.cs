namespace RvmParser.Storing.Shapes
{
    public class RectangularTorus : IShape
    {
        #region Public Properties

        public float Angle { get; set; }
        public float Height { get; set; }
        public float InnerRadius { get; set; }
        public float OuterRadius { get; set; }

        #endregion Public Properties
    }
}