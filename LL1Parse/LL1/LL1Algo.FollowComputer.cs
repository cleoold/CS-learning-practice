using System.Linq;
using System.Collections.Generic;

namespace LL1Parse
{
    using FollowType = Dictionary<Symbol, HashSet<Symbol>>;

    partial class LL1Algo
    {
        /// <summary>
        /// compute follow set for all symbols in NonTerminal
        /// </summary>
        public class FollowComputer
        {
            public readonly LL1Algo Algo;

            public readonly List<FollowType> Iterations;

            public FollowType Result
            {
                get => Iterations.Last();
            }

            public FollowComputer(LL1Algo algo, List<FollowType> iterations)
            {
                Algo = algo;
                Iterations = iterations;
            }

            public static FollowComputer Compute(LL1Algo algo)
            {
                var iterations = new List<FollowType>();

                var follow = new FollowType();
                // initialize first iteration
                foreach (var A in algo.thecfg.Nonterminals)
                    follow.Add(A, new HashSet<Symbol>());
                do
                {
                    if (iterations.Count == 0)
                        iterations.Add(follow);
                    else
                        iterations.Add(DictUtil.Clone(follow));
                    follow = iterations.Last();
                    foreach (var prod in algo.thecfg.Productions)
                    {
                        for (int i = 0; i < prod.Rhs.Count; ++i)
                        {
                            var Bi = prod.Rhs[i];
                            if (Bi.Type == Symbol.Types.NonTerminal)
                            {
                                // test Bi+1,...,Bk
                                var rest = prod.Rhs.Skip(i+1);
                                follow[Bi].UnionWith(algo.First.ComputeSequence(rest));
                                if (rest.All(e => e.Type == Symbol.Types.NonTerminal && algo.Nullable.Result[e])
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
