using System;
using System.Collections.Generic;

namespace DoLess.UriTemplates.Benchmarks.Adapters
{
    public abstract class UriTemplateAdapter
    {
        private readonly Dictionary<string, object> variables;

        protected UriTemplateAdapter(string name)
        {
            this.Name = name;
            this.variables = new Dictionary<string, object>();
        }

        public bool PartialExpand { get; set; }

        public string Template { get; set; }

        public bool CaseSensitive { get; set; }

        public string Name { get; }

        public string Expand()
        {
            this.InitAdaptee();
            this.AddParametersInternal(this.variables);
            return this.ExpandInternal();
        }

        public UriTemplateAdapter AddParameter(string name, object value)
        {
            this.variables.Add(name, value);
            return this;
        }

        protected void AddParametersInternal(IReadOnlyDictionary<string, object> variables)
        {
            foreach (var key in variables.Keys)
            {
                this.AddParameterInternal(key, this.variables[key]);
            }
        }

        protected abstract void AddParameterInternal(string name, object value);

        protected abstract string ExpandInternal();

        protected abstract void InitAdaptee();
    }
}
