namespace RvmParser.Storing
{
    public class Triangulation
    {
        #region Public Properties

        public float Error { get; set; }
        public int ID { get; set; }
        public uint[] Indices { get; set; }
        public float[] Normals { get; set; }
        public float TexCoords { get; set; }

        public uint TrianglesN { get; set; }
        public float[] Vertices { get; set; }

        #endregion Public Properties
    }
}