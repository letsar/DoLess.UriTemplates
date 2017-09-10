using System.Collections.Generic;

namespace DoLess.UriTemplates.Tests.Entities
{
    public class SpecListTestCase : SpecTestCase
    {
        public SpecListTestCase(string template, SpecTestSet testSet, string[] results)
            : base(template, testSet)
        {
            this.Results = results;
        }

        public IReadOnlyList<string> Results { get; }
    }
}
