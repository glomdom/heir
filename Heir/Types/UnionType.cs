﻿namespace Heir.Types
{
    public class UnionType(List<BaseType> types) : BaseType
    {
        public override TypeKind Kind => TypeKind.Union;
        public List<BaseType> Types { get; } = types;

        public override string ToString(bool colors = false) =>
            string.Join(" | ", new HashSet<string>(Types.ConvertAll(t => t.ToString(colors))))
            .Replace(" | none", "?");
    }
}