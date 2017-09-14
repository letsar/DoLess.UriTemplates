using System;

namespace DoLess.UriTemplates.Benchmarks.Adapters
{
    public class TavisAdapter : UriTemplateAdapter
    {
        private Tavis.UriTemplates.UriTemplate adaptee;

        public TavisAdapter()
            : base("Tavis")
        {
        }

        protected override void AddParameterInternal(string name, object value)
        {
            this.adaptee.SetParameter(name, value);
        }

        protected override string ExpandInternal()
        {
            return this.adaptee.Resolve();
        }

        protected override void InitAdaptee()
        {
            this.adaptee = new Tavis.UriTemplates.UriTemplate(this.Template, this.PartialExpand, !this.CaseSensitive);
        }
    }
}
