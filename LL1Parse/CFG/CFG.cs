using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LL1Parse
{
    class CFG
    {
        public readonly HashSet<Symbol> Symbols;
        public readonly Symbol StartSymbol;
        public readonly List<ProductionRule> Productions;

        public IEnumerable<Symbol> Nonterminals
        {
            get => Symbols.Where(e => e.Type == Symbol.Types.NonTerminal);
        }

        public IEnumerable<Symbol> Terminals
        {
            get => Symbols.Where(e => e.Type == Symbol.Types.Terminal);
        }

        private CFG(HashSet<Symbol> symbols, Symbol startSymbol, List<ProductionRule> productions)
        {
            Symbols = symbols;
            StartSymbol = startSymbol;
            Productions = productions;
        }

        /// <summary>
        /// constructs CFG based on lines of productions. N, T sets are inferred properly
        /// each production has the form "A -> B C D" where A is lhs, B C D is rhs,
        /// -> is arrow that is ignored. "A ->" implies rhs is epsilon. any redundant
        /// whitespaces are omitted. symbol "|" is reserved.
        /// </summary>
        /// <exception cref="InvalidCFGInputException"></exception>
        public static CFG FromProductionStrings(IEnumerable<string> lines)
        {
            var allsyms = new HashSet<string>();
            var nontermsyms = new HashSet<string>();
            Symbol start;
            var prods = new List<ProductionRule>();

            var processed = TokenizeProductionStrings(lines);

            // assume the first lhs is starting symbol
            if (processed.Count != 0)
                start = new Symbol(processed[0][0], Symbol.Types.NonTerminal);
            else
                throw new InvalidCFGInputException("expects productions");

            foreach (var line in processed)
            {
                nontermsyms.Add(line[0]);
                allsyms.Add(line[0]);
                for (int i = 2; i < line.Length; ++i)
                    allsyms.Add(line[i]);
            }
            int key = 0;
            foreach (var line in processed)
            {
                var lhs = new Symbol(line[0], Symbol.Types.NonTerminal);
                var rhs = new List<Symbol>();
                for (int i = 2; i < line.Length; ++i)
                {
                    rhs.Add(new Symbol(line[i],
                        nontermsyms.Contains(line[i]) ?
                        Symbol.Types.NonTerminal : Symbol.Types.Terminal
                    ));
                }
                prods.Add(new ProductionRule(lhs, rhs, key++));
            }

            return new CFG(
                allsyms.Select(e => new Symbol(e,
                    nontermsyms.Contains(e) ?
                    Symbol.Types.NonTerminal : Symbol.Types.Terminal
                )).ToHashSet(),
                start, prods
            );
        }

        /// <summary>
        /// given a list of production strings, does 3 things:
        /// 1. split strings by space to create tokens
        /// 2. extra whitespaces discarded. eg "A  -> B  C " is same as "A -> B C"
        /// 3. if rhs contains union, they are split into multiple rules. eg
        ///    "A -> B C | D" is same as consecutive "A -> B C" and "A -> D"
        /// </summary>
        /// <exception cref="InvalidCFGInputException"></exception>
        private static List<string[]> TokenizeProductionStrings(IEnumerable<string> lines)
        {
            var result = new List<string[]>();

            var pretokens = lines.Select(
                e => e.Split(' ').Where(e => e != " " && e != "").ToArray()
            );
            if (! pretokens.All(e => e.Length >= 2 && e[1] == ProductionRule.Arrow))
                throw new InvalidCFGInputException("invalid production format");

            foreach (var line in pretokens)
            {
                var front = line.Take(2).ToList();
                var rhs = line.Skip(2);
                var rhsPart = new List<string>();
                foreach (var token in rhs)
                {
                    if (token == ProductionRule.Union)
                    {
                        result.Add(front.Concat(rhsPart).ToArray());
                        rhsPart.Clear();
                        continue;
                    }
                    rhsPart.Add(token);
                }
                result.Add(front.Concat(rhsPart).ToArray());
            }

            return result;
        }

        public bool TryQueryTypedSymbolByName(string name, [NotNullWhen(true)] out Symbol? value)
        {
            var newSymbol = new Symbol(name, Symbol.Types.NonTerminal);
            if (Symbols.Contains(newSymbol)
                || Symbols.Contains(newSymbol = new Symbol(name, Symbol.Types.Terminal)))
            {
                value = newSymbol;
                return true;
            }
            value = null;
            return false;
        }
    }
}
