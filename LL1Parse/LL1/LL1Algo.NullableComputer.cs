using System.Linq;
using System.Collections.Generic;

namespace LL1Parse
{
    using NullableType = Dictionary<Symbol, bool>;

    partial class LL1Algo
    {
        /// <summary>
        /// compute nullable for all symbols in NonTerminal
        /// </summary>
        public class NullableComputer
        {
            public readonly List<NullableType> Iterations;

            public NullableType Result
            {
                get => Iterations.Last();
            }

            private NullableComputer(List<NullableType> iterations)
            {
                Iterations = iterations;
            }

            public static NullableComputer Compute(LL1Algo algo)
            {
                var iterations = new List<NullableType>();

                var nullable = new NullableType();
                // initialize first iteration
                foreach (var A in algo.thecfg.Nonterminals)
                    nullable.Add(A, false);

                do
                {
                    if (iterations.Count == 0)
                        iterations.Add(nullable);
                    else
                        iterations.Add(new NullableType(nullable));
                    nullable = iterations.Last();
                    foreach (var prod in algo.thecfg.Productions)
                    {
                        if (prod.DerivesToNull ||
                            prod.Rhs.All(e => e.Type == Symbol.Types.NonTerminal && nullable[e]))
                            nullable[prod.Lhs] = true;
                    }
                // stop loop if no changes between iterations
                } while (iterations.Count == 1 ||
                    nullable.Except(iterations[iterations.Count-2]).Any());

                return new NullableComputer(iterations);
            }
        }
    }
}
