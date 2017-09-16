namespace DoLess.UriTemplates.Entities
{
    internal class ExpressionInfo
    {
        public static ExpressionInfo Default = new ExpressionInfo(string.Empty, string.Empty, ',', false, string.Empty, false, false);
        public static ExpressionInfo Reserved = new ExpressionInfo("+", string.Empty, ',', false, string.Empty, true, false);
        public static ExpressionInfo Label = new ExpressionInfo(".", ".", '.', false, string.Empty, false, true);
        public static ExpressionInfo Path = new ExpressionInfo("/", "/", '/', false, string.Empty, false, true);
        public static ExpressionInfo Matrix = new ExpressionInfo(";", ";", ';', true, string.Empty, false, true);
        public static ExpressionInfo Query = new ExpressionInfo("?", "?", '&', true, "=", false, true);
        public static ExpressionInfo Continuation = new ExpressionInfo("&", "&", '&', true, "=", false, true);
        public static ExpressionInfo Fragment = new ExpressionInfo("#", "#", ',', false, string.Empty, true, false);

        private ExpressionInfo(string opCode, string first, char sep, bool named, string ifemp, bool allowReserved, bool supportsPartialExpandInMulti)
        {
            this.OpCode = opCode;
            this.First = first;
            this.Separator = sep;
            this.IsNamed = named;
            this.IfEmpty = ifemp;
            this.AllowReserved = allowReserved;
            this.SupportsPartialExpandInMulti = supportsPartialExpandInMulti;
        }

        public string OpCode { get; }

        public string First { get; }

        public char Separator { get; }

        public bool IsNamed { get; }

        public string IfEmpty { get; }

        public bool AllowReserved { get; }

        public bool SupportsPartialExpandInMulti { get; }
    }
}
