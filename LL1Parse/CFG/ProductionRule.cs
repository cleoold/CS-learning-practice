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

        // key to identify
        public int Number { get; } = -1;

        public bool DerivesToNull
        {
            get => Rhs.Count == 0;
        }

        /// <exception cref="InvalidSymbolException"></exception>
        public ProductionRule(Symbol lhs, List<Symbol> rhs)
        {
            if (lhs.Type != Symbol.Types.NonTerminal)
                throw new InvalidSymbolException("lhs of production must be one non-terminal");
            Lhs = lhs;
            Rhs = rhs;
        }

        /// <exception cref="InvalidArgumentException"></exception>
        public ProductionRule(Symbol lhs, List<Symbol> rhs, int number) : this(lhs, rhs)
        {
            if (number < 0)
                throw new InvalidArgumentException("number cannot be negative");
            this.Number = number;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Number != -1) sb.Append($"{Number}. ");
            sb.Append($"{Lhs.Name} {Arrow}");
            foreach (var s in Rhs)
                sb.Append($" {s.Name}");
            return sb.ToString();
        }
    }
}
