using System.Collections.Generic;
using System.Linq;

namespace CCalculator
{
    public static class EvalRPN
    {
        public static decimal Eval(string rpn)
        {
            var tokens = 
                from s in rpn.Split(' ') where s != "" select s;
            var stak = new Stack<decimal>();
            foreach (var token in tokens)
            {
                decimal constant;
                if (decimal.TryParse(token, out constant))
                {
                    stak.Push(constant);
                    continue;
                }
                decimal rhs = stak.Pop();
                decimal lhs = stak.Pop();
                switch (token)
                {
                case "+":
                    stak.Push(lhs + rhs);
                    break;
                case "-":
                    stak.Push(lhs - rhs);
                    break;
                case "*":
                    stak.Push(lhs * rhs);
                    break;
                case "/":
                    stak.Push(lhs / rhs);
                    break;
                }
            }
            return stak.Pop();
        }
    }
}
