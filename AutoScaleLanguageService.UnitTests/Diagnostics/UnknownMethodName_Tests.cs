using FluentAssertions;
using Lakewood.AutoScale.Diagnostics;
using Xunit;

namespace Lakewood.AutoScale.UnitTests.Diagnostics
{
    public class UnknownMethodName_Tests
    {
        public static readonly object[] ValidMethodNameTestCases = new object[]
        {
            new object[]
            {
                "Valid method name",
                "a = $CPUPercent.GetSamples()",
                new string[0]
            },

            new object[]
            {
                "Unknown method name",
                "a = $CPUPercent.GetStuff()",
                new []
                {
                    new Diagnostic(UnknownMethodName.Descriptor),
                }
            },

            new object[]
            {
                "Multiple unknown method names",
                "a = $CPUPercent.GetStuff() + $CPUPercent.Other();\nb = $CPUPercent.Another()",
                new []
                {
                    new Diagnostic(UnknownMethodName.Descriptor),
                    new Diagnostic(UnknownMethodName.Descriptor),
                    new Diagnostic(UnknownMethodName.Descriptor),
                }
            },

            new object[]
            {
                "Multiple unknown method names with parse errors",
                "a = $CPUPercent.GetStuff() + $CPUPercent.Other()^+ $CPUPercent.NotReported();\nb = $CPUPercent.Another()^",
                new []
                {
                    // All of the parse errors appear...
                    new Diagnostic(ParseError.Descriptor),
                    new Diagnostic(ParseError.Descriptor),

                    // ... before any of the errors reported by the diagnostic rules.
                    new Diagnostic(UnknownMethodName.Descriptor),
                    new Diagnostic(UnknownMethodName.Descriptor),
                    new Diagnostic(UnknownMethodName.Descriptor),
                }
            }
        };

        [Theory]
        [MemberData(nameof(ValidMethodNameTestCases))]
        public void Produces_expected_diagnostics(string testName, string input, Diagnostic[] expectedDiagnostics)
        {
            var parser = new Parser(input);

            parser.Parse();

            parser.Diagnostics.Should().ContainInOrder(expectedDiagnostics);
        }
    }
}
