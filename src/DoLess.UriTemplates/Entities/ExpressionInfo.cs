using System;
namespace DoLess.UriTemplates.Entities
{
    internal class ExpressionInfo
    {
        public static ExpressionInfo Default = new ExpressionInfo(string.Empty, string.Empty, ',', false, string.Empty, false);
        public static ExpressionInfo Reserved = new ExpressionInfo("+", string.Empty, ',', false, string.Empty, true);
        public static ExpressionInfo Label = new ExpressionInfo(".", ".", '.', false, string.Empty, false);
        public static ExpressionInfo Path = new ExpressionInfo("/", "/", '/', false, string.Empty, false);
        public static ExpressionInfo Matrix = new ExpressionInfo(";", ";", ';', true, string.Empty, false);
        public static ExpressionInfo Query = new ExpressionInfo("?", "?", '&', true, "=", false, "&");
        public static ExpressionInfo Continuation = new ExpressionInfo("&", "&", '&', true, "=", false);
        public static ExpressionInfo Fragment = new ExpressionInfo("#", "#", ',', false, string.Empty, true, "");

        private ExpressionInfo(string opCode, string first, char sep, bool named, string ifemp, bool allowReserved, string otherSep = null)
        {
            this.OpCode = opCode;
            this.First = first;
            this.Separator = sep;
            this.IsNamed = named;
            this.IfEmpty = ifemp;
            this.AllowReserved = allowReserved;
            this.OtherSeparator = otherSep ?? opCode;

        }

        public string OpCode { get; }

        public string First { get; }

        public char Separator { get; }

        public bool IsNamed { get; }

        public string IfEmpty { get; }

        public bool AllowReserved { get; }

        public string OtherSeparator { get; }
    }
}
