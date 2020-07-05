using System;
using System.Linq;
using System.Collections.Generic;

namespace LL1Parse
{
    class Program
    {
        CFG thecfg;

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
        }

        void PrintCFGSummary()
        {
            Console.WriteLine($"Non terminals: {string.Join(", ", thecfg.Nonterminals.OrderBy(e => e.Name))}");
            Console.WriteLine($"Terminals    : {string.Join(", ", thecfg.Terminals.OrderBy(e => e.Name))}");
            Console.WriteLine($"Start        : {thecfg.StartSymbol}");
            Console.WriteLine("\nProductions:");
            int i = 0;
            foreach (var p in thecfg.Productions)
                Console.WriteLine($"{i++}. {p}");
        }

        static void Main(string[] args)
        {
            var program = new Program();
            program.ReadCFGFromConsole();
            program.PrintCFGSummary();
        }
    }
}
