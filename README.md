# DoLess.UriTemplates

.Net Standard 2.0 implementation of the URI Template Spec [RFC6570](http://tools.ietf.org/html/rfc6570):

* Supports up to level 4 templates expression
* Tested against all test cases from [UriTemplate test suite](https://github.com/uri-templates/uritemplate-test).
* Fluent API
* .Net Standard 2.0

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

## Extended Spec

The original specification has some limitations. In order to have more functionalities, `DoLess.UriTemplates` introduce some new concepts.

**This is not in the original specification, so this can change in the future, along with the specification.**

### Expression Modifiers

In order to fully support the partial expand, `DoLess.UriTemplates` supports expression modifiers.
An expression modifier follow immediately the operator. In case of the default operator, it will be the first character.

### Start modifier

```abnf
start = "<"
```

A start ("<") modifier indicates to always prepend to the result the "first" string defined by the operator, even if none of the expression's variable are defined.
If any of the expression's variables are defined, this indicates to append the "sep" string defined by the operator to the result.

Example:

```
var1 := "v1"

"{<var1}" = "v1,"
"{?<var}" = "?"
```

### End modifier

```abnf
end = ">"
```

An end (">") modifier indicates to prepend to the result the "sep" string defined by the operator if any of the expression's variable are defined.

Example:

```
var1 := "v1"

"{>var1}" = ",v1"
"{?>var}" = ""
```

This modifier is only useful for these operators:

* Default
* Reserved
* Fragment
* Query

## Roadmap/Ideas

- [ ] Support partial resolve