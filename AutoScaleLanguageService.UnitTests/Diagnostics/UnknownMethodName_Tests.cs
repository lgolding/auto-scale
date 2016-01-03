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
                    "ASF0002"
                }
            },

            new object[]
            {
                "Multiple unknown method names",
                "a = $CPUPercent.GetStuff() + $CPUPercent.Other();\nb = $CPUPercent.Another()",
                new []
                {
                    "ASF0002",
                    "ASF0002",
                    "ASF0002"
                }
            },

            new object[]
            {
                "Multiple unknown method names with parse errors",
                "a = $CPUPercent.GetStuff() + $CPUPercent.Other()^+ $CPUPercent.NotReported();\nb = $CPUPercent.Another()^",
                new []
                {
                    // All of the parse errors appear...
                    ParseError.Descriptor.DiagnosticId,
                    ParseError.Descriptor.DiagnosticId,

                    // ... before any of the errors reported by the diagnostic rules.
                    "ASF0002",
                    "ASF0002",
                    "ASF0002"
                }
            }
        };

        [Theory]
        [MemberData(nameof(ValidMethodNameTestCases))]
        public void Produces_expected_errors(string testName, string input, string[] expectedErrors)
        {
            var parser = new Parser(input);

            parser.Parse();

            parser.Errors.Should().ContainInOrder(expectedErrors);
        }
    }
}
