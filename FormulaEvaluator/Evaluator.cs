using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// Calculates expressions when given in a string
    /// </summary>
    public static class Evaluator
    {
        public delegate int Lookup(string v);

        /// <summary>
        /// Takes in a string expression and returns the result using order of operations
        /// </summary>
        /// <param name="exp">The string expression to be calculated</param>
        /// <param name="variableEvaluator">The function to convert variables into a number. Ex(a4, ab37, h4, etc..)</param>
        /// <returns>The calculated result of the expression</returns>
        public static int Evaluate(string exp, Lookup variableEvaluator)
        {
            // Breaks down the string into individual characters and symbols
            var substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            var operatorStack = new Stack<string>();
            var valueStack = new Stack<int>();
            // For loop to go through each of the substrings. Sub is the current substring the loop is on
            foreach (var sub in substrings)
            {
                var topOperator = operatorStack.Any() ? operatorStack.Peek() : "";
                // Test if the substring is a number
                if (int.TryParse(sub, out var value))
                {
                    if (topOperator.Equals("*") || topOperator.Equals("/"))
                    {
                        value = Calculate(value, valueStack.Pop(), operatorStack.Pop());
                    }

                    valueStack.Push(value);
                }
                else if (Regex.IsMatch(sub, "^[a-zA-Z]+[0-9]+$")) //Test if the substring is a value. Ex(a4, ab37, h4, etc..)
                {
                    value = variableEvaluator(sub);
                    if (topOperator.Equals("*") || topOperator.Equals("/"))
                    {
                        value = Calculate(value, valueStack.Pop(), operatorStack.Pop());
                    }

                    valueStack.Push(value);
                }
                else if (sub.Equals("+") || sub.Equals("-"))
                {
                    if (topOperator.Equals("+") || topOperator.Equals("-"))
                    {
                        // Calculate the two top values with the top operator
                        valueStack.Push(Calculate(valueStack.Pop(), valueStack.Pop(), operatorStack.Pop()));
                    }

                    operatorStack.Push(sub);
                }
                else if (sub.Equals("*") || sub.Equals("/"))
                {
                    operatorStack.Push(sub);
                }
                else if (sub.Equals("("))
                {
                    operatorStack.Push(sub);
                }
                else if (sub.Equals(")"))
                {
                    if (topOperator.Equals("+") || topOperator.Equals("-"))
                    {
                        // Calculate the two top values with the top operator
                        valueStack.Push(Calculate(valueStack.Pop(), valueStack.Pop(), operatorStack.Pop()));
                    }

                    topOperator = operatorStack.Any() ? operatorStack.Peek() : "";
                    if (topOperator.Equals("("))
                    {
                        operatorStack.Pop();
                    }
                    else
                    {
                        throw new ArgumentException("Error! Missing Parentheses");
                    }

                    topOperator = operatorStack.Any() ? operatorStack.Peek() : "";
                    if (topOperator.Equals("*") || topOperator.Equals("/"))
                    {
                        // Calculate the two top values with the top operator
                        valueStack.Push(Calculate(valueStack.Pop(), valueStack.Pop(), operatorStack.Pop()));
                    }
                }
                else if (!string.IsNullOrWhiteSpace(sub))
                {
                    throw new ArgumentException("Unknown token " + sub + " was given in the expression.");
                }
            }

            if (!operatorStack.Any())
            {
                if (valueStack.Count() == 1)
                {
                    return valueStack.Pop();
                }

                throw new ArgumentException("No values were given in the expression");
            }

            if (valueStack.Count() == 2)
            {
                return Calculate(valueStack.Pop(), valueStack.Pop(), operatorStack.Pop());
            }
            throw new ArgumentException("An extra operator was given in the expression.");

        }

        /// <summary>
        /// Calculates a number from the given string operator
        /// </summary>
        /// <param name="num1">The first number</param>
        /// <param name="num2">The second number</param>
        /// <param name="op">The operator to do the calculation</param>
        /// <returns>The calculated result</returns>
        private static int Calculate(int num1, int num2, string op)
        {
            switch (op)
            {
                case "*":
                    return num2 * num1;
                case "/":
                    if (num1 == 0) throw new ArgumentException("Can't divide by zero");
                    return num2 / num1;
                case "+":
                    return num2 + num1;
                case "-":
                    return num2 - num1;
            }
            throw new ArgumentException("Invalid operator " + op + " was given");
        }

    }
}
