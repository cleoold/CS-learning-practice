using System;
using System.Linq;
using System.Collections.Generic;

namespace LL1Parse
{
    class Program
    {
        CFG thecfg;
        LL1Algo algo;

        void ReadCFGFromConsole()
        {
            var lines = new List<string>();
            while (true)
            {
                var line = Console.ReadLine();
                if (line == null || line == "")
                    break;
                lines.Add(line);
            }
            thecfg = CFG.FromProductionStrings(lines);
            algo = new LL1Algo(thecfg);
            algo.Compute();
        }

        void PrintCFGSummary()
        {
            Console.WriteLine($"Non-terminals: {string.Join(", ", thecfg.Nonterminals.OrderBy(e => e.Name))}");
            Console.WriteLine($"Terminals    : {string.Join(", ", thecfg.Terminals.OrderBy(e => e.Name))}");
            Console.WriteLine($"Start        : {thecfg.StartSymbol}");
            Console.WriteLine("\nProductions:");
            int i = 0;
            foreach (var p in thecfg.Productions)
                Console.WriteLine($"{i++}. {p}");
            
            Console.WriteLine("\nNullable iterations:");
            foreach (var pair in algo.Nullable.Result)
            {
                Console.Write(pair.Key.Name.PadRight(5));
                foreach (var iter in algo.Nullable.Iterations)
                    Console.Write($"{(iter[pair.Key] ? "true " : "false")} ");
                Console.WriteLine();
            }

            Console.WriteLine("\nFirst:");
            foreach (var pair in algo.First.Result.OrderBy(e => e.Key.Name))
            {
                Console.Write(pair.Key.Name.PadRight(5));
                Console.WriteLine(string.Join(", ", pair.Value.OrderBy(e => e.Name)));
            }

            Console.WriteLine("\nFollow:");
            foreach (var pair in algo.Follow.Result.OrderBy(e => e.Key.Name))
            {
                Console.Write(pair.Key.Name.PadRight(5));
                Console.WriteLine(string.Join(", ", pair.Value.OrderBy(e => e.Name)));
            }
        }

        static void Main(string[] args)
        {
            var program = new Program();
            program.ReadCFGFromConsole();
            program.PrintCFGSummary();
        }
    }
}
