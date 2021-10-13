namespace RvmParser.Storing
{
    public class Color
    {
        #region Public Properties

        public uint ColorIndex { get; set; }
        public uint ColorKind { get; set; }
        public Color Next { get; set; }
        public byte[] Rgb { get; } = new byte[3];

        #endregion Public Properties
    }
}