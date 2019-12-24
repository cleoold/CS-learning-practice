using System;

namespace CCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            string usrinput;
            decimal res;
            Console.ForegroundColor = ConsoleColor.Gray;
            while (true)
            {
                Console.WriteLine("\nInput expression; enter q to exit...");
                Console.Write("EXPR: ");
                usrinput = Console.ReadLine();
                if (usrinput == "q") break;
                string rpn = ToRPN.infixToRPN(usrinput);
                Console.Write("Parsed RPN expression: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(rpn);
                Console.ForegroundColor = ConsoleColor.Gray;
                try
                {
                    res = EvalRPN.Eval(rpn);
                    Console.WriteLine("Result: " + res);
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("Failed to evaluate expression. The expression may be malformed.");
                }
            }
        }
    }
}
