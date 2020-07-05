using System.Text;
using System.Collections.Generic;

namespace LL1Parse
{
    class ProductionRule
    {
        // reserved symbols. special meanings
        public const string Arrow = "->";
        public const string Union = "|";

        public readonly Symbol Lhs;
        public readonly List<Symbol> Rhs;

        public bool DerivesToNull
        {
            get => Rhs.Count == 0;
        }

        public ProductionRule(Symbol lhs, List<Symbol> rhs)
        {
            Lhs = lhs;
            Rhs = rhs;
        }

        public override string ToString()
        {
            var sb = new StringBuilder($"{Lhs.Name} {Arrow}");
            foreach (var s in Rhs)
                sb.Append($" {s.Name}");
            return sb.ToString();
        }
    }
}
