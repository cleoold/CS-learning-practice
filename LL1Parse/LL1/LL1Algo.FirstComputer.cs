using System.Linq;
using System.Collections.Generic;

namespace LL1Parse
{
    using FirstResult = HashSet<Symbol>;
    using FirstTable = Dictionary<Symbol, HashSet<Symbol>>;

    partial class LL1Algo
    {
        /// <summary>
        /// compute first set for all symbols in NonTerminal
        /// and for a sequence of arbitrary symbols
        /// </summary>
        public class FirstComputer : AbstractComputer<FirstResult>
        {
            public readonly List<FirstTable> Iterations;

            public override FirstTable Result { get => Iterations.Last(); }

            private FirstComputer(LL1Algo algo, List<FirstTable> iterations)
                : base(algo)
            {
                Iterations = iterations;
            }

            /// <exception cref="NotComputedException"></exception>
            public static FirstComputer Compute(LL1Algo algo)
            {
                _ = algo.Nullable ?? throw new NotComputedException(nameof(algo.Nullable));

                var iterations = new List<FirstTable>();

                var first = new FirstTable();
                // initialize first iteration
                foreach (var A in algo.Thecfg.Nonterminals)
                    first.Add(A, new FirstResult());
                do
                {
                    if (iterations.Count == 0)
                        iterations.Add(first);
                    else
                        iterations.Add(DictUtil.Clone(first));
                    first = iterations.Last();
                    foreach (var prod in algo.Thecfg.Productions)
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
                            if (!algo.Nullable[Bi])
                                break;
                        }
                    }
                // stop loop if no changes between iterations
                } while (iterations.Count == 1 ||
                    !DictUtil.Equal(first, iterations[iterations.Count-2]));

                return new FirstComputer(algo, iterations);
            }

            // same algorithm as above
            /// <exception cref="NotComputedException"></exception>
            public FirstResult Sequence(IEnumerable<Symbol> syms)
            {
                _ = Algo.Nullable ?? throw new NotComputedException(nameof(Algo.Nullable));

                var result = new FirstResult();
                foreach (var Bi in syms)
                {
                    if (Bi.Type == Symbol.Types.Terminal)
                    {
                        result.Add(Bi);
                        break;
                    }
                    else
                    {
                        result.UnionWith(this[Bi]);
                    }
                    if (!Algo.Nullable[Bi])
                        break;
                }
                return result;
            }
        }
    }
}
