namespace DoLess.UriTemplates
{
    public class NotSuitablePrefixException : UriTemplateException
    {
        public NotSuitablePrefixException(string varName)
            : base($"The variable {varName} is an enumerable and cannot be prefixed")
        {
        }
    }
}
