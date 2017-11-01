# DoLess.UriTemplates

.Net Standard 1.3 implementation of the URI Template Spec [RFC6570](http://tools.ietf.org/html/rfc6570):

* Supports up to level 4 templates expression
* Tested against all test cases from [UriTemplate test suite](https://github.com/uri-templates/uritemplate-test).
* Fluent API
* .Net Standard 1.3
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
* `IEnumerable<KeyValuePair<string, string>`
* `IEnumerable`

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

## Query Object

A **query object** is an object which properties are meant to be query parameters.
You can create a **query object** by subclassing the `QueryObject` abstract class:

```csharp
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

Filters filters = new Filters
{
    Year = 1988,
    Genres = new[] { "action", "adventure" }
};

var result = UriTemplate.For("/api{?filters*}")
                        .WithParameter("filters", filters)
                        .ExpandToString();

result.ShouldBeEquivalentTo("/api?year=1988&genres=action,adventure");

```

You can choose the way to format your property name by specifying a `Func<string,string>` in the `Get` and `Set` methods.
You can also set a default format function for all your properties by calling the base constructor with a `Func<string,string>` (By default it will be snake_lower_case):

```csharp
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
```

`DoLess.UriTemplates` comes with default formatters, available in `DoLess.UriTemplates.Helpers.StringFormatters`:

* `ToLowerCamelCase` (myVariableName)
* `ToUpperCamelCase` (MyVariableName)
* `ToLowerSnakeCase` (my_variable_name)
* `ToUpperSnakeCase` (My_Variable_Name)
* `ToKebabCase` (my-variable-name)
* `ToTrainCase` (My-Variable-Name)
