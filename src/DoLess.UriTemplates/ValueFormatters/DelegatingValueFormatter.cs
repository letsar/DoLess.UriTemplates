using System;

namespace DoLess.UriTemplates.ValueFormatters
{
    internal class DelegatingValueFormatter : IValueFormatter
    {
        private readonly Func<object, string> func;

        public DelegatingValueFormatter(Func<object, string> func)
        {
            this.func = func ?? throw new ArgumentNullException(nameof(func));
        }

        public string Format(object value)
        {
            return this.func(value);
        }
    }
}
