using System.Linq;
using System.Collections.Generic;

namespace LL1Parse
{
    using FirstType = Dictionary<Symbol, HashSet<Symbol>>;

    partial class LL1Algo
    {
        /// <summary>
        /// compute first set for all symbols in NonTerminal
        /// and for a sequence of arbitrary symbols
        /// </summary>
        public class FirstComputer
        {
            public readonly LL1Algo Algo;

            public readonly List<FirstType> Iterations;

            public FirstType Result
            {
                get => Iterations.Last();
            }

            public FirstComputer(LL1Algo algo, List<FirstType> iterations)
            {
                Algo = algo;
                Iterations = iterations;
            }

            public static FirstComputer Compute(LL1Algo algo)
            {
                var iterations = new List<FirstType>();

                var first = new FirstType();
                // initialize first iteration
                foreach (var A in algo.thecfg.Nonterminals)
                    first.Add(A, new HashSet<Symbol>());
                do
                {
                    if (iterations.Count == 0)
                        iterations.Add(first);
                    else
                        iterations.Add(DictUtil.Clone(first));
                    first = iterations.Last();
                    foreach (var prod in algo.thecfg.Productions)
                    {
                        foreach (var Bi in prod.Rhs)
                        {
                            if (Bi.Type == Symbol.Types.Terminal)
                            {
                                first[prod.Lhs].Add(Bi);
                                break;
                            }
                            else
                            {
                                first[prod.Lhs].UnionWith(first[Bi]);
                            }
                            if (!algo.Nullable.Result[Bi])
                                break;
                        }
                    }
                // stop loop if no changes between iterations
                } while (iterations.Count == 1 ||
                    !DictUtil.Equal(first, iterations[iterations.Count-2]));

                return new FirstComputer(algo, iterations);
            }

            // same algorithm as above
            public HashSet<Symbol> ComputeSequence(IEnumerable<Symbol> tokens)
            {
                var result = new HashSet<Symbol>();
                foreach (var Bi in tokens)
                {
                    if (Bi.Type == Symbol.Types.Terminal)
                    {
                        result.Add(Bi);
                        break;
                    }
                    else
                    {
                        result.UnionWith(this.Result[Bi]);
                    }
                    if (!Algo.Nullable.Result[Bi])
                        break;
                }
                return result;
            }
        }
    }
}
