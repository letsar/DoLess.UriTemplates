using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DoLess.UriTemplates.Helpers;

namespace DoLess.UriTemplates
{
    /// <summary>
    /// Represents an object where properties are part of a query string.
    /// </summary>
    public abstract class QueryObject : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly Dictionary<string, object> properties;
        private readonly Func<string, string> defaultStringFormatter;

        /// <summary>
        /// Creates a new <see cref="QueryObject"/>.
        /// </summary>
        protected QueryObject(Func<string, string> defaultStringFormatter = null)
        {
            this.properties = new Dictionary<string, object>();
            this.defaultStringFormatter = defaultStringFormatter ?? StringFormatters.ToLowerSnakeCase;
        }

        public int Count => this.properties.Count;

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => this.properties.GetEnumerator();

        IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Gets the value for the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected T Get<T>([CallerMemberName] string key = null)
        {
            return this.Get<T>(this.defaultStringFormatter, key);
        }

        /// <summary>
        /// Gets the value for the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="stringFormatter">The function used to format the key into another case convention.</param>
        /// <param name="key">The key.</param>
        protected T Get<T>(Func<string, string> stringFormatter, [CallerMemberName] string key = null)
        {
            this.properties.TryGetValue(this.GetKey(key, stringFormatter), out object value);
            return (T)value;
        }

        /// <summary>
        /// Sets the value for the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="key">The key.</param>
        protected void Set<T>(T value, [CallerMemberName] string key = null)
        {
            this.Set<T>(this.defaultStringFormatter, value, key);
        }

        /// <summary>
        /// Sets the value for the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="stringFormatter">The function used to format the key into another case convention.</param>
        /// <param name="value">The value.</param>
        /// <param name="key">The key.</param>
        protected void Set<T>(Func<string, string> stringFormatter, T value, [CallerMemberName] string key = null)
        {
            if (value == null)
            {
                this.properties.Remove(this.GetKey(key, stringFormatter));
            }
            else
            {
                this.properties[this.GetKey(key, stringFormatter)] = value;
            }
        }

        private string GetKey(string key, Func<string, string> stringFormatter)
        {
            return stringFormatter == null ? key : stringFormatter(key);
        }
    }
}
