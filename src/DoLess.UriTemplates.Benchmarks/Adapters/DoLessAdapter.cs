using System;

namespace DoLess.UriTemplates.Benchmarks.Adapters
{
    public class DoLessAdapter : UriTemplateAdapter
    {
        private DoLess.UriTemplates.UriTemplate adaptee;

        public DoLessAdapter()
            : base("DoLess")
        {
        }

        protected override void AddParameterInternal(string name, object value)
        {
            this.adaptee.WithParameter(name, value);
        }

        protected override string ExpandInternal()
        {
            return this.adaptee.ExpandToString();
        }

        protected override void InitAdaptee()
        {
            this.adaptee = DoLess.UriTemplates.UriTemplate.For(this.Template, this.CaseSensitive);
        }
    }
}
