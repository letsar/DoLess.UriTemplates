# DoLess.UriTemplates

.Net Standard 2.0 implementation of the URI Template Spec [RFC6570](http://tools.ietf.org/html/rfc6570):

* Supports up to level 4 templates expression
* Tested against all test cases from [UriTemplate test suite](https://github.com/uri-templates/uritemplate-test).
* Fluent API
* .Net Standard 2.0
* Partial expand
* Fast

## Install

Install via [Nuget package](https://www.nuget.org/packages/DoLess.UriTemplates)

## Examples

Resolve a URI template:

```csharp
string uriString = UriTemplate.For("http://example.org/{resource}{?genre,count}")
                              .WithParameter("resource", "books")
                              .WithParameter("genre", "sci-fi")
                              .WithParameter("count", 10)
                              .ExpandToString();
uriString.ShouldBeEquivalentTo("http://example.org/books?genre=sci-fi&count=10");
```

You can pass a string list:

```csharp
string uriString = UriTemplate.For("http://example.org/{resource}{?genre}")
                              .WithParameter("resource", "books")
                              .WithParameter("genre", "sci-fi", "horror", "fantasy")
                              .ExpandToString();
uriString.ShouldBeEquivalentTo("http://example.org/books?genre=sci-fi,horror,fantasy");
```

```csharp
string uriString = UriTemplate.For("http://example.org/{resource}{?genre*}")
                              .WithParameter("resource", "books")
                              .WithParameter("genre", "sci-fi", "horror", "fantasy")
                              .ExpandToString();
uriString.ShouldBeEquivalentTo("http://example.org/books?genre=sci-fi&genre=horror&genre=fantasy");
```

Or a string dictionary:

```csharp
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
```

When a parameter is not set, it simply removes it:

```csharp
string uriString = UriTemplate.For("http://example.org/{resource}{?genre,count}")
                              .WithParameter("resource", "books")
                              .WithParameter("count", 10)
                              .ExpandToString();
uriString.ShouldBeEquivalentTo("http://example.org/books?count=10");
```

## Partial Expand

`DoLess.UriTemplates` can expand partially some templates.

The following operators cannot expand partially if there are multiple values:

* Default
* Reserved
* Fragment
* Query

Example:

```csharp
string uriString = UriTemplate.For("http://example.org/{area}/news{?type,count}")
                              .WithParameter("count", 10)
                              .ExpandToString(true);
uriString.ShouldBeEquivalentTo("http://example.org/{area}/news?count=10{&type}");
```

## Roadmap/Ideas

- [x] Support partial expand