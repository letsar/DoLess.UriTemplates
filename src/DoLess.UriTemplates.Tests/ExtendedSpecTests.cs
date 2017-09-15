using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace DoLess.UriTemplates.Tests
{
    [TestFixture]
    public class ExtendedSpecTests
    {
        [TestCase("{var1?}", "", "v1,")]
        [TestCase("{+var1?}", "", "v1,")]
        [TestCase("{.var1?}", ".", ".v1.")]
        [TestCase("{/var1?}", "/", "/v1/")]
        [TestCase("{;var1?}", ";", ";var1=v1;")]
        [TestCase("{?var1?}", "?", "?var1=v1&")]
        [TestCase("{&var1?}", "&", "&var1=v1&")]
        [TestCase("{#var1?}", "#", "#v1,")]
        public void ShouldConditionalModifierProcessAsExpected(string template, string expectedWhenUndefined, string expectedWhenDefined)
        {
            var resultWhenUndefined = UriTemplate.For(template)
                                                 .ExpandToString();

            var resultWhenDefined = UriTemplate.For(template)
                                               .WithParameter("var1", "v1")
                                               .ExpandToString();

            resultWhenUndefined.ShouldBeEquivalentTo(expectedWhenUndefined);
            resultWhenDefined.ShouldBeEquivalentTo(expectedWhenDefined);
        }

        [TestCase("{var1&}", "", ",v1")]
        [TestCase("{+var1&}", "", ",v1")]
        [TestCase("{.var1&}", "", ".v1")]
        [TestCase("{/var1&}", "", "/v1")]
        [TestCase("{;var1&}", "", ";var1=v1")]
        [TestCase("{?var1&}", "", "&var1=v1")]
        [TestCase("{&var1&}", "", "&var1=v1")]
        [TestCase("{#var1&}", "", ",v1")]
        public void ShouldContinuationModifierProcessAsExpected(string template, string expectedWhenUndefined, string expectedWhenDefined)
        {
            var resultWhenUndefined = UriTemplate.For(template)
                                                 .ExpandToString();

            var resultWhenDefined = UriTemplate.For(template)
                                               .WithParameter("var1", "v1")
                                               .ExpandToString();

            resultWhenUndefined.ShouldBeEquivalentTo(expectedWhenUndefined);
            resultWhenDefined.ShouldBeEquivalentTo(expectedWhenDefined);
        }

    }
}
