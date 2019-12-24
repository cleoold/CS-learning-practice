using System;
using System.Collections.Generic;
using System.Text;

namespace CCalculator
{
    // (C++ source: Classic data structures in C++, T. A. Budd., 1994)

    public static class ToRPN
    {
        private enum Operators
        {
            leftParen,
            plus,
            minus,
            divide,
            mul,
            pow
        }

        static private Dictionary<Operators, string> opString = new Dictionary<Operators, string>
        {
            { Operators.plus, " + " },
            { Operators.minus, " - " },
            { Operators.mul, " * " },
            { Operators.divide, " / " },
            { Operators.pow, " ^ " }
        };

        static private void processOp(Operators op, Stack<Operators> opStack, StringBuilder result)
        {   // pop stack when they have higher precedence
            while (opStack.Count != 0 && op < opStack.Peek())
                result.Append(opString[opStack.Pop()]);
            // push current op
            opStack.Push(op);
        }

        static public string infixToRPN(string infixStr)
        {
            var opStack = new Stack<Operators>();
            var res = new StringBuilder();
            int j = 0;
            while (j < infixStr.Length)
            {   // process constants
                if (Char.IsDigit(infixStr[j]))
                {
                    while (j < infixStr.Length && (Char.IsDigit(infixStr[j]) || infixStr[j] == '.' || infixStr[j] == ','))
                        res.Append(infixStr[j++]);
                    res.Append(' ');
                    continue;
                } // process other chars
                switch (infixStr[j++])
                {
                case '(':
                    opStack.Push(Operators.leftParen);
                    break;
                case ')':
                    while (opStack.Peek() != Operators.leftParen)
                        res.Append(opString[opStack.Pop()]);
                    opStack.Pop(); // pops left paren
                    break;
                case '+':
                    processOp(Operators.plus, opStack, res);
                    break;
                case '-':
                    processOp(Operators.minus, opStack, res);
                    break;
                case '*':
                    processOp(Operators.mul, opStack, res);
                    break;
                case '/':
                    processOp(Operators.divide, opStack, res);
                    break;
                case '^':
                    processOp(Operators.pow, opStack, res);
                    break;
                }
            }
            // empty stack on end of input
            while (opStack.Count != 0)
                res.Append(opString[opStack.Pop()]);

            return res.ToString();
        }
    }
}

