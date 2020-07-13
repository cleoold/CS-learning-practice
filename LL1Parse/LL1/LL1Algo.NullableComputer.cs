using System.Linq;
using System.Collections.Generic;

namespace LL1Parse
{
    using NullableTable = Dictionary<Symbol, bool>;

    partial class LL1Algo
    {
        /// <summary>
        /// compute nullable for all symbols in NonTerminal, and arbitrary
        /// sequence of symbols
        /// </summary>
        public class NullableComputer : AbstractComputer<bool>
        {
            public readonly List<NullableTable> Iterations;

            public override NullableTable Result { get => Iterations.Last(); }

            private NullableComputer(LL1Algo algo, List<NullableTable> iterations)
                : base(algo)
            {
                Iterations = iterations;
            }

            public static NullableComputer Compute(LL1Algo algo)
            {
                var iterations = new List<NullableTable>();

                var nullable = new NullableTable();
                // initialize first iteration
                foreach (var A in algo.Thecfg.Nonterminals)
                    nullable.Add(A, false);

                do
                {
                    if (iterations.Count == 0)
                        iterations.Add(nullable);
                    else
                        iterations.Add(new NullableTable(nullable));
                    nullable = iterations.Last();
                    foreach (var prod in algo.Thecfg.Productions)
                    {
                        if (prod.DerivesToNull ||
                            prod.Rhs.All(e => e.Type == Symbol.Types.NonTerminal && nullable[e]))
                            nullable[prod.Lhs] = true;
                    }
                // stop loop if no changes between iterations
                } while (iterations.Count == 1 ||
                    nullable.Except(iterations[iterations.Count-2]).Any());

                return new NullableComputer(algo, iterations);
            }

            // same algorithm as above
            public bool Sequence(IEnumerable<Symbol> syms)
            {
                return syms.Count() == 0 ||
                    syms.All(e => e.Type == Symbol.Types.NonTerminal && this[e]);
            }
        }
    }
}
