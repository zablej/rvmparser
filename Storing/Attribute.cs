namespace RvmParser.Storing
{
    public class Attribute
    {
        #region Public Properties

        public string Key { get; set; }
        public Attribute Next { get; set; }
        public string Value { get; set; }

        #endregion Public Properties
    }
}