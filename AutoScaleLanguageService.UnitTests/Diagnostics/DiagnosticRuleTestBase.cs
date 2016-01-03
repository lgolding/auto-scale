using FluentAssertions;
using Lakewood.AutoScale.Diagnostics;
using Xunit;

namespace Lakewood.AutoScale.UnitTests.Diagnostics
{
    public abstract class DiagnosticRuleTestBase
    {
        protected void RunTestCase(string testName, string input, Diagnostic[] expectedDiagnostics)
        {
            var parser = new Parser(input);

            parser.Parse();

            parser.Diagnostics.Count.Should().Be(expectedDiagnostics.Length);
            parser.Diagnostics.Should().ContainInOrder(expectedDiagnostics);
        }
    }
}
