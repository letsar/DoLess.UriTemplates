using DoLess.UriTemplates.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace DoLess.UriTemplates.Tests.Helpers
{
    [TestFixture]
    public class StringFormattersTests
    {
        [Test]
        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("One", "one")]
        [TestCase("OneTwo", "oneTwo")]
        [TestCase("OneTwoThree", "oneTwoThree")]
        [TestCase("One-Two", "oneTwo")]
        [TestCase("One Two", "oneTwo")]
        [TestCase("One_Two", "oneTwo")]        
        public void ShouldBeLowerCamelCase(string source, string expected)
        {
            var result = StringFormatters.ToLowerCamelCase(source);
            result.ShouldBeEquivalentTo(expected);
        }

        [Test]
        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("One", "One")]
        [TestCase("OneTwo", "OneTwo")]
        [TestCase("OneTwoThree", "OneTwoThree")]
        [TestCase("One-Two", "OneTwo")]
        [TestCase("One Two", "OneTwo")]
        [TestCase("One_Two", "OneTwo")]
        public void ShouldBeUpperCamelCase(string source, string expected)
        {
            var result = StringFormatters.ToUpperCamelCase(source);
            result.ShouldBeEquivalentTo(expected);
        }

        [Test]
        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("One", "one")]
        [TestCase("OneTwo", "one_two")]
        [TestCase("OneTwoThree", "one_two_three")]
        [TestCase("One-Two", "one_two")]
        [TestCase("One Two", "one_two")]
        [TestCase("One_Two", "one_two")]
        public void ShouldBeLowerSnakeCase(string source, string expected)
        {
            var result = StringFormatters.ToLowerSnakeCase(source);
            result.ShouldBeEquivalentTo(expected);
        }

        [Test]
        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("One", "One")]
        [TestCase("OneTwo", "One_Two")]
        [TestCase("OneTwoThree", "One_Two_Three")]
        [TestCase("One-Two", "One_Two")]
        [TestCase("One Two", "One_Two")]
        [TestCase("One_Two", "One_Two")]
        public void ShouldBeUpperSnakeCase(string source, string expected)
        {
            var result = StringFormatters.ToUpperSnakeCase(source);
            result.ShouldBeEquivalentTo(expected);
        }

        [Test]
        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("One", "one")]
        [TestCase("OneTwo", "one-two")]
        [TestCase("OneTwoThree", "one-two-three")]
        [TestCase("One-Two", "one-two")]
        [TestCase("One Two", "one-two")]
        [TestCase("One_Two", "one-two")]
        public void ShouldBeKebabCase(string source, string expected)
        {
            var result = StringFormatters.ToKebabCase(source);
            result.ShouldBeEquivalentTo(expected);
        }

        [Test]
        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("One", "One")]
        [TestCase("OneTwo", "One-Two")]
        [TestCase("OneTwoThree", "One-Two-Three")]
        [TestCase("One-Two", "One-Two")]
        [TestCase("One Two", "One-Two")]
        [TestCase("One_Two", "One-Two")]
        public void ShouldBeTrainCase(string source, string expected)
        {
            var result = StringFormatters.ToTrainCase(source);
            result.ShouldBeEquivalentTo(expected);
        }
    }
}
