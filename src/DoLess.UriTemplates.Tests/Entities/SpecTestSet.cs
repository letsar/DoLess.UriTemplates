using System.Collections.Generic;

namespace DoLess.UriTemplates.Tests.Entities
{
    public class SpecTestSet
    {
        public SpecTestSet(string name, int level = 4)
        {
            this.Name = name;
            this.Level = level;
        }

        public string Name { get; }

        public int Level { get; }

        public Dictionary<string, object> Variables { get; set; }

        public List<SpecTestCase> TestCases { get; set; }
    }
}
