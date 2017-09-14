using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DoLess.UriTemplates.Entities;

namespace DoLess.UriTemplates
{
    internal class ExpressionProcessor
    {
        private readonly IReadOnlyDictionary<string, object> variables;
        private readonly StringBuilder builder;
        private ExpressionInfo expressionInfo;
        private int startLength;

        public ExpressionProcessor(IReadOnlyDictionary<string, object> variables, StringBuilder builder)
        {
            this.variables = variables;
            this.builder = builder;
        }

        public void StartExpression(ExpressionInfo expressionInfo)
        {
            this.expressionInfo = expressionInfo;
            this.startLength = this.builder.Length;
        }

        public void Expand(VarSpec varSpec)
        {
            if (this.variables.TryGetValue(varSpec.Name, out object value) && value != null)
            {
                bool isStart = this.builder.Length == this.startLength;
                if (isStart && IsDefined(value))
                {
                    this.builder.Append(this.expressionInfo.First);
                }
                else if (!isStart)
                {
                    this.builder.Append(this.expressionInfo.Separator);
                }

                this.Expand(varSpec, value);
            }
            else
            {
                varSpec.HasBeenExpanded = false;
            }
        }

        private static bool IsDefined(object value)
        {
            switch (value)
            {
                case IEnumerable enumerable:
                    return enumerable.Any();
                case null:
                    return false;
                default:
                    return true;
            }
        }

        private void Expand(VarSpec varSpec, string value)
        {
            this.ExpandName(varSpec, value == string.Empty);
            this.ExpandStringValue(value, varSpec.MaxLength);
        }

        private void Expand(VarSpec varSpec, IEnumerable values)
        {
            bool isEmpty = !values.Any();
            this.Expand(varSpec, isEmpty, values, this.ExpandValues);
        }

        private void Expand(VarSpec varSpec, IDictionary<string, string> values)
        {
            bool isEmpty = values.Count == 0;
            this.Expand(varSpec, isEmpty, values, this.ExpandValues);
        }

        private void Expand(VarSpec varSpec, IReadOnlyDictionary<string, string> values)
        {
            bool isEmpty = values.Count == 0;
            this.Expand(varSpec, isEmpty, values, this.ExpandValues);
        }

        private void ExpandValues(IEnumerable values, VarSpec varSpec, char separator)
        {
            var isNamedAndExploded = varSpec.IsExploded && this.expressionInfo.IsNamed;
            var name = varSpec.Name;

            foreach (var value in values)
            {
                if (isNamedAndExploded)
                {
                    this.builder.AppendEncoded(name, this.expressionInfo.AllowReserved);
                    this.builder.Append('=');
                }

                this.builder.AppendEncoded(this.ValueToString(value), this.expressionInfo.AllowReserved);
                this.builder.Append(separator);
            }
        }

        private void ExpandValues(IReadOnlyDictionary<string, string> values, VarSpec varSpec, char separator)
        {
            var isExploded = varSpec.IsExploded;

            var name = varSpec.Name;
            var pairSeparator = isExploded ? '=' : ',';

            foreach (var key in values.Keys)
            {
                this.builder.AppendEncoded(key, this.expressionInfo.AllowReserved);
                this.builder.Append(pairSeparator);
                this.builder.AppendEncoded(values[key], this.expressionInfo.AllowReserved);
                this.builder.Append(separator);
            }
        }

        private void ExpandValues(IDictionary<string, string> values, VarSpec varSpec, char separator)
        {
            var isExploded = varSpec.IsExploded;

            var name = varSpec.Name;
            var pairSeparator = isExploded ? '=' : ',';

            foreach (var key in values.Keys)
            {
                this.builder.AppendEncoded(key, this.expressionInfo.AllowReserved);
                this.builder.Append(pairSeparator);
                this.builder.AppendEncoded(values[key], this.expressionInfo.AllowReserved);
                this.builder.Append(separator);
            }
        }

        private void Expand<T>(VarSpec varSpec, bool isEmpty, T values, Action<T, VarSpec, char> expandValues)
        {
            if (varSpec.MaxLength > 0)
            {
                // A list cannot be prefixed.
                this.ThrowNotSuitablePrefixException(varSpec);
            }

            var isExploded = varSpec.IsExploded;
            char separator = isExploded ? this.expressionInfo.Separator : ',';

            if (!isExploded && !isEmpty)
            {
                this.ExpandName(varSpec, isEmpty);
            }

            expandValues(values, varSpec, separator);

            if (!isEmpty)
            {
                this.builder.RemoveLastChar();
            }
        }

        private void Expand(VarSpec varSpec, object value)
        {
            switch (value)
            {
                case string val:
                    this.Expand(varSpec, val);
                    break;

                case IDictionary<string, string> val:
                    this.Expand(varSpec, val);
                    break;

                case IReadOnlyDictionary<string, string> val:
                    this.Expand(varSpec, val);
                    break;

                case IEnumerable val:
                    this.Expand(varSpec, val);
                    break;

                default:
                    // If all above fails, convert the object to its string representation.
                    this.Expand(varSpec, this.ValueToString(value));
                    break;
            }
        }

        private void ExpandName(VarSpec varSpec, bool isValueEmpty)
        {
            if (this.expressionInfo.IsNamed)
            {
                this.builder.Append(varSpec.Name);

                if (isValueEmpty)
                {
                    this.builder.Append(this.expressionInfo.IfEmpty);
                }
                else
                {
                    this.builder.Append('=');
                }
            }
        }

        private void ExpandStringValue(string value, int maxLength = 0)
        {
            if (maxLength > 0 && maxLength < value.Length)
            {
                value = value.Substring(0, maxLength);
            }

            this.builder.AppendEncoded(value, this.expressionInfo.AllowReserved);
        }

        private string ValueToString(object value)
        {
            var result = Convert.ToString(value, CultureInfo.InvariantCulture);
            return result;
        }

        private void ThrowNotSuitablePrefixException(VarSpec varSpec)
        {
            throw new NotSuitablePrefixException(varSpec.Name);
        }
    }
}
