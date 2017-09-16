using System;
using System.Diagnostics;

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
            this.HasBeenExpanded = false;
        }

        public string Name { get; }

        public int MaxLength { get; }

        public bool IsExploded { get; }

        public bool HasBeenExpanded { get; set; }
    }
}
