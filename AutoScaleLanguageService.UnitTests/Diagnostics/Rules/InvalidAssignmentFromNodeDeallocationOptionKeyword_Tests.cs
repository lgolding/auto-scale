using Lakewood.AutoScale.Diagnostics;
using Lakewood.AutoScale.Diagnostics.Rules;
using Xunit;

namespace Lakewood.AutoScale.UnitTests.Diagnostics.Rules
{
    public class InvalidAssignmentFromNodeDeallocationOptionKeyword_Tests : DiagnosticRuleTestBase
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
            RunTestCase(testName, input, expectedDiagnostics);
        }
    }
}
