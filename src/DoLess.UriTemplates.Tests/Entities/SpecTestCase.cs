using System.Collections.Generic;
using System.Diagnostics;

namespace DoLess.UriTemplates.Tests.Entities
{
    [DebuggerDisplay("{Template}")]
    public class SpecTestCase
    {
        public SpecTestCase(string template, SpecTestSet testSet)
        {
            this.Template = template;
            this.TestSet = testSet;
        }

        public string Template { get; }

        public SpecTestSet TestSet { get; }

        public IReadOnlyDictionary<string, object> Variables => this.TestSet.Variables;
    }
}
