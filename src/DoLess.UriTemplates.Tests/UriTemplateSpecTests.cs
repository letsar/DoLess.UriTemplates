using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using DoLess.UriTemplates.Tests.Entities;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace DoLess.UriTemplates.Tests
{
    [TestFixture]
    public class UriTemplateSpecTests
    {
        [TestCaseSource(nameof(SpecExamplesData))]
        [TestCaseSource(nameof(ExtendedTestsData))]
        [TestCaseSource(nameof(NegativeTestsData))]
        public void RunTests(SpecTestCase testCase)
        {
            SpecTestCaseTests(testCase, false);
        }

        [TestCaseSource(nameof(PartialResolveData))]
        public void PartialRunTests(SpecTestCase testCase)
        {
            SpecTestCaseTests(testCase, true);
        }

        public static IEnumerable<TestCaseData> SpecExamplesData()
        {
            return CreateTestSuiteFromResource("spec-examples");
        }

        public static IEnumerable<TestCaseData> ExtendedTestsData()
        {
            return CreateTestSuiteFromResource("extended-tests");
        }

        public static IEnumerable<TestCaseData> NegativeTestsData()
        {
            return CreateTestSuiteFromResource("negative-tests");
        }

        public static IEnumerable<TestCaseData> PartialResolveData()
        {
            return CreateTestSuiteFromResource("partial-resolve-tests");
        }

        private static void SpecTestCaseTests(SpecTestCase testCase, bool isPartial)
        {
            var uriTemplate = UriTemplate.For(testCase.Template);

            foreach (var variable in testCase.Variables)
            {
                uriTemplate.WithParameter(variable.Key, variable.Value);
            }

            switch (testCase)
            {
                case SpecFailTestCase x:
                    ShouldFail(uriTemplate, isPartial);
                    break;

                case SpecListTestCase x:
                    ShouldMatchOne(uriTemplate, x, isPartial);
                    break;

                case SpecStringTestCase x:
                    ShouldMatch(uriTemplate, x, isPartial);
                    break;

                default:
                    break;
            }
        }

        private static void ShouldFail(UriTemplate uriTemplate, bool isPartial)
        {
            Action job = () => uriTemplate.ExpandToString(isPartial);
            job.ShouldThrow<UriTemplateException>();
        }

        private static void ShouldMatch(UriTemplate uriTemplate, SpecStringTestCase testCase, bool isPartial)
        {
            string result = uriTemplate.ExpandToString(isPartial);
            result.ShouldBeEquivalentTo(testCase.Result);
        }

        private static void ShouldMatchOne(UriTemplate uriTemplate, SpecListTestCase testCase, bool isPartial)
        {
            string result = uriTemplate.ExpandToString(isPartial);
            result.Should()
                  .BeOneOf(testCase.Results);
        }

        private static IEnumerable<TestCaseData> CreateTestSuiteFromResource(string name)
        {
            var t = typeof(UriTemplateSpecTests).Assembly.GetManifestResourceNames();
            using (var stream = typeof(UriTemplateSpecTests).Assembly.GetManifestResourceStream($"DoLess.UriTemplates.Tests.Assets.{name}.json"))
            using (var streamReader = new StreamReader(stream))
            {
                string json = streamReader.ReadToEnd();
                return CreateTestSuite(json).Select(x => x.Value)
                                            .SelectMany(x => x.TestCases)
                                            .Select(x => new TestCaseData(x).SetName($"{name} - {x.Template}"));
            }
        }

        private static Dictionary<string, SpecTestSet> CreateTestSuite(string json)
        {
            return JObject.Parse(json)
                          .Children()
                          .Cast<JProperty>()
                          .ToDictionary(x => x.Name, x => CreateTestSet(x.Name, x.Value));
        }

        private static SpecTestSet CreateTestSet(string name, JToken token)
        {
            int level = token["level"]?.Value<int?>() ?? 4;
            var testSet = new SpecTestSet(name, level);

            testSet.Variables = token["variables"].Cast<JProperty>()
                                                  .ToDictionary(x => x.Name, x => ParseVariable(x));

            testSet.TestCases = token["testcases"].Select(x => CreateTestCase(testSet, x))
                                                  .ToList();

            return testSet;
        }

        private static object ParseVariable(JProperty variable)
        {
            switch (variable.Value)
            {
                case JArray value:
                    return value.Values<string>()
                                .ToList();

                case JObject value:
                    return value.Properties()
                                .ToDictionary(x => x.Name, x => ((JValue)x.Value).ToString(CultureInfo.InvariantCulture));

                case JValue value:
                    return value.ToString(CultureInfo.InvariantCulture);

                default:
                    return null;
            }
        }

        private static SpecTestCase CreateTestCase(SpecTestSet testSet, JToken token)
        {
            var template = token[0].Value<string>();
            var results = token[1];

            switch (results.Type)
            {
                case JTokenType.Array:
                    return new SpecListTestCase(template, testSet, results.Select(x => x.Value<string>()).ToArray());

                case JTokenType.String:
                    return new SpecStringTestCase(template, testSet, results.Value<string>());

                case JTokenType.Boolean:
                    return new SpecFailTestCase(template, testSet);

                default:
                    return null;
            }
        }
    }
}
