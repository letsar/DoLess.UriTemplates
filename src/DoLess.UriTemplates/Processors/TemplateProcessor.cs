using System;
using System.Collections.Generic;
using System.Text;
using DoLess.UriTemplates.Entities;
using DoLess.UriTemplates.Helpers;

namespace DoLess.UriTemplates
{
    /// <summary>
    /// Represents a uri template parser as described in https://tools.ietf.org/html/rfc6570.
    /// </summary>
    internal class TemplateProcessor
    {
        private enum State
        {
            Literal,
            LiteralPercentEncoded,
            Operator,
            VarSpec,
            VarSpecPercentEncoded,
            VarSpecMaxLength,
            VarSpecExploded
        }

        private readonly string template;
        private readonly StringBuilder uriStringBuilder;
        private readonly StringBuilder varStringBuilder;
        private readonly char[] pctEncoded = { Constants.PercentChar, char.MinValue, char.MinValue };
        private readonly ExpressionProcessor expressionProcessor;

        // Global state.
        private State state;
        private int position;
        private char currentChar;

        // Pct-encoded state.
        private int pctEncodedLength;

        // VarSpec.
        private int varSpecMaxLength;
        private bool varSpecIsExploded;

        public TemplateProcessor(string template, IReadOnlyDictionary<string, object> variables, bool expandPartially, IValueFormatter valueFormatter)
        {
            this.template = template;

            this.uriStringBuilder = new StringBuilder(template.Length * 2);
            this.varStringBuilder = new StringBuilder();

            this.expressionProcessor = new ExpressionProcessor(variables, this.uriStringBuilder, expandPartially, valueFormatter);
        }

        public string Template => this.template;

        public int Position => this.position;

        public char CurrentChar => this.currentChar;

        private void Clear()
        {
            this.uriStringBuilder.Clear();
            this.ClearVarSpecFields();
        }

        private void ClearVarSpecFields()
        {
            this.varSpecMaxLength = 0;
            this.varSpecIsExploded = false;
            this.varStringBuilder.Clear();
        }

        public string Expand()
        {
            this.Clear();

            for (this.position = 0; this.position < this.template.Length; this.position++)
            {
                this.currentChar = this.template[this.position];

                this.ProcessState();
            }

            this.ProcessFinalStep();

            return this.uriStringBuilder.ToString();
        }

        private void ProcessState()
        {
            switch (this.state)
            {
                case State.Literal:
                    this.ProcessLiteral();
                    break;

                case State.LiteralPercentEncoded:
                    this.ProcessLiteralPercentEncoded();
                    break;

                case State.Operator:
                    this.ProcessOperator();
                    break;               

                case State.VarSpec:
                    this.ProcessVarSpec();
                    break;

                case State.VarSpecPercentEncoded:
                    this.ProcessVarSpecPercentEncoded();
                    break;

                case State.VarSpecMaxLength:
                    this.ProcessVarSpecMaxLength();
                    break;

                case State.VarSpecExploded:
                    this.ProcessVarSpecExploded();
                    break;

                default:
                    this.Throw("unexpected state");
                    break;
            }
        }

        private void ProcessLiteral()
        {
            switch (this.currentChar)
            {
                case Constants.ExpStart:
                    this.state = State.Operator;
                    break;

                case Constants.ExpEnd:
                    this.Throw("unexpected '}'");
                    break;

                case Constants.PercentChar:
                    this.state = State.LiteralPercentEncoded;
                    this.pctEncodedLength = 1;
                    break;

                default:
                    this.uriStringBuilder.Append(this.currentChar);
                    break;
            }
        }

        private void ProcessLiteralPercentEncoded()
        {
            if (this.currentChar.IsHexDigit())
            {
                this.pctEncoded[this.pctEncodedLength++] = this.currentChar;

                if (this.pctEncodedLength == PercentEncoding.PercentEncodeLength)
                {
                    // Full pct-encoded string, we can append it to the uri string builder as is.
                    this.uriStringBuilder.Append(this.pctEncoded);
                    this.pctEncodedLength = 0;
                    this.state = State.Literal;
                }
            }
            else
            {
                // The pct-encoded string contains illegal characters, we encode it.
                this.ProcessIllegalLiteralPercentEncoded();
            }
        }

        private void ProcessIllegalLiteralPercentEncoded()
        {
            this.EncodePctEncodedString();
            this.state = State.Literal;
        }

        private void EncodePctEncodedString()
        {
            // Encode the previous pct-encoded string.
            for (int i = 0; i < this.pctEncodedLength; i++)
            {
                this.uriStringBuilder.AppendEncoded(this.pctEncoded[i], true);
            }
        }

        private void ProcessOperator()
        {
            try
            {
                if (this.currentChar.IsOpNotSupported())
                {
                    this.ThrowOperatorNotSupported();
                }
                else
                {
                    switch (this.currentChar)
                    {
                        case '+':
                            this.expressionProcessor.StartExpression(ExpressionInfo.Reserved);
                            break;
                        case '.':
                            this.expressionProcessor.StartExpression(ExpressionInfo.Label);
                            break;
                        case '/':
                            this.expressionProcessor.StartExpression(ExpressionInfo.Path);
                            break;
                        case ';':
                            this.expressionProcessor.StartExpression(ExpressionInfo.Matrix);
                            break;
                        case '?':
                            this.expressionProcessor.StartExpression(ExpressionInfo.Query);
                            break;
                        case '&':
                            this.expressionProcessor.StartExpression(ExpressionInfo.Continuation);
                            break;
                        case '#':
                            this.expressionProcessor.StartExpression(ExpressionInfo.Fragment);
                            break;
                        default:
                            this.expressionProcessor.StartExpression(ExpressionInfo.Default);
                            this.varStringBuilder.Append(this.currentChar);
                            break;
                    }
                    this.state = State.VarSpec;
                }
            }
            catch (Exception ex)
            {
                this.Throw(ex.Message, ex);
            }
        }

