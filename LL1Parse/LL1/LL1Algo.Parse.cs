using System.Linq;
using System.Collections.Generic;

namespace LL1Parse
{
    using ParseTableResult = HashSet<ProductionRule>;
    using ParseTableRow = Dictionary<Symbol, HashSet<ProductionRule>>;
    using ParseTable = Dictionary<Symbol,
        Dictionary<Symbol, HashSet<ProductionRule>>
    >;

    partial class LL1Algo
    {
        public class Parse : AbstractComputer<ParseTableRow>
        {
            private readonly ParseTable _Result;
            public override ParseTable Result { get => _Result; }
            // if not parsable, cannot do more than giving parse table
            public readonly bool Parsable;

            private Parse(LL1Algo algo, ParseTable result)
                : base(algo)
            {
                _Result = result;
                Parsable = result.All(dict => dict.Value.All(e => e.Value.Count <= 1));
            }

            /// <exception cref="NotComputedException"></exception>
            public static Parse Compute(LL1Algo algo)
            {
                _ = algo.Nullable ?? throw new NotComputedException(nameof(algo.Nullable));
                _ = algo.First ?? throw new NotComputedException(nameof(algo.First));
                _ = algo.Follow ?? throw new NotComputedException(nameof(algo.Follow));

                // init 2d dict
                var parset = new ParseTable();
                foreach (var A in algo.Thecfg.Nonterminals)
                {
                    parset.Add(A, new ParseTableRow());
                    foreach (var a in algo.Thecfg.Terminals)
                    {
                        parset[A].Add(a, new ParseTableResult());
                    }
                }

                // do algo
                foreach (var prod in algo.Thecfg.Productions)
                {
                    foreach (var a in algo.First.Sequence(prod.Rhs))
                        parset[prod.Lhs][a].Add(prod);
                    if (algo.Nullable.Sequence(prod.Rhs))
                    {
                        foreach (var a in algo.Follow[prod.Lhs])
                            parset[prod.Lhs][a].Add(prod);
                    }
                }

                return new Parse(algo, parset);
            }

            /// <summary>
            /// try getting the derivations of a list of input strings
            /// </summary>
            /// <exception cref="CannotParseException"></exception>
            public List<(ProductionRule, string)> EasyParse(IEnumerable<string> line)
            {
                if (!Parsable)
                    throw new CannotParseException("this language is not LL1 parsable");

                var syms = StringsToSymbols(line);
                var res = new List<(ProductionRule, string)>();
                var stk = new Stack<Symbol>();
                var read = new List<Symbol>();

                stk.Push(Algo.Thecfg.StartSymbol);
                foreach (var a in syms)
                {
                    Symbol A;
                    // top of stack is non terminal
                    while (stk.Count != 0 && (A = stk.Peek()).Type == Symbol.Types.NonTerminal)
                    {
                        stk.Pop();
                        HashSet<ProductionRule>? predicteds;
                        // essentially this[A][a]
                        if (this[A].TryGetValue(a, out predicteds) && predicteds.Count != 0)
                        {
                            var predicted = predicteds.First();
                            for (int i = predicted.Rhs.Count - 1; i >= 0 ; i--)
                                stk.Push(predicted.Rhs[i]);
                            // read+stack snapshot is the whole string
                            res.Add( (predicted, ReadStackSnapshot(read, stk)) );
                        }
                        else
                        {
                            throw new CannotParseException($"parse failed at symbol {a}");
                        }
                    }
                    // top of stack is terminal
                    if (stk.Count == 0 || !a.Equals(stk.Peek()))
                        throw new CannotParseException($"parse failed at symbol {a}. tos mismatch");
                    read.Add(stk.Pop());
                }

                // if last symbol in original string is not a terminal, stk may have
                // nullable nonterminals left
                // not good!
                while (stk.Count != 0)
                {
                    var A = stk.Pop();
                    var applied = Algo.Thecfg.Productions.Find(e => e.Lhs.Equals(A) && e.DerivesToNull);
                    _ = applied ?? throw new CannotParseException("unexpeced symbol left in stack");
                    res.Add( (applied, ReadStackSnapshot(read, stk)) );
                }

                return res;
            }

            /// <summary>
            /// try getting the derivations of a string
            /// </summary>
            /// <exception cref="CannotParseException"></exception>
            public List<(ProductionRule, string)> EasyParse(string line)
            {
                return EasyParse(line.Split(' ', System.StringSplitOptions.RemoveEmptyEntries));
            }

            /// <exception cref="CannotParseException"></exception>
            private IEnumerable<Symbol> StringsToSymbols(IEnumerable<string> strs)
            {
                return strs.Select(e => {
                    Symbol? symbol;
                    if (Algo.Thecfg.TryQueryTypedSymbolByName(e, out symbol))
                        return symbol;
                    else
                        throw new CannotParseException($"unknown symbol: {e}");
                });
            }

            static private string ReadStackSnapshot(IEnumerable<Symbol> read, IEnumerable<Symbol> stack)
            {
                return string.Join(' ', read.Select(e => e.Name).Concat(stack.Select(e => e.Name)));
            }
        }
    }
}
