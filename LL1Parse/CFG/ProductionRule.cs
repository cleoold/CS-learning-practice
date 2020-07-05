using System.Text;
using System.Collections.Generic;

namespace LL1Parse
{
    class ProductionRule
    {
        // reserved symbols. special meanings
        public const string Arrow = "->";
        public const string Union = "|";

        // lhs must be non terminal
        public readonly Symbol Lhs;
        public readonly List<Symbol> Rhs;

        public bool DerivesToNull
        {
            get => Rhs.Count == 0;
        }

        /// <exception cref="InvalidSymbolException"></exception>
        public ProductionRule(Symbol lhs, List<Symbol> rhs)
        {
            if (lhs?.Type != Symbol.Types.NonTerminal)
                throw new InvalidSymbolException();
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
