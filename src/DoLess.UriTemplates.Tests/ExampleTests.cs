using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace DoLess.UriTemplates.Tests
{
    [TestFixture]
    public class ExampleTests
    {
        [Test]
        public void Example01()
        {
            string uriString = UriTemplate.For("http://example.org/{resource}{?genre,count}")
                                          .WithParameter("resource", "books")
                                          .WithParameter("genre", "sci-fi")
                                          .WithParameter("count", 10)
                                          .ExpandToString();
            uriString.ShouldBeEquivalentTo("http://example.org/books?genre=sci-fi&count=10");
        }

        [Test]
        public void Example02()
        {
            string uriString = UriTemplate.For("http://example.org/{resource}{?genre}")
                                          .WithParameter("resource", "books")
                                          .WithParameter("genre", "sci-fi", "horror", "fantasy")
                                          .ExpandToString();
            uriString.ShouldBeEquivalentTo("http://example.org/books?genre=sci-fi,horror,fantasy");
        }

        [Test]
        public void Example03()
        {
            string uriString = UriTemplate.For("http://example.org/{resource}{?genre*}")
                                          .WithParameter("resource", "books")
                                          .WithParameter("genre", "sci-fi", "horror", "fantasy")
                                          .ExpandToString();
            uriString.ShouldBeEquivalentTo("http://example.org/books?genre=sci-fi&genre=horror&genre=fantasy");
        }

        [Test]
        public void Example04()
        {
            Dictionary<string, string> options = new Dictionary<string, string>
            {
                ["genre"] = "sci-fi",
                ["count"] = "10",
                ["author"] = "George R. R. Martin",
            };

            string uriString = UriTemplate.For("http://example.org/{resource}{?options*}")
                                          .WithParameter("resource", "books")
                                          .WithParameter("options", options)
                                          .ExpandToString();
            uriString.ShouldBeEquivalentTo("http://example.org/books?genre=sci-fi&count=10&author=George%20R.%20R.%20Martin");
        }

        [Test]
        public void Example05()
        {
            string uriString = UriTemplate.For("http://example.org/{resource}{?genre,count}")
                                          .WithParameter("resource", "books")
                                          .WithParameter("count", 10)
                                          .ExpandToString();
            uriString.ShouldBeEquivalentTo("http://example.org/books?count=10");
        }

        [Test]
        public void Example06()
        {
            string uriString = UriTemplate.For("http://example.org/{area}/news{?type,count}")
                                          .WithParameter("count", 10)
                                          .ExpandToString(true);
            uriString.ShouldBeEquivalentTo("http://example.org/{area}/news?count=10{&type}");
        }

    }
}