        private void ProcessVarSpec()
        {
            switch (this.currentChar)
            {
                case Constants.PrefixModifier:
                    this.state = State.VarSpecMaxLength;
                    break;

                case Constants.ExplodeModifier:
                    this.state = State.VarSpecExploded;
                    break;

                case Constants.ExpEnd:
                    this.ProcessEndExpression();
                    break;

                case Constants.ExpStart:
                    this.Throw("unexpected '{'");
                    break;

                case Constants.PercentChar:
                    this.state = State.VarSpecPercentEncoded;
                    this.pctEncodedLength = 1;
                    break;

                case Constants.VariableSeparator:
                    this.ExpandVarSpec();
                    this.state = State.VarSpec;
                    break;

                default:
                    if (this.currentChar.IsValidVarSpecChar())
                    {
                        this.varStringBuilder.Append(this.currentChar);
                    }
                    else
                    {
                        this.Throw($"unexpected '{this.currentChar}' in variable name");
                    }
                    break;
            }
        }

        private void ProcessVarSpecPercentEncoded()
        {
            if (this.currentChar.IsHexDigit())
            {
                this.pctEncoded[this.pctEncodedLength++] = this.currentChar;

                if (this.pctEncodedLength == PercentEncoding.PercentEncodeLength)
                {
                    // Full pct-encoded string, we can append it to the uri string builder as is.
                    this.varStringBuilder.Append(this.pctEncoded);
                    this.pctEncodedLength = 0;
                    this.state = State.VarSpec;
                }
            }
            else
            {
                // The pct-encoded string contains illegal characters.
                this.Throw($"unexpected malformed percent-encoded char in variable name");
            }
        }

        private void ProcessVarSpecMaxLength()
        {
            if (this.currentChar.IsDigit())
            {
                // max-length    =  %x31-39 0*3DIGIT   ; positive integer < 10000.
                if (this.varSpecMaxLength > 999)
                {
                    this.Throw("max length must be < 10000");
                }

                this.varSpecMaxLength = this.varSpecMaxLength * 10 + (this.currentChar - '0');
            }
            else if (this.varSpecMaxLength == 0)
            {
                this.Throw("the max-length should be greater than 0");
            }
            else
            {
                this.ProcessEndVarSpec();
            }
        }

        private void ProcessVarSpecExploded()
        {
            this.varSpecIsExploded = true;
            this.ProcessEndVarSpec();
        }

        private void ProcessEndVarSpec()
        {
            switch (this.currentChar)
            {
                case Constants.ExpEnd:
                    this.ProcessEndExpression();
                    break;

                case Constants.VariableSeparator:
                    this.ExpandVarSpec();
                    this.state = State.VarSpec;
                    break;

                default:
                    this.Throw($"unexpected {this.currentChar} character");
                    break;
            }
        }

        private void ProcessEndExpression()
        {
            this.ExpandVarSpec();
            this.expressionProcessor.EndExpression();
            this.state = State.Literal;
        }

        private void ProcessVarSpecOptionalModifier(ref bool modifier)
        {
            modifier = true;
            this.ProcessEndVarSpecOptionalModifier();
        }

        private void ProcessEndVarSpecOptionalModifier()
        {
            switch (this.currentChar)
            {
                case Constants.ExpEnd:
                    this.ExpandVarSpec();
                    this.state = State.Literal;
                    break;

                case Constants.VariableSeparator:
                    this.ExpandVarSpec();
                    this.state = State.VarSpec;
                    break;

                case Constants.PrefixModifier:
                    this.state = State.VarSpecMaxLength;
                    break;

                case Constants.ExplodeModifier:
                    this.state = State.VarSpecExploded;
                    break;

                default:
                    this.Throw($"unexpected {this.currentChar} character");
                    break;
            }
        }

        private void ExpandVarSpec()
        {
            string varName = this.varStringBuilder.ToString();

            if (string.IsNullOrEmpty(varName))
            {
                this.Throw("empty var name");
            }

            VarSpec varSpec = new VarSpec(varName, this.varSpecMaxLength, this.varSpecIsExploded);

            this.ClearVarSpecFields();

            this.expressionProcessor.Expand(varSpec);
        }

        private void ProcessFinalStep()
        {
            switch (this.state)
            {
                case State.Literal:
                    // Nothing to do. This final state is expected.
                    break;

                case State.LiteralPercentEncoded:
                    this.ProcessIllegalLiteralPercentEncoded();
                    break;

                case State.Operator:
                case State.VarSpec:
                case State.VarSpecPercentEncoded:
                case State.VarSpecMaxLength:
                case State.VarSpecExploded:
                    this.position--;
                    this.Throw($"unexpected end of template");
                    break;

                default:
                    this.Throw("unexpected state");
                    break;
            }
        }

        private void Throw(string message, Exception innerException = null)
        {
            throw new UriTemplateParseException(message, this, innerException);
        }

        private void ThrowOperatorNotSupported()
        {
            throw new OperatorNotSupportedException(this.currentChar);
        }
    }
}
