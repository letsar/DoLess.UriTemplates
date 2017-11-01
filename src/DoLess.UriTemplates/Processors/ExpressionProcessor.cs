using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoLess.UriTemplates.Entities;
using DoLess.UriTemplates.Helpers;
using DoLess.UriTemplates.ValueFormatters;

namespace DoLess.UriTemplates
{
    internal class ExpressionProcessor
    {
        private static readonly IValueFormatter ValueFormatter = new DefaultValueFormatter();

        private readonly IReadOnlyDictionary<string, object> variables;
        private readonly StringBuilder builder;
        private readonly List<VarSpec> unexpandedVariables;
        private readonly IValueFormatter valueFormatter;
        private ExpressionInfo expressionInfo;
        private int startLength;
        private bool expandPartially;
        private int definedVariables;

        public ExpressionProcessor(IReadOnlyDictionary<string, object> variables, StringBuilder builder, bool expandPartially, IValueFormatter valueFormatter)
        {
            this.variables = variables;
            this.builder = builder;
            this.expandPartially = expandPartially;
            this.unexpandedVariables = new List<VarSpec>();
            this.valueFormatter = valueFormatter ?? ValueFormatter;
        }

        public void StartExpression(ExpressionInfo expressionInfo)
        {
            this.expressionInfo = expressionInfo;
            this.startLength = this.builder.Length;
            this.Clear();
        }

        public void EndExpression()
        {
            this.AppendUnexpandedVariables();
        }

        public void Clear()
        {
            this.unexpandedVariables.Clear();
            this.definedVariables = 0;
        }

        public void Expand(VarSpec varSpec)
        {
            if (this.variables.TryGetValue(varSpec.Name, out object value) && value != null)
            {
                this.definedVariables++;
                if (this.expandPartially && this.expressionInfo != ExpressionInfo.Query)
                {
                    this.AppendUnexpandedVariables();
                }

                if (this.IsStart() && IsDefined(value))
                {
                    this.builder.Append(this.expressionInfo.First);
                }
                else if (!this.IsStart())
                {
                    this.builder.Append(this.expressionInfo.Separator);
                }

                this.Expand(varSpec, value);
            }
            else
            {
                if (this.expandPartially)
                {
                    this.unexpandedVariables.Add(varSpec);
                }
            }
        }

        private bool IsStart()
        {
            return this.builder.Length == this.startLength;
        }

        private void AppendUnexpandedVariables()
        {
            var count = this.unexpandedVariables.Count;

            if (count > 0)
            {
                if (!this.expressionInfo.SupportsPartialExpandInMulti && this.definedVariables > 0)
                {
                    this.ThrowPartialExpandException();
                }

                string op = this.expressionInfo.OpCode;

                op = this.expressionInfo == ExpressionInfo.Query && this.definedVariables > 0 ?
                     ExpressionInfo.Continuation.OpCode :
                     this.expressionInfo.OpCode;


                this.builder.Append(Constants.ExpStart);
                this.builder.Append(op);
                this.builder.Append(string.Join(",", this.unexpandedVariables.Select(x => x.ToString())));
                this.builder.Append(Constants.ExpEnd);

                this.unexpandedVariables.Clear();
            }
        }

        private static bool IsDefined(object value)
        {
            switch (value)
            {
                case QueryObject queryObject:
                    return queryObject.Count > 0;
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

        private void Expand(VarSpec varSpec, IEnumerable<KeyValuePair<string, string>> values)
        {
            bool isEmpty = !values.Any();
            this.Expand(varSpec, isEmpty, values, this.ExpandValues);
        }

        private void Expand(VarSpec varSpec, IEnumerable<KeyValuePair<string, object>> values)
        {
            bool isEmpty = !values.Any();
            this.Expand(varSpec, isEmpty, values, this.ExpandValues);
        }

        private void Expand(VarSpec varSpec, QueryObject values)
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

        private void ExpandValues(IEnumerable<KeyValuePair<string, string>> values, VarSpec varSpec, char separator)
        {
            var isExploded = varSpec.IsExploded;

            var name = varSpec.Name;
            var pairSeparator = isExploded ? '=' : ',';

            foreach (var pair in values)
            {
                this.builder.AppendEncoded(pair.Key, this.expressionInfo.AllowReserved);
                this.builder.Append(pairSeparator);
                this.builder.AppendEncoded(pair.Value, this.expressionInfo.AllowReserved);
                this.builder.Append(separator);
            }
        }

        private void ExpandValues(IEnumerable<KeyValuePair<string, object>> values, VarSpec varSpec, char separator)
        {
            var isExploded = varSpec.IsExploded;

            var name = varSpec.Name;
            var pairSeparator = isExploded ? '=' : ',';

            foreach (var pair in values)
            {
                this.builder.AppendEncoded(pair.Key, this.expressionInfo.AllowReserved);
                this.builder.Append(pairSeparator);

                if (!(pair.Value is string) && pair.Value is IEnumerable innerValues)
                {
                    bool hasValue = false;
                    foreach (var innerValue in innerValues)
                    {
                        this.builder.AppendEncoded(this.ValueToString(innerValue), this.expressionInfo.AllowReserved);
                        this.builder.Append(',');
                        hasValue = true;
                    }
                    if (hasValue)
                    {
                        this.builder.RemoveLastChar();
                    }

                }
                else
                {
                    this.builder.AppendEncoded(this.ValueToString(pair.Value), this.expressionInfo.AllowReserved);
                }

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

                case QueryObject val:
                    this.Expand(varSpec, val);
                    break;

                case IEnumerable<KeyValuePair<string, object>> val:
                    this.Expand(varSpec, val);
                    break;

                case IEnumerable<KeyValuePair<string, string>> val:
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
            if (value is string stringValue)
            {
                return stringValue;
            }
            else
            {
                return this.valueFormatter.Format(value);
            }
        }

        private void ThrowNotSuitablePrefixException(VarSpec varSpec)
        {
            throw new NotSuitablePrefixException(varSpec.Name);
        }

        private void ThrowPartialExpandException()
        {
            throw new UriTemplateException("Partial expand is not allowed for this expression");
        }
    }
}
