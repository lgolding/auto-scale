using FluentAssertions;
using Lakewood.AutoScale.Diagnostics;
using Xunit;

namespace Lakewood.AutoScale.UnitTests.Diagnostics
{
    public class InvalidAssignmentFromNodeDeallocationOptionKeyword_Tests
    {
        public static readonly object[] TestCases = new object[]
        {
            new object[]
            {
                "Valid assignment to $NodeDeallocationOption",
                "$NodeDeallocationOption = taskcompletion",
                new Diagnostic[0]
            },

            new object[]
            {
                "Invalid assignment to any other variable",
                "abc = taskcompletion",
                new []
                {
                    new Diagnostic(
                        InvalidAssignmentFromNodeDeallocationOptionKeywordRule.Descriptor,
                        InvalidAssignmentFromNodeDeallocationOptionKeywordRule.FormatMessage("taskcompletion", "abc"),
                        6, 19)
                }
            }
        };

        [Theory]
        [MemberData(nameof(TestCases))]
        public void Produces_expected_diagnostics(string testName, string input, Diagnostic[] expectedDiagnostics)
        {
            var parser = new Parser(input);

            parser.Parse();

            parser.Diagnostics.Count.Should().Be(expectedDiagnostics.Length);
            parser.Diagnostics.Should().ContainInOrder(expectedDiagnostics);
        }
    }
}
