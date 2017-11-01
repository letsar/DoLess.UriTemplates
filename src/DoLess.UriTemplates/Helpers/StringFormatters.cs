using System;
using System.Text;

namespace DoLess.UriTemplates.Helpers
{
    /// <summary>
    /// Contains string formatters that can be useful for a <see cref="QueryObject"/>.
    /// </summary>
    public static class StringFormatters
    {
        /// <summary>
        /// Returns a series of words, with each word started with a capital letter.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        /// <remarks>
        /// UpperCamelCase
        /// </remarks>
        public static string ToUpperCamelCase(string source)
        {
            return Format
            (
                source,
                (c, _) => new[] { char.ToUpperInvariant(c) }                
            );
        }

        /// <summary>
        /// Returns a series of words, with each intermediate word started with a capital letter.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        /// <remarks>
        /// lowerCamelCase
        /// </remarks>
        public static string ToLowerCamelCase(string source)
        {
            return Format
            (
                source,
                (c, isFirstChar) =>
                {
                    if (isFirstChar)
                    {
                        return new[] { char.ToLowerInvariant(c) };
                    }
                    else
                    {
                        return new[] { char.ToUpperInvariant(c) };
                    }
                }
            );
        }

        /// <summary>
        /// Returns a series of words separated with an underscore (_). All words start with a lower case.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        /// <remarks>
        /// lower_snake_case
        /// </remarks>
        public static string ToLowerSnakeCase(string source)
        {
            return Format
            (
                source,
                (c, isFirstChar) =>
                {
                    if (isFirstChar)
                    {
                        return new[] { char.ToLowerInvariant(c) };
                    }
                    else
                    {
                        return new[] { '_', char.ToLowerInvariant(c) };
                    }
                }
            );
        }

        /// <summary>
        /// Returns a series of words separated with an underscore (_). All words start with a capital letter.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        /// <remarks>
        /// Upper_Snake_Case
        /// </remarks>
        public static string ToUpperSnakeCase(string source)
        {
            return Format
            (
                source,
                (c, isFirstChar) =>
                {
                    if (isFirstChar)
                    {
                        return new[] { char.ToUpperInvariant(c) };
                    }
                    else
                    {
                        return new[] { '_', char.ToUpperInvariant(c) };
                    }
                }
            );
        }

        /// <summary>
        /// Returns a series of words separated with a hyphen (-). All words start with a lower case.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        /// <remarks>
        /// kebab-case
        /// </remarks>
        public static string ToKebabCase(string source)
        {
            return Format
            (
                source,
                (c, isFirstChar) =>
                {
                    if (isFirstChar)
                    {
                        return new[] { char.ToLowerInvariant(c) };
                    }
                    else
                    {
                        return new[] { '-', char.ToLowerInvariant(c) };
                    }
                }
            );
        }

        /// <summary>
        /// Returns a series of words separated with a hyphen (-). All words start with a capital letter.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        /// <remarks>
        /// Train-Case
        /// </remarks>
        public static string ToTrainCase(string source)
        {
            return Format
            (
                source,
                (c, isFirstChar) =>
                {
                    if (isFirstChar)
                    {
                        return new[] { char.ToUpperInvariant(c) };
                    }
                    else
                    {
                        return new[] { '-', char.ToUpperInvariant(c) };
                    }
                }
            );
        }

        private static string Format(string source, Func<char, bool, char[]> delimiterFunc)
        {
            if (source == null)
            {
                return null;
            }

            var stringBuilder = new StringBuilder();
            var isNewWord = true;
            var isFirstChar = true;

            for (int i = 0; i < source.Length; i++)
            {
                var c = source[i];
                if (!char.IsLetterOrDigit(c))
                {
                    // If not a letter nor a digit, we assumes it's a delimiter.
                    isNewWord = true;
                }
                else
                {
                    // It is a letter or a digit.
                    if (isNewWord || char.IsUpper(c))
                    {
                        stringBuilder.Append(delimiterFunc(c, isFirstChar));
                        isNewWord = false;
                        isFirstChar = false;
                    }
                    else
                    {
                        stringBuilder.Append(c);
                    }
                }
            }

            return stringBuilder.ToString();
        }
    }
}
