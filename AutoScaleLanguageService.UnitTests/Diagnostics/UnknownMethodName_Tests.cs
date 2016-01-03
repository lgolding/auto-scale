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
                    new Diagnostic(UnknownMethodNameRule.Descriptor, UnknownMethodNameRule.FormatMessage("GetStuff")),
                }
            },

            new object[]
            {
                "Multiple unknown method names",
                "a = $CPUPercent.GetStuff() + $CPUPercent.Other();\nb = $CPUPercent.Another()",
                new []
                {
                    new Diagnostic(UnknownMethodNameRule.Descriptor, UnknownMethodNameRule.FormatMessage("GetStuff")),
                    new Diagnostic(UnknownMethodNameRule.Descriptor, UnknownMethodNameRule.FormatMessage("Other")),
                    new Diagnostic(UnknownMethodNameRule.Descriptor, UnknownMethodNameRule.FormatMessage("Another")),
                }
            },

            new object[]
            {
                "Multiple unknown method names with parse errors",
                "a = $CPUPercent.GetStuff() + $CPUPercent.Other()^+ $CPUPercent.NotReported();\nb = $CPUPercent.Another()^",
                new []
                {
                    // All of the parse errors appear...
                    new Diagnostic(ParseError.Descriptor, ParseException.FormatUnexpectedTokenMessage(AutoScaleTokenType.Semicolon, MakeUnknownToken())),
                    new Diagnostic(ParseError.Descriptor, ParseException.FormatUnexpectedTokenMessage(AutoScaleTokenType.Semicolon, MakeUnknownToken())),

                    // ... before any of the errors reported by the diagnostic rules.
                    new Diagnostic(UnknownMethodNameRule.Descriptor, UnknownMethodNameRule.FormatMessage("GetStuff")),
                    new Diagnostic(UnknownMethodNameRule.Descriptor, UnknownMethodNameRule.FormatMessage("Other")),
                    new Diagnostic(UnknownMethodNameRule.Descriptor, UnknownMethodNameRule.FormatMessage("Another")),
                }
            }
        };

        private static AutoScaleToken MakeUnknownToken()
        {
            return new AutoScaleToken(AutoScaleTokenType.Unknown, 0, 0, "^");
        }

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
