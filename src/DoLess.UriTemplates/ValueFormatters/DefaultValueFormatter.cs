using System;
using System.Globalization;

namespace DoLess.UriTemplates.ValueFormatters
{
    internal class DefaultValueFormatter : IValueFormatter
    {
        public string Format(object value)
        {
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }
    }
}
