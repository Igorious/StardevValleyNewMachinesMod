﻿using Igorious.StardewValley.DynamicAPI.Delegates;
using Igorious.StardewValley.DynamicAPI.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Igorious.StardewValley.DynamicAPI.Tests
{
    [TestClass]
    public class ExpressionCompilerTests
    {
        [TestMethod]
        public void InvokeIntExpression()
        {
            var f = ExpressionCompiler.CompileExpression<PriceExpression>("100");
            var result = f(1, 350);
            Assert.AreEqual(100, result);
        }

        [TestMethod]
        public void InvokeArithmeticExpression()
        {
            var f = ExpressionCompiler.CompileExpression<CountExpression>("1 + q + p / 200");
            var result = f(350, 1, 0.3, 0.6);
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void InvokeConditionalExpression()
        {
            var f = ExpressionCompiler.CompileExpression<QualityExpression>("(r1 > 0.9)? 2 : (r2 > 0.2)? 1 : 0");
            var result = f(350, 1, 0.3, 0.6);
            Assert.AreEqual(1, result);
        }
    }
}
