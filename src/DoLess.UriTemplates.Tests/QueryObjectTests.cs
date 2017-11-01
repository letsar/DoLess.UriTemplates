using System.Collections.Generic;
using DoLess.UriTemplates.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace DoLess.UriTemplates.Tests
{
    [TestFixture]
    public class QueryObjectTests
    {
        [Test]
        [TestCaseSource(nameof(ShouldQueryObjectBeExpandedAsExpectedTestCases))]
        public void ShouldQueryObjectBeExpandedAsExpected(string expected, Options options, Filters filters)
        {
            var result = UriTemplate.For("/api{?options*,filters*}")
                                    .WithParameter("options", options)
                                    .WithParameter("filters", filters)
                                    .ExpandToString();
            result.ShouldBeEquivalentTo(expected);
        }

        public void ShouldQueryObjectBeExpandedAsExpected()
        {
            Filters filters = new Filters
            {
                Year = 1988,
                Genres = new[] { "action", "adventure" }
            };

            var result = UriTemplate.For("/api{?filters*}")
                                    .WithParameter("filters", filters)
                                    .ExpandToString();

            result.ShouldBeEquivalentTo("/api?year=1988&genres=action,adventure");
        }

        [Test]
        public void ShouldHaveRightCases()
        {
            CustomQueryObject customQueryObject = new CustomQueryObject
            {
                KebabCase = 1,
                LowerCamelCase = 2,
                MyOwnFormat = 3
            };

            var result = UriTemplate.For("/api{?filters*}")
                                    .WithParameter("filters", customQueryObject)
                                    .ExpandToString();

            result.ShouldBeEquivalentTo("/api?kebab-case=1&lowerCamelCase=2&WithMyOwnKey=3");
        }

        private static readonly object[] ShouldQueryObjectBeExpandedAsExpectedTestCases =
        {
            new TestCaseData("/api",null,null),
            new TestCaseData("/api",new Options(),null),
            new TestCaseData("/api?page=1",new Options{Page = 1},null),
            new TestCaseData("/api?id_type=movie",new Options{IdType = "movie"},null),
            new TestCaseData("/api?page=1&id_type=movie",new Options{Page = 1,IdType = "movie"},null),
            new TestCaseData("/api",null,new Filters()),
            new TestCaseData("/api?year=1988",null,new Filters{Year = 1988}),
            new TestCaseData("/api?genres=action,adventure",null,new Filters{Genres = new[]{"action","adventure"}}),
            new TestCaseData("/api?year=1988&genres=action,adventure",null,new Filters{Year = 1988, Genres = new[]{"action","adventure"}}),
            new TestCaseData("/api?page=1&id_type=movie&year=1988&genres=action,adventure",new Options{Page = 1, IdType ="movie"},new Filters{Year = 1988, Genres = new[]{"action","adventure"}}),
        };

        public class Options : QueryObject
        {
            public int Page
            {
                get => this.Get<int>();
                set => this.Set<int>(value);
            }

            public string IdType
            {
                get => this.Get<string>();
                set => this.Set<string>(value);
            }
        }

        public class Filters : QueryObject
        {
            public int Year
            {
                get => this.Get<int>();
                set => this.Set<int>(value);
            }

            public IEnumerable<string> Genres
            {
                get => this.Get<IEnumerable<string>>();
                set => this.Set<IEnumerable<string>>(value);
            }
        }

        public class CustomQueryObject : QueryObject
        {
            public CustomQueryObject() 
                : base(StringFormatters.ToKebabCase)
            {
            }

            public int KebabCase
            {
                get => this.Get<int>();
                set => this.Set<int>(value);
            }

            public int LowerCamelCase
            {
                get => this.Get<int>(StringFormatters.ToLowerCamelCase);
                set => this.Set<int>(StringFormatters.ToLowerCamelCase, value);
            }

            public int MyOwnFormat
            {
                get => this.Get<int>(null,"WithMyOwnKey");
                set => this.Set<int>(null, value, "WithMyOwnKey");
            }
        }
    }
}
