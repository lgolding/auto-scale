﻿using Lakewood.AutoScale.Diagnostics;
using Xunit;

namespace Lakewood.AutoScale.UnitTests.Diagnostics
{
    public class UnknownFunctionName_Tests: DiagnosticRuleTestBase
    {
        public static readonly object[] TestCases = new object[]
        {
            new object[]
            {
                "Known function name",
                "a = rand()",
                new Diagnostic[0]
            },

            new object[]
            {
                "Unknown function name",
                "a = random()",
                new []
                {
                    new Diagnostic(UnknownFunctionNameRule.Descriptor, UnknownFunctionNameRule.FormatMessage("random"), 4, 9),
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