using System;
using System.Diagnostics;
using DoLess.UriTemplates.Helpers;

namespace DoLess.UriTemplates.Entities
{
    [DebuggerDisplay("{Name}, {MaxLength}, {IsExploded}")]
    internal class VarSpec
    {
        public VarSpec(string name, int maxLength, bool isExploded)
        {
            this.Name = name;
            this.MaxLength = Math.Max(0, maxLength);
            this.IsExploded = isExploded;
        }

        public string Name { get; }

        public int MaxLength { get; }

        public bool IsExploded { get; }

        public override string ToString()
        {
            if (this.IsExploded)
            {
                return this.Name + Constants.ExplodeModifier;
            }
            else if (this.MaxLength > 0)
            {
                return this.Name + Constants.PrefixModifier + this.MaxLength.ToString();
            }
            else
            {
                return this.Name;
            }
        }
    }
}
