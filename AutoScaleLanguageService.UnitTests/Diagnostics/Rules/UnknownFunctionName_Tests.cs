// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using Lakewood.AutoScale.Diagnostics;
using Lakewood.AutoScale.Diagnostics.Rules;
using Xunit;

namespace Lakewood.AutoScale.UnitTests.Diagnostics.Rules
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
