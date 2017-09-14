using System;
using System.Collections.Generic;

namespace DoLess.UriTemplates.Benchmarks.Adapters
{
    public class RestaAdapter : UriTemplateAdapter
    {
        private Resta.UriTemplates.UriTemplate adaptee;
        private Dictionary<string, object> variables;

        public RestaAdapter()
            : base("Resta")
        {
        }

        protected override void AddParameterInternal(string name, object value)
        {
            this.variables[name] = value;
        }

        protected override string ExpandInternal()
        {
            return this.adaptee.Resolve(this.variables);
        }

        protected override void InitAdaptee()
        {
            this.variables = new Dictionary<string, object>(this.CaseSensitive ? null : StringComparer.OrdinalIgnoreCase);
            this.adaptee = new Resta.UriTemplates.UriTemplate(this.Template);
        }
    }
}
