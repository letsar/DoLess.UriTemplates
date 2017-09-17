using System;
using System.Collections.Generic;
using DoLess.UriTemplates.ValueFormatters;

namespace DoLess.UriTemplates
{
    /// <summary>
    /// Represents a uri template as described in https://tools.ietf.org/html/rfc6570.
    /// </summary>
    public class UriTemplate
    {
        private readonly Dictionary<string, object> variables;
        private readonly string template;
        private IValueFormatter valueFormatter;
        private bool isPartialExpand;

        private UriTemplate(string template, bool isParameterNameCaseSensitive)
        {
            this.template = template;
            this.variables = new Dictionary<string, object>(isParameterNameCaseSensitive ? null : StringComparer.OrdinalIgnoreCase);
            this.isPartialExpand = false;
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
        /// Indicates whether the undefined variables should be let as is or removed.
        /// </summary>
        /// <param name="isPartialExpand">The new value.</param>
        /// <returns></returns>
        public UriTemplate WithPartialExpand(bool isPartialExpand = true)
        {
            this.isPartialExpand = isPartialExpand;
            return this;
        }

        /// <summary>
        /// Sets the value formatter.
        /// </summary>
        /// <param name="valueFormatter">The value formatter.</param>
        /// <returns></returns>
        /// <remarks>
        /// Setting the <paramref name="valueFormatter"/> to null, resets to the default value formatter.
        /// </remarks>
        public UriTemplate WithValueFormatter(IValueFormatter valueFormatter)
        {
            this.valueFormatter = valueFormatter;
            return this;
        }

        /// <summary>
        /// Sets the value formatter.
        /// </summary>
        /// <param name="func">The value formatter.</param>
        /// <returns></returns>
        public UriTemplate WithValueFormatter(Func<object, string> func)
        {
            this.valueFormatter = new DelegatingValueFormatter(func);
            return this;
        }

        /// <summary>
        /// Expands the variables and returns the resulting <see cref="string"/>.
        /// </summary>     
        /// <returns></returns>
        public string ExpandToString()
        {
            TemplateProcessor templateProcessor = new TemplateProcessor(this.template, this.variables, this.isPartialExpand, this.valueFormatter);
            return templateProcessor.Expand();
        }

        /// <summary>
        /// Expands the variables and returns the resulting <see cref="Uri"/>.
        /// </summary>      
        /// <returns></returns>
        public Uri ExpandToUri()
        {
            return new Uri(this.ExpandToString());
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
