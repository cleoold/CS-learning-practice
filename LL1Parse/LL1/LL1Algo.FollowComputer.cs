using System.Linq;
using System.Collections.Generic;

namespace LL1Parse
{
    using FollowResult = HashSet<Symbol>;
    using FollowTable = Dictionary<Symbol, HashSet<Symbol>>;

    partial class LL1Algo
    {
        /// <summary>
        /// compute follow set for all symbols in NonTerminal
        /// </summary>
        public class FollowComputer : AbstractComputer<FollowResult>
        {
            public readonly List<FollowTable> Iterations;

            public override FollowTable Result { get => Iterations.Last(); }

            private FollowComputer(LL1Algo algo, List<FollowTable> iterations)
                : base(algo)
            {
                Iterations = iterations;
            }

            /// <exception cref="NotComputedException"></exception>
            public static FollowComputer Compute(LL1Algo algo)
            {
                _ = algo.Nullable ?? throw new NotComputedException(nameof(algo.Nullable));
                _ = algo.First ?? throw new NotComputedException(nameof(algo.First));

                var iterations = new List<FollowTable>();

                var follow = new FollowTable();
                // initialize first iteration
                foreach (var A in algo.Thecfg.Nonterminals)
                    follow.Add(A, new FollowResult());
                do
                {
                    if (iterations.Count == 0)
                        iterations.Add(follow);
                    else
                        iterations.Add(DictUtil.Clone(follow));
                    follow = iterations.Last();
                    foreach (var prod in algo.Thecfg.Productions)
                    {
                        for (int i = 0; i < prod.Rhs.Count; ++i)
                        {
                            var Bi = prod.Rhs[i];
                            if (Bi.Type == Symbol.Types.NonTerminal)
                            {
                                // test Bi+1,...,Bk
                                var rest = prod.Rhs.Skip(i+1);
                                follow[Bi].UnionWith(algo.First.Sequence(rest));
                                if (rest.All(e => e.Type == Symbol.Types.NonTerminal && algo.Nullable[e])
                                    || i == prod.Rhs.Count-1)
                                    follow[Bi].UnionWith(follow[prod.Lhs]);
                            }
                        }
                    }
                // stop loop if no changes between iterations
                } while (iterations.Count == 1 ||
                    !DictUtil.Equal(follow, iterations[iterations.Count-2]));

                return new FollowComputer(algo, iterations);
            }
        }
    }
}
