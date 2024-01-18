using NUnit.Framework;
using Python.Runtime;
using QuantConnect.Algorithm.Framework.Portfolio;
using QuantConnect.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantConnect.Tests.Algorithm.Framework.Portfolio;

[TestFixture]
public class PortfolioOptimizerPythonWrapperTests
{
    [Test]
    public void OptimizeIsCalled()
    {
        using (Py.GIL())
        {
            var module = PyModule.FromString(Guid.NewGuid().ToString(),
                    @$"
from AlgorithmImports import *

class CustomPortfolioOptimizer:
    def __init__(self):
        self.OptimizeWasCalled = False

    def Optimize(self, historicalReturns, expectedReturns = None, covariance = None):
        self.OptimizeWasCalled = True");
                
            var pyCustomOptimizer = module.GetAttr("CustomPortfolioOptimizer").Invoke();
            var wrapper = new PortfolioOptimizerPythonWrapper(pyCustomOptimizer);
            var historicalReturns = new double[,] { { -0.50, -0.13 }, { 0.81, 0.31 }, { -0.02, 0.01 } };

            wrapper.Optimize(historicalReturns);
            pyCustomOptimizer
                .GetAttr("OptimizeWasCalled")
                .TryConvert(out bool optimizerWasCalled);

            Assert.IsTrue(optimizerWasCalled);
        }
    }

    [Test]
    public void WrapperThrowsIfOptimizerDoesNotImplementInterface()
    {
        using (Py.GIL())
        {
            var module = PyModule.FromString(Guid.NewGuid().ToString(),
                    @$"
from AlgorithmImports import *

class CustomPortfolioOptimizer:
    def __init__(self):
        self.OptimizeWasCalled = False

    def Calculate(self, historicalReturns, expectedReturns = None, covariance = None):
        pass");

            var pyCustomOptimizer = module.GetAttr("CustomPortfolioOptimizer").Invoke();

            Assert.Throws<NotImplementedException>(() => new PortfolioOptimizerPythonWrapper(pyCustomOptimizer));
        }
    }
}
