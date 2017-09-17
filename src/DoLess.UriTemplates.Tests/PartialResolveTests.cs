using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace DoLess.UriTemplates.Tests
{
    [TestFixture]
    public class PartialResolveTests
    {
        private static readonly Dictionary<string, object> Variables = new Dictionary<string, object>
        {
            ["var1"] = "v1",
            ["var2"] = "v2",
            ["var3"] = "v3",
            ["var4"] = "v4",
            ["var5"] = "v5",
            ["l1"] = new[] { "a1", "a2", "a3" },
            ["l2"] = new[] { "b1", "b2", "b3" },
            ["a1"] = new Dictionary<string, string> { ["k1"] = "v1", ["k2"] = "v2" },
            ["a2"] = new Dictionary<string, string> { ["l1"] = "w1", ["l2"] = "w2" },
        };

        [TestCase("http://www.example.org{/var1}{?a1*}{#l1}", "http://www.example.org/v1?k1=v1&k2=v2#a1,a2,a3")]
        [TestCase("http://www.example.org{/var3,var1,var5}{?a1*}{#l1}", "http://www.example.org/v3/v1/v5?k1=v1&k2=v2#a1,a2,a3")]
        [TestCase("http://www.example.org{/var3,var1:1,var5}{?a1*}{#l1}", "http://www.example.org/v3/v/v5?k1=v1&k2=v2#a1,a2,a3")]
        [TestCase("http://www.example.org{/var3,var1,var2,var5,var4}{?a1*}{#l1}", "http://www.example.org/v3/v1/v2/v5/v4?k1=v1&k2=v2#a1,a2,a3")]
        [TestCase("http://www.example.org{/var1,var2,var3,var4,var5}{?a1*}{#l1}", "http://www.example.org/v1/v2/v3/v4/v5?k1=v1&k2=v2#a1,a2,a3")]
        [TestCase("http://www.example.org{/var5,var4,var3,var2,var1}{?a1*}{#l1}", "http://www.example.org/v5/v4/v3/v2/v1?k1=v1&k2=v2#a1,a2,a3")]
        [TestCase("http://www.example.org{/udf3,var1,var2,var5,var4}{?a1*}{#l1}", "http://www.example.org/v1/v2/v5/v4?k1=v1&k2=v2#a1,a2,a3")]
        [TestCase("http://www.example.org{/udf3,var1,var2,udf5,var4}{?a1*}{#l1}", "http://www.example.org/v1/v2/v4?k1=v1&k2=v2#a1,a2,a3")]
        [TestCase("http://www.example.org{/udf3,var1,var2,udf5,udf4}{?a1*}{#l1}", "http://www.example.org/v1/v2?k1=v1&k2=v2#a1,a2,a3")]
        [TestCase("http://www.example.org{/var3,var1,var2,var5,var4}", "http://www.example.org/v3/v1/v2/v5/v4")]
        [TestCase("http://www.example.org{/var5,var4,var3,var2,var1}", "http://www.example.org/v5/v4/v3/v2/v1")]
        [TestCase("http://www.example.org{/udf3,var1,var2,var5,var4}", "http://www.example.org/v1/v2/v5/v4")]
        [TestCase("http://www.example.org{/var5,var4,var3,var2,udf1}", "http://www.example.org/v5/v4/v3/v2")]
        [TestCase("http://www.example.org{/var5,var4,udf3,var2,var1}", "http://www.example.org/v5/v4/v2/v1")]
        [TestCase("http://www.example.org{/var5,udf4,udf3,udf2,udf1}", "http://www.example.org/v5")]
        [TestCase("http://www.example.org{/udf5,udf4,udf3,udf2,var1}", "http://www.example.org/v1")]
        [TestCase("http://www.example.org{/udf5,udf4,udf3,udf2,udf1}", "http://www.example.org")]
        public void CascadingPartialExpandShouldBeCorrect(string template, string expected)
        {
            var uriTemplate = UriTemplate.For(template)
                                         .WithPartialExpand();

            foreach (var key in Variables.Keys)
            {
                uriTemplate.WithParameter(key, Variables[key]);
                uriTemplate = UriTemplate.For(uriTemplate.ExpandToString()).WithPartialExpand();
            }

            var result = uriTemplate.WithPartialExpand(false)
                                    .ExpandToString();
            result.ShouldBeEquivalentTo(expected);
        }       
    }
}
