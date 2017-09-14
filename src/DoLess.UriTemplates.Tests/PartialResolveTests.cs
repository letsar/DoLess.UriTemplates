using System;
using FluentAssertions;
using NUnit.Framework;

namespace DoLess.UriTemplates.Tests
{
    [TestFixture]
    public class PartialResolveTests
    {
        [TestCase("host", "www.example.org", "http://www.example.org{/path}{?query}{#fragment}")]
        [TestCase("path", "some_resource", "http://{host}/some_resource{?query}{#fragment}")]
        [TestCase("query", "test", "http://{host}{/path}?query=test{#fragment}")]
        [TestCase("fragment", "frag", "http://{host}{/path}{?query}#frag")]
        public void ShouldMonoParameterShouldBeExpandedAsExpected (string name, string value, string expected)
        {
            var result = UriTemplate.For("http://{host}{/path}{?query}{#fragment}")
                                    .WithParameter(name, value)
                                    .ExpandToString();
            result.ShouldBeEquivalentTo(expected);
        }
    }
}
