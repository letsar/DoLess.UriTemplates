using System;
using System.Diagnostics;

namespace DoLess.UriTemplates.Entities
{
    [DebuggerDisplay("{Name}, {MaxLength}, {IsExploded}")]
    internal class VarSpec
    {
        public VarSpec(string name, int maxLength, bool isExploded, bool isConditional, bool isContinuation)
        {
            this.Name = name;
            this.MaxLength = Math.Max(0, maxLength);
            this.IsExploded = isExploded;
            this.IsConditional = isConditional;
            this.IsContinuation = isContinuation;
            this.HasBeenExpanded = false;
        }

        public string Name { get; }

        public int MaxLength { get; }

        public bool IsExploded { get; }

        public bool IsConditional { get; }

        public bool IsContinuation { get; }

        public bool HasBeenExpanded { get; set; }
    }
}
