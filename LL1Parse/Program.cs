#nullable disable

using System;
using System.Linq;
using System.Collections.Generic;

namespace LL1Parse
{
    class Program
    {
        CFG Thecfg;
        LL1Algo Algo;

        void ReadCFGFromConsole()
        {
            var lines = new List<string>();
            string line;
            while (!string.IsNullOrEmpty(line = Console.ReadLine()))
                lines.Add(line);
            Thecfg = CFG.FromProductionStrings(lines);
            Algo = new LL1Algo(Thecfg);
            Algo.Compute();
        }

        void PrintCFGSummary()
        {
            Console.WriteLine($"Non-terminals: {Thecfg.Nonterminals.ToPrettyString()}");
            Console.WriteLine($"Terminals    : {Thecfg.Terminals.ToPrettyString()}");
            Console.WriteLine($"Start        : {Thecfg.StartSymbol}");
            Console.WriteLine("\nProductions:");
            foreach (var p in Thecfg.Productions)
                Console.WriteLine(p);

            Console.WriteLine("\nNullable:");
            foreach (var (sym, val) in Algo.Nullable.OrderBy(e => e.Key.Name))
            {
                Console.Write(sym.Name.PadRight(5));
                    Console.Write($"{(val ? "true " : "false")} ");
                Console.WriteLine();
            }

            Console.WriteLine("\nFirst:");
            foreach (var (sym, val) in Algo.First.OrderBy(e => e.Key.Name))
            {
                Console.Write(sym.Name.PadRight(5));
                Console.WriteLine(val.ToPrettyString());
            }

            Console.WriteLine("\nFollow:");
            foreach (var (sym, val) in Algo.Follow.OrderBy(e => e.Key.Name))
            {
                Console.Write(sym.Name.PadRight(5));
                Console.WriteLine(val.ToPrettyString());
            }

            Console.WriteLine("\nParse table:");
            var parset = Algo.Parser.OrderBy(e => e.Key.Name);
            // string repr of each COLUMN
            var allcols = Thecfg.Terminals.Select(term => new KeyValuePair<Symbol, string[]>(
                term, parset.Select(e => string.Join(", ", e.Value[term].OrderBy(x => x.Number).Select(x => x.Number))).ToArray()
            )).ToDictionary(e => e.Key, e => e.Value);
            // get max width of each COLUMN
            var paddings = Thecfg.Terminals.Select(term => new KeyValuePair<Symbol, int>(
                term, Math.Max(term.Name.Length, allcols[term].Max(e => e.Length))
            )).ToDictionary(e => e.Key, e => e.Value);
            Console.Write("     ");
            foreach (var (sym, pad) in paddings.OrderBy(e => e.Key.Name))
                Console.Write(sym.Name.PadRight(pad+3));
            Console.WriteLine();
            int i = 0;
            foreach (var (sym, vals) in Algo.Parser.OrderBy(e => e.Key.Name))
            {
                Console.Write(sym.Name.PadRight(5));
                Console.WriteLine(string.Join(' ',
                    vals.OrderBy(e => e.Key.Name)
                        .Select(e =>
                            $"{allcols[e.Key][i].PadRight(paddings[e.Key]+2)}"
                        )
                ));
                ++i;
            }
            Console.WriteLine($"Grammar is LL1 {(Algo.Parser.Parsable ? "parsable" : "unparsable")}.");
        }

        void SummarizeSentencesFromConsole()
        {
            if (!Algo.Parser.Parsable)
                return;
            string line;
            while (!string.IsNullOrEmpty(line = Console.ReadLine()))
            {
                Console.WriteLine($"\nDerivation of \"{line}\":");
                try
                {
                    var derivations = Algo.Parser.EasyParse(line);
                    var pad = derivations.Max(e => e.Item2.Length);
                    foreach (var (rule, str) in derivations)
                    {
                        Console.Write(str.PadRight(pad+2));
                        Console.WriteLine($"({rule})");
                    }
                }
                catch (CannotParseException e)
                {
                    Console.WriteLine($"Cannot parse: {e.Message}");
                }
            }
        }

        static void Main(string[] args)
        {
            var program = new Program();
            try
            {
                program.ReadCFGFromConsole();
                program.PrintCFGSummary();
                program.SummarizeSentencesFromConsole();
            }
            catch (Exception e)
            {
                Console.Error.WriteLineAsync($"{e.GetType()}: {e.Message}");
            }
        }
    }
}
