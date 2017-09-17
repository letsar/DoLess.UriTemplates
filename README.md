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

Expand a URI template:

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

`DoLess.UriTemplates` supports the following parameter types:

* `string`
* `IEnumerable<string>`
* `IDictionary<string,string>`
* `IReadOnlyDictionary<string,string>`

All other types will be converted to `string` using the default value converter (which does a `Convert.ToString(value, CultureInfo.InvariantCulture)`).
You can control the way an object is formatted by providing an `IValueFormatter` or a `Func<object,string>`:

```csharp
Func<object, string> func = x =>
{
    switch (x)
    {
        case Vector2 y:
            return $"({y.X},{y.Y})";
        default:
            return x?.ToString();
    }
};
Vector2 u = new Vector2(3, 4);
string uriString = UriTemplate.For("http://example.org{/vector}")
                              .WithParameter("vector", u)
                              .WithValueFormatter(func)
                              .ExpandToString();

uriString.ShouldBeEquivalentTo("http://example.org/%283%2C4%29");
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
                              .WithPartialExpand()
                              .ExpandToString();
uriString.ShouldBeEquivalentTo("http://example.org/{area}/news?count=10{&type}");
```

## Roadmap/Ideas

- [x] Support partial expand