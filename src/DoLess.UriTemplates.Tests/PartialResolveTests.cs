using System;
using FluentAssertions;
using NUnit.Framework;

namespace DoLess.UriTemplates.Tests
{
    [TestFixture]
    public class PartialResolveTests
    {
        [Ignore("Not implemented")]
        [TestCase("host", "www.example.org", "http://www.example.org{/path}{?query}{#fragment}")]
        [TestCase("path", "some_resource", "http://{host}/some_resource{?query}{#fragment}")]
        [TestCase("query", "test", "http://{host}{/path}?query=test{#fragment}")]
        [TestCase("fragment", "frag", "http://{host}{/path}{?query}#frag")]
        public void MonoParameterShouldBeExpandedAsExpected(string name, string value, string expected)
        {
            var result = UriTemplate.For("http://{host}{/path}{?query}{#fragment}")
                                    .WithParameter(name, value)
                                    .ExpandToString();
            result.ShouldBeEquivalentTo(expected);
        }

        [Ignore("Not implemented")]
        [TestCase("http://ex.org/{path1,path2,path3}/test", "http://ex.org/{path1}p2{path3}/test")]
        [TestCase("http://ex.org{/path1,path2,path3}/test", "http://ex.org{/path1}/p2{/path3}/test")] // Can be done.
        [TestCase("http://ex.org/{+path1,path2,path3}/test", "http://ex.org/{+path1}p2{+path3}/test")]
        [TestCase("http://ex.org/{#path1,path2,path3}", "http://ex.org/#p2{#path1}p2{+path3}")]
        public void MultiParametersShouldBeExpandedAsExpectedWhenOnlyOneSet(string template, string expected)
        {
            var result = UriTemplate.For(template)
                                    .WithParameter("param2", "p2")
                                    .ExpandToString();
            result.ShouldBeEquivalentTo(expected);
        }
    }
}
