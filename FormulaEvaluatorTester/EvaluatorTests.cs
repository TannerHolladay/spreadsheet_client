using FormulaEvaluator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace FormulaEvaluatorTester
{
    [TestClass]
    public class EvaluatorTests
    {
        public Dictionary<string, int> Dic;

        [TestInitialize]
        public void TestInitialize()
        {
            Dic = new Dictionary<string, int>
            {
                ["a32"] = 65,
                ["fd3"] = 87,
                ["ASD55"] = 34,
                ["abc2"] = 43,
                ["h3"] = 65
            };
        }

        public int SampleEvaluator(string str)
        {
            return 0;
        }

        public int WorkingEvaluator(string str)
        {
            return Dic[str];
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MissingParentheses()
        {
            Evaluator.Evaluate("2)", SampleEvaluator);
            Evaluator.Evaluate("2(", SampleEvaluator);
            Evaluator.Evaluate("(2", SampleEvaluator);
            Evaluator.Evaluate("(2", SampleEvaluator);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ErrorInvalidVariable()
        {
            Evaluator.Evaluate("2+2+2a", SampleEvaluator);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MissingNumbers()
        {
            Evaluator.Evaluate("+*/", SampleEvaluator);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MissingNumber()
        {
            Evaluator.Evaluate("2+", SampleEvaluator);
            Evaluator.Evaluate("2*", SampleEvaluator);
            Evaluator.Evaluate("2/", SampleEvaluator);
            Evaluator.Evaluate("2-", SampleEvaluator);
            Evaluator.Evaluate("+2", SampleEvaluator);
            Evaluator.Evaluate("*2", SampleEvaluator);
            Evaluator.Evaluate("/2", SampleEvaluator);
            Evaluator.Evaluate("-2", SampleEvaluator);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidValue()
        {
            Evaluator.Evaluate("2+b5b", SampleEvaluator);
            Evaluator.Evaluate("2+b", SampleEvaluator);
            Evaluator.Evaluate("2+5b", SampleEvaluator);
            Evaluator.Evaluate("2+5b", SampleEvaluator);
            Evaluator.Evaluate("2+5b4", SampleEvaluator);
        }

        [TestMethod]
        public void AdditionExpression()
        {
            Assert.AreEqual(153, Evaluator.Evaluate("30+123", SampleEvaluator));
        }

        [TestMethod]
        public void SubtractionExpression()
        {
            Assert.AreEqual(1233500001, Evaluator.Evaluate("1233512346-12345", SampleEvaluator));
        }

        [TestMethod]
        public void MultiplicationExpression()
        {
            Assert.AreEqual(28204532, Evaluator.Evaluate("6463*4364", SampleEvaluator));
        }

        [TestMethod]
        public void DivisionExpression()
        {
            Assert.AreEqual(147, Evaluator.Evaluate("3456342/23451", SampleEvaluator));
        }

        [TestMethod]
        public void DecimalDivisionExpression()
        {
            Assert.AreEqual(32607, Evaluator.Evaluate("3456342/106", SampleEvaluator));
        }

        [TestMethod]
        public void ParenthesesExpression()
        {
            Assert.AreEqual(2162414, Evaluator.Evaluate("(544+123)*3242", SampleEvaluator));
        }

        [TestMethod]
        public void CorrectOrder()
        {
            Assert.AreEqual(-46, Evaluator.Evaluate("2+2-5*8/4*(3+2)", SampleEvaluator));
        }

        [TestMethod]
        public void ValueEvaluator()
        {
            Assert.AreEqual(106, Evaluator.Evaluate("41+a32", WorkingEvaluator));
            Assert.AreEqual(2584, Evaluator.Evaluate("76*ASD55", WorkingEvaluator));
            Assert.AreEqual(516, Evaluator.Evaluate("12*abc2", WorkingEvaluator));
        }

        [TestMethod]
        public void NoOperators()
        {
            Assert.AreEqual(12, Evaluator.Evaluate("12", SampleEvaluator));
            Assert.AreEqual(20, Evaluator.Evaluate("20", SampleEvaluator));
        }

    }
}
