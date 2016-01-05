using System.Linq;
using FluentAssertions;
using Lakewood.AutoScale.Diagnostics;

namespace Lakewood.AutoScale.UnitTests.Diagnostics
{
    public abstract class DiagnosticRuleTestBase
    {
        protected void RunTestCase(string testName, string input, Diagnostic[] expectedDiagnostics)
        {
            var parser = new Parser(input);
            var formulaNode = parser.Parse();

            var analyzer = new Analyzer();
            analyzer.Analyze(formulaNode);

            var allDiagnostics = parser.Diagnostics.Union(analyzer.Diagnostics);

            allDiagnostics.Count().Should().Be(expectedDiagnostics.Length);
            allDiagnostics.Should().ContainInOrder(expectedDiagnostics);
        }
    }
}
