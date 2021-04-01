// These tests are for private use only
// Redistributing this file is strictly against SoC policy.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;
using System.Text.RegularExpressions;

namespace PS1GradingTests
{


    /// <summary>
    ///This is a test class for EvaluatorTest and is intended
    ///to contain all EvaluatorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EvaluatorTest
    {

        [TestMethod(), Timeout(5000)]
        public void TestSingleNumber()
        {
            Formula formula = new Formula("5");
            Assert.AreEqual(5.0, formula.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestSingleVariable()
        {
            Formula formula = new Formula("X5");
            Assert.AreEqual(13.0, formula.Evaluate(s => 13));
        }

        [TestMethod(), Timeout(5000)]
        public void TestAddition()
        {
            Formula formula = new Formula("5+3");
            Assert.AreEqual(8.0, formula.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestSubtraction()
        {
            Formula formula = new Formula("18-10");
            Assert.AreEqual(8.0, formula.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestMultiplication()
        {
            Formula formula = new Formula("2*4");
            Assert.AreEqual(8.0, formula.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestDivision()
        {
            Formula formula = new Formula("16/2");
            Assert.AreEqual(8.0, formula.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestArithmeticWithVariable()
        {
            Formula formula = new Formula("2+X1");
            Assert.AreEqual(6.0, formula.Evaluate(s => 4));
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestUnknownVariable()
        {
            Formula formula = new Formula("2+X1");
            formula.Evaluate(s => { throw new FormulaFormatException("Unknown variable"); });
        }

        [TestMethod(), Timeout(5000)]
        public void TestLeftToRight()
        {
            Formula formula = new Formula("2*6+3");
            Assert.AreEqual(15.0, formula.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestOrderOperations()
        {
            Formula formula = new Formula("2+6*3");
            Assert.AreEqual(20.0, formula.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestParenthesesTimes()
        {
            Formula formula = new Formula("(2+6)*3");
            Assert.AreEqual(24.0, formula.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestTimesParentheses()
        {
            Formula formula = new Formula("2*(3+5)");
            Assert.AreEqual(16.0, formula.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestPlusParentheses()
        {
            Formula formula = new Formula("2+(3+5)");
            Assert.AreEqual(10.0, formula.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestPlusComplex()
        {
            Formula formula = new Formula("2+(3+5*9)");
            Assert.AreEqual(50.0, formula.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestOperatorAfterParens()
        {
            Formula formula = new Formula("(1*1)-2/2");
            Assert.AreEqual(0.0, formula.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestComplexTimesParentheses()
        {
            Formula formula = new Formula("2+3*(3+5)");
            Assert.AreEqual(26.0, formula.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestComplexAndParentheses()
        {
            Formula formula = new Formula("2+3*5+(3+4*8)*5+2");
            Assert.AreEqual(194.0, formula.Evaluate(s => 0));
        }

        [TestMethod(), Timeout(5000)]
        public void TestDivideByZero()
        {
            Formula formula = new Formula("5/0");
            Console.Write(formula.Evaluate(s => 0));
            Assert.IsTrue(formula.Evaluate(s => 0) is FormulaError);
            FormulaError v = (FormulaError)formula.Evaluate(s => 0);
            Assert.IsTrue(v.Reason.Length > 0);
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSingleOperator()
        {
            new Formula("+");
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraOperator()
        {
            new Formula("2+5+");
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraParentheses()
        {
            new Formula("2+5*7)");
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidVariable()
        {
            new Formula("xx", s => s, s => false);
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestPlusInvalidVariable()
        {
            new Formula("5+xx", s => s, s => false);
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestParensNoOperator()
        {
            new Formula("5+7+(5)8");
        }


        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestEmpty()
        {
            new Formula("");
        }

        [TestMethod(), Timeout(5000)]
        public void TestComplexMultiVar()
        {
            Formula formula = new Formula("y1*3-8/2+4*(8-9*2)/14*x7");
            Assert.AreEqual(5.142857142857142, formula.Evaluate(s => (s == "x7") ? 1 : 4));
        }

        [TestMethod(), Timeout(5000)]
        public void TestComplexNestedParensRight()
        {
            Formula formula = new Formula("x1+(x2+(x3+(x4+(x5+x6))))");
            Assert.AreEqual(6.0, formula.Evaluate(s => 1));
        }

        [TestMethod(), Timeout(5000)]
        public void TestComplexNestedParensLeft()
        {
            Formula formula = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Assert.AreEqual(12.0, formula.Evaluate(s => 2));
        }

        [TestMethod(), Timeout(5000)]
        public void TestRepeatedVar()
        {
            Formula formula = new Formula("a4-a4*a4/a4");
            Assert.AreEqual(0.0, formula.Evaluate(s => 3));
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestDoubleOperator()
        {
            new Formula("5**7");
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestStartingOperator()
        {
            Formula formula = new Formula("+2");
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestStartingClosingParentheses()
        {
            Formula formula = new Formula(")+2");
        }

        [TestMethod(), Timeout(5000)]
        public void TestToString()
        {
            Formula formula = new Formula("5*7+(2/4)+a2");
            Assert.AreEqual("5*7+(2/4)+a2", formula.ToString());
        }

        [TestMethod(), Timeout(5000)]
        public void TestEqualFormulas()
        {
            Formula formula = new Formula("5*7+(2/4)+a2");
            Formula formula2 = new Formula("5*7+(2/4)+a2");
            Assert.AreEqual(formula, formula2);
            Formula formula3 = new Formula("5*7+(2/4)+A2");
            Assert.AreNotEqual(formula, formula3);
            Formula formula4 = new Formula("5*7+(2/4)+A2", s => s.ToLower(), s => true);
            Assert.AreEqual(formula, formula4);
        }

        [TestMethod(), Timeout(5000)]
        public void TestEquals()
        {
            Formula formula = new Formula("5*7+(2/4)+a2");
            Formula formula2 = new Formula("5*7+(2/4)+a2");
            Assert.IsTrue(formula.Equals(formula2));
            Formula formula3 = new Formula("5*7+(2/4)+A2");
            Assert.IsFalse(formula.Equals(formula3));
            Formula formula4 = new Formula("5*7+(2/4)+A2", s => s.ToLower(), s => true);
            Assert.IsTrue(formula.Equals(formula4));
        }

        [TestMethod(), Timeout(5000)]
        public void TestNullEquals()
        {
            Formula formula = new Formula("5*7+(2/4)+a2");
            Assert.IsFalse(formula.Equals(null));
        }

        [TestMethod(), Timeout(5000)]
        public void TestEqualOperator()
        {
            Formula formula = new Formula("5*7+(2/4)+a2");
            Formula formula2 = new Formula("5*7+(2/4)+a2");
            Assert.IsTrue(formula == formula2);
            Formula formula3 = new Formula("5*7+(2/4)+A2");
            Assert.IsFalse(formula == formula3);
            Formula formula4 = new Formula("5*7+(2/4)+A2", s => s.ToLower(), s => true);
            Assert.IsTrue(formula == formula4);
        }

        [TestMethod(), Timeout(5000)]
        public void TestNullEqualOperator()
        {
            Formula formula2 = new Formula("5*7+(2/4)+a2");
            Assert.IsFalse(null == formula2);
        }

        [TestMethod(), Timeout(5000)]
        public void TestNullNotEqualOperator()
        {
            Formula formula2 = new Formula("5*7+(2/4)+a2");
            Assert.IsTrue(null != formula2);
        }

        [TestMethod(), Timeout(5000)]
        public void TestNotEqualOperator()
        {
            Formula formula = new Formula("5*7+(2/4)+a2");
            Formula formula2 = new Formula("5*7+(2/4)+a2");
            Assert.IsFalse(formula != formula2);
            Formula formula3 = new Formula("5*7+(2/4)+A2");
            Assert.IsTrue(formula != formula3);
            Formula formula4 = new Formula("5*7+(2/4)+A2", s => s.ToLower(), s => true);
            Assert.IsFalse(formula != formula4);
        }

        [TestMethod(), Timeout(5000)]
        public void TestHashCode()
        {
            Formula formula = new Formula("5*7+(2/4)+a2");
            Formula formula2 = new Formula("5*7+(2/4)+a2");
            Assert.AreEqual(formula.GetHashCode(), "5*7+(2/4)+a2".GetHashCode());
            Assert.AreEqual(formula.GetHashCode(), formula2.GetHashCode());
            Formula formula3 = new Formula("5*7+(2/4)+A2");
            Assert.AreNotEqual(formula.GetHashCode(), formula3.GetHashCode());
            Formula formula4 = new Formula("5*7+(2/4)+A2", s => s.ToLower(), s => true);
            Assert.AreEqual(formula.GetHashCode(), formula4.GetHashCode());
        }

        [TestMethod(), Timeout(5000)]
        public void TestNormalizer()
        {
            Formula formula = new Formula("bd2+v4", s => s.ToUpper(), s => true);
            Assert.AreEqual("BD2+V4", formula.ToString());
        }

        [TestMethod(), Timeout(5000)]
        public void TestValidator()
        {
            new Formula("a23", s => s, s => Regex.IsMatch(s, "^[a-zA-Z]+[0-9]+$"));
            new Formula("bd2+v4", s => s, s => Regex.IsMatch(s, "^[a-zA-Z]+[0-9]+$"));
            new Formula("bd2*f4", s => s, s => Regex.IsMatch(s, "^[a-zA-Z]+[0-9]+$"));
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestParenthesesError()
        {
            new Formula("()");
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestOperatorAfterOpenParentheses()
        {
            new Formula("(+5)");
        }

        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestVariableAfterClosingParentheses()
        {
            new Formula("(2+5)a2");
        }

        [TestMethod(), Timeout(5000)]
        public void TestVariableDivideByZero()
        {
            Formula formula = new Formula("10/a2");
            Assert.IsTrue(formula.Evaluate(s => 0) is FormulaError);
        }

        [TestMethod(), Timeout(5000)]
        public void TestParenthesesDivideByZero()
        {
            Formula formula = new Formula("20/(10-10)");
            Assert.IsTrue(formula.Evaluate(s => 0) is FormulaError);
        }

        [TestMethod(), Timeout(5000)]
        public void TestDivideByZeroEndParentheses()
        {
            Formula formula = new Formula("(20/(5-5))");
            Assert.IsTrue(formula.Evaluate(s => 0) is FormulaError);
        }

        [TestMethod(), Timeout(5000)]
        public void TestDivideByParentheses()
        {
            Formula formula = new Formula("20/(10-5)");
            Assert.AreEqual(4.0, formula.Evaluate(s => 0));
        }

    }
}



