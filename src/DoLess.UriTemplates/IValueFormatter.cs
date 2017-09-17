namespace DoLess.UriTemplates
{
    /// <summary>
    /// Represents an object that can transforms an <see cref="object"/> into a <see cref="string"/>.
    /// </summary>
    public interface IValueFormatter
    {
        string Format(object value);
    }
}
