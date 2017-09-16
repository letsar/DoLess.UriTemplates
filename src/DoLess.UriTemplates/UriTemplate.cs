using System;
using System.Collections.Generic;

namespace DoLess.UriTemplates
{
    /// <summary>
    /// Represents a uri template as described in https://tools.ietf.org/html/rfc6570.
    /// </summary>
    public class UriTemplate
    {
        private readonly Dictionary<string, object> variables;
        private readonly string template;

        private UriTemplate(string template, bool isParameterNameCaseSensitive)
        {
            this.template = template;
            this.variables = new Dictionary<string, object>(isParameterNameCaseSensitive ? null : StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the provided template.
        /// </summary>
        public string Template => this.template;

        /// <summary>
        /// Gets the parameter names already provided.
        /// </summary>
        public IReadOnlyCollection<string> ParameterNames => this.variables.Keys;

        /// <summary>
        /// Creates a new <see cref="UriTemplate"/> with the specified template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="isParameterNameCaseSensitive">Indicates whether two parameters with the same name but different cases should be the same (default <code>true</code>).</param>
        /// <returns></returns>
        public static UriTemplate For(string template, bool isParameterNameCaseSensitive = true)
        {
            return new UriTemplate(template, isParameterNameCaseSensitive);
        }

        /// <summary>
        /// Remove the specified parameter.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns></returns>
        public UriTemplate WithoutParameter(string name)
        {
            this.variables.Remove(name);
            return this;
        }

        /// <summary>
        /// Sets the parameter's value.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value associated with.</param>
        /// <returns></returns>
        public UriTemplate WithParameter(string name, object value)
        {
            this.variables[name] = value;
            return this;
        }

        /// <summary>
        /// Sets the parameter's values (as an array).
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="values">The values associated with.</param>
        /// <returns></returns>
        public UriTemplate WithParameter(string name, params object[] values)
        {
            this.variables[name] = values;
            return this;
        }

        /// <summary>
        /// Expands the variables and returns the resulting <see cref="string"/>.
        /// </summary>
        /// <param name="expandPartially">Indicates whether undefined variables should be ignored or let as is.</param>
        /// <returns></returns>
        public string ExpandToString(bool expandPartially = false)
        {
            TemplateProcessor templateProcessor = new TemplateProcessor(this.template, this.variables, expandPartially);
            return templateProcessor.Expand();
        }

        /// <summary>
        /// Expands the variables and returns the resulting <see cref="Uri"/>.
        /// </summary>
        /// <param name="expandPartially">Indicates whether undefined variables should be ignored or let as is.</param>
        /// <returns></returns>
        public Uri ExpandToUri(bool expandPartially = false)
        {
            return new Uri(this.ExpandToString(expandPartially));
        }

        /// <summary>
        /// Returns the template.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.template;
        }
    }
}
