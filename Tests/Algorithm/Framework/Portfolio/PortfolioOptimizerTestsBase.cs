using NUnit.Framework;
using QuantConnect.Algorithm.Framework.Portfolio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantConnect.Tests.Algorithm.Framework.Portfolio;

public abstract class PortfolioOptimizerTestsBase
{
    protected IList<double[,]> HistoricalReturns { get; set; }

    protected IList<double[]> ExpectedReturns { get; set; }

    protected IList<double[,]> Covariances { get; set; }

    protected IList<double[]> ExpectedResults { get; set; }

    protected abstract IPortfolioOptimizer CreateOptimizer();

    protected virtual void OptimizeWeightings(int testCaseNumber)
    {
        var testOptimizer = CreateOptimizer();

        var result = testOptimizer.Optimize(
            HistoricalReturns[testCaseNumber],
            ExpectedReturns[testCaseNumber],
            Covariances[testCaseNumber]);

        Assert.AreEqual(ExpectedResults[testCaseNumber], result.Select(x => Math.Round(x, 6)));
        Assert.AreEqual(1d, result.Select(x => Math.Round(Math.Abs(x), 6)).Sum());
    }

    [Test]
    protected virtual void EmptyPortfolioReturnsEmptyArrayOfDouble()
    {
        var testOptimizer = CreateOptimizer();
        var historicalReturns = new double[,] { { } };
        var expectedResult = Array.Empty<double>();

        var result = testOptimizer.Optimize(historicalReturns);

        Assert.AreEqual(result, expectedResult);
    }
}
