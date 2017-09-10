namespace DoLess.UriTemplates.Tests.Entities
{
    public class SpecStringTestCase : SpecTestCase
    {
        public SpecStringTestCase(string template, SpecTestSet testSet, string result)
            : base(template, testSet)
        {
            this.Result = result;
        }

        public string Result { get; }
    }
}
