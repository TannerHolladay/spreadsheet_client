// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    ///     Represents formulas written in standard infix notation using standard precedence
    ///     rules.  The allowed symbols are non-negative numbers written using double-precision
    ///     floating-point syntax (without unary preceeding '-' or '+');
    ///     variables that consist of a letter or underscore followed by
    ///     zero or more letters, underscores, or digits; parentheses; and the four operator
    ///     symbols +, -, *, and /.
    ///     Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    ///     a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable;
    ///     and "x 23" consists of a variable "x" and a number "23".
    ///     Associated with every formula are two delegates:  a normalizer and a validator.  The
    ///     normalizer is used to convert variables into a canonical form, and the validator is used
    ///     to add extra restrictions on the validity of a variable (beyond the standard requirement
    ///     that it consist of a letter or underscore followed by zero or more letters, underscores,
    ///     or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private readonly List<string> _substrings;
        private readonly HashSet<string> _variables;

        /// <summary>
        ///     Creates a Formula from a string that consists of an infix expression written as
        ///     described in the class comment.  If the expression is syntactically invalid,
        ///     throws a FormulaFormatException with an explanatory Message.
        ///     The associated normalizer is the identity function, and the associated validator
        ///     maps every string to true.
        /// </summary>
        public Formula(string formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        ///     Creates a Formula from a string that consists of an infix expression written as
        ///     described in the class comment.  If the expression is syntactically incorrect,
        ///     throws a FormulaFormatException with an explanatory Message.
        ///     The associated normalizer and validator are the second and third parameters,
        ///     respectively.
        ///     If the formula contains a variable v such that normalize(v) is not a legal variable,
        ///     throws a FormulaFormatException with an explanatory message.
        ///     If the formula contains a variable v such that isValid(normalize(v)) is false,
        ///     throws a FormulaFormatException with an explanatory message.
        ///     Suppose that N is a method that converts all the letters in a string to upper case, and
        ///     that V is a method that returns true only if a string consists of one letter followed
        ///     by one digit.  Then:
        ///     new Formula("x2+y3", N, V) should succeed
        ///     new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        ///     new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(string formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            var tokens = GetTokens(formula).ToList();
            if (tokens.Count == 0) throw new FormulaFormatException("No valid tokens in the formula were given.");

            const string opPattern = @"^[\+\-*/]$";

            string first = tokens.First();
            string last = tokens.Last();

            if (Regex.IsMatch(last, opPattern) || last == "(")
                throw new FormulaFormatException(
                    "Last token should only be a number, variable, or closing parentheses");

            _substrings = new List<string>();
            _variables = new HashSet<string>();
            var parCount = 0;
            var prev = "";
            foreach (string token in tokens)
            {
                string current = token;
                // Test if the substring is a number
                if (Regex.IsMatch(token, opPattern) || token == ")")
                {
                    if (_substrings.Count == 0)
                        throw new FormulaFormatException(
                            "First token of an expression or after an open parentheses should only be a number, variable, or open parentheses");
                    if (prev == "(")
                        throw new FormulaFormatException("Open parentheses is being used before " + token +
                                                         " which is not allowed");
                    if (Regex.IsMatch(prev, opPattern))
                        throw new FormulaFormatException(
                            "Double operator or operator after open parentheses is not allowed");

                    if (token == ")")
                    {
                        if (parCount <= 0)
                            throw new FormulaFormatException(
                                "Not all of the open parentheses in the equation are closed!");
                        parCount--;
                    }
                }
                else
                {
                    if (prev == ")")
                        throw new FormulaFormatException("Can't use a number or variable after a closing parentheses");
                    if (prev != "" && prev != "(" && !Regex.IsMatch(prev, opPattern))
                        throw new FormulaFormatException("Operator is missing from the equation");
                    if (token == "(")
                    {
                        parCount++;
                    }
                    else if (double.TryParse(token, out double tokenDouble))
                    {
                        current = tokenDouble.ToString(CultureInfo.CurrentCulture);
                    }
                    else if (isValid(token))
                    {
                        current = normalize(token);
                        _variables.Add(current);
                    }
                    else
                    {
                        throw new FormulaFormatException("Unknown token " + token + " was given in the expression.");
                    }
                }

                _substrings.Add(current);

                prev = token;
            }

            if (parCount != 0)
                throw new FormulaFormatException("Not all of the open parentheses in the equation are closed!");
        }

        /// <summary>
        ///     Evaluates this Formula, using the lookup delegate to determine the values of
        ///     variables.  When a variable symbol v needs to be determined, it should be looked up
        ///     via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to
        ///     the constructor.)
        ///     For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters
        ///     in a string to upper case:
        ///     new Formula("x+7", N, s => true).Evaluate(L) is 11
        ///     new Formula("x+7").Evaluate(L) is 9
        ///     Given a variable symbol as its parameter, lookup returns the variable's value
        ///     (if it has one) or throws an ArgumentException (otherwise).
        ///     If no undefined variables or divisions by zero are encountered when evaluating
        ///     this Formula, the value is returned.  Otherwise, a FormulaError is returned.
        ///     The Reason property of the FormulaError should have a meaningful explanation.
        ///     This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            // Breaks down the string into individual characters and symbols
            var operatorStack = new Stack<string>();
            var valueStack = new Stack<double>();
            // For loop to go through each of the substrings. Sub is the current substring the loop is on
            foreach (string sub in _substrings)
            {
                string topOperator = operatorStack.Any() ? operatorStack.Peek() : "";
                // Test if the substring is a number
                if (double.TryParse(sub, out double value))
                {
                    if (topOperator == "*" || topOperator == "/")
                        if (!TryCalculate(value, valueStack.Pop(), operatorStack.Pop(), out value))
                            return new FormulaError("Can't divide by zero");

                    valueStack.Push(value);
                }
                else
                {
                    switch (sub)
                    {
                        case "+":
                        case "-":
                        {
                            if (topOperator == "+" || topOperator == "-")
                            {
                                // Calculate the two top values with the top operator
                                TryCalculate(valueStack.Pop(), valueStack.Pop(), operatorStack.Pop(), out value);
                                valueStack.Push(value);
                            }

                            operatorStack.Push(sub);
                            break;
                        }
                        case "*":
                        case "/":
                        case "(":
                            operatorStack.Push(sub);
                            break;
                        case ")":
                        {
                            if (topOperator == "+" || topOperator == "-")
                            {
                                // Calculate the two top values with the top operator
                                TryCalculate(valueStack.Pop(), valueStack.Pop(), operatorStack.Pop(), out value);
                                valueStack.Push(value);
                            }

                            operatorStack.Pop();

                            topOperator = operatorStack.Any() ? operatorStack.Peek() : "";
                            if (topOperator != "*" && topOperator != "/") continue;
                            // Calculate the two top values with the top operator
                            if (!TryCalculate(valueStack.Pop(), valueStack.Pop(), operatorStack.Pop(), out value))
                                return new FormulaError("Can't divide by zero");
                            valueStack.Push(value);
                            break;
                        }
                        //Test if the substring is a value. Ex(a4, ab37, h4, etc..)
                        default:
                        {
                            try
                            {
                                value = lookup(sub);
                            }
                            catch (ArgumentException)
                            {
                                return new FormulaError("Given value in the formula does not exist in the lookup");
                            }

                            if (topOperator == "*" || topOperator == "/")
                                if (!TryCalculate(value, valueStack.Pop(), operatorStack.Pop(), out value))
                                    return new FormulaError("Can't divide by zero");

                            valueStack.Push(value);
                            break;
                        }
                    }
                }
            }

            if (!operatorStack.Any()) return valueStack.Pop();

            return TryCalculate(valueStack.Pop(), valueStack.Pop(), operatorStack.Pop(), out double calculation)
                ? calculation
                : new FormulaError("Can't divide by zero");
        }


        /// <summary>
        ///     Calculates a number from the given string operator
        /// </summary>
        /// <param name="num1">The first number</param>
        /// <param name="num2">The second number</param>
        /// <param name="op">The operator to do the calculation</param>
        /// <param name="result">The result of the calculation</param>
        /// <returns>Returns false if the operator is invalid or dividing by zero occurs</returns>
        private static bool TryCalculate(double num1, double num2, string op, out double result)
        {
            switch (op)
            {
                case "*":
                    result = num2 * num1;
                    return true;
                case "/":
                    result = num2 / num1;
                    return num1 != 0;
                case "-":
                    result = num2 - num1;
                    return true;
                case "+":
                    result = num2 + num1;
                    return true;
            }

            result = 0;
            return false;
        }


        /// <summary>
        ///     Enumerates the normalized versions of all of the variables that occur in this
        ///     formula.  No normalization may appear more than once in the enumeration, even
        ///     if it appears more than once in this Formula.
        ///     For example, if N is a method that converts all the letters in a string to upper case:
        ///     new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        ///     new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        ///     new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<string> GetVariables()
        {
            return _variables;
        }

        /// <summary>
        ///     Returns a string containing no spaces which, if passed to the Formula
        ///     constructor, will produce a Formula f such that this == (f).  All of the
        ///     variables in the string should be normalized.
        ///     For example, if N is a method that converts all the letters in a string to upper case:
        ///     new Formula("x + y", N, s => true).ToString() should return "X+Y"
        ///     new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return string.Join("", _substrings.ToArray());
        }

        /// <summary>
        ///     If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        ///     whether or not this Formula and obj are equal.
        ///     Two Formulae are considered equal if they consist of the same tokens in the
        ///     same order.  To determine token equality, all tokens are compared as strings
        ///     except for numeric tokens and variable tokens.
        ///     Numeric tokens are considered equal if they are equal after being "normalized"
        ///     by C#'s standard conversion from string to double, then back to string. This
        ///     eliminates any inconsistencies due to limited floating point precision.
        ///     Variable tokens are considered equal if their normalized forms are equal, as
        ///     defined by the provided normalizer.
        ///     For example, if N is a method that converts all the letters in a string to upper case:
        ///     new Formula("x1+y2", N, s => true) == (new Formula("X1  +  Y2")) is true
        ///     new Formula("x1+y2") == (new Formula("X1+Y2")) is false
        ///     new Formula("x1+y2") == (new Formula("y2+x1")) is false
        ///     new Formula("2.0 + x7") == (new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is Formula)) return false;
            return ToString() == obj.ToString();
        }

        /// <summary>
        ///     Reports whether f1 == f2, using the notion of equality from the Equals method.
        ///     Note that if both f1 and f2 are null, this method should return true.  If one is
        ///     null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (f1 is null || f2 is null) return false;
            return f1.Equals(f2);
        }

        /// <summary>
        ///     Reports whether f1 != f2, using the notion of equality from the Equals method.
        ///     Note that if both f1 and f2 are null, this method should return false.  If one is
        ///     null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            if (f1 is null || f2 is null) return true;
            return !f1.Equals(f2);
        }

        /// <summary>
        ///     Returns a hash code for this Formula.  If f1 == (f2), then it must be the
        ///     case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two
        ///     randomly-generated unequal Formula have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        ///     Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        ///     right paren; one of the four operator symbols; a string consisting of a letter or underscore
        ///     followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        ///     match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(string formula)
        {
            // Patterns for individual tokens
            var lpPattern = @"\(";
            var rpPattern = @"\)";
            var opPattern = @"[\+\-*/]";
            var varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            var doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            var spacePattern = @"\s+";

            // Overall pattern
            string pattern = string.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (string s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                    yield return s;
        }
    }

    /// <summary>
    ///     Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        ///     Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    ///     Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        ///     Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(string reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///     The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; }
    }
}