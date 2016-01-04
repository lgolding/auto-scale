using Lakewood.AutoScale.Diagnostics;
using Lakewood.AutoScale.Diagnostics.Rules;
using Xunit;

namespace Lakewood.AutoScale.UnitTests.Diagnostics.Rules
{
    public class InvalidMethodInvocationTarget_Tests : DiagnosticRuleTestBase
    {
        public static readonly object[] TestCases = new object[]
        {
            new object[]
            {
                "Valid method invocation on sampling system variable",
                "a = $CPUPercent.GetSample()",
                new Diagnostic[0]
            },

            new object[]
            {
                "Invalid method invocation on non-sampling system variable",
                "a = $TargetDedicated.GetSample()",
                new []
                {
                    new Diagnostic(
                        InvalidMethodInvocationTargetRule.Descriptor,
                        InvalidMethodInvocationTargetRule.FormatMessage("$TargetDedicated"),
                        4, 19)
                }
            },

            new object[]
            {
                "Invalid method invocation on user-defined variable",
                "a = abc.GetSample()",
                new []
                {
                    new Diagnostic(
                        InvalidMethodInvocationTargetRule.Descriptor,
                        InvalidMethodInvocationTargetRule.FormatMessage("abc"),
                        4, 6)
                }
            },
        };

        [Theory]
        [MemberData(nameof(TestCases))]
        public void Produces_expected_diagnostics(string testName, string input, Diagnostic[] expectedDiagnostics)
        {
            RunTestCase(testName, input, expectedDiagnostics);
        }
    }
}
