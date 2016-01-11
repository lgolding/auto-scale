// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using Lakewood.AutoScale.Diagnostics;
using Lakewood.AutoScale.Diagnostics.Rules;
using Xunit;

namespace Lakewood.AutoScale.UnitTests.Diagnostics.Rules
{
    public class UnknownMethodName_Tests : DiagnosticRuleTestBase
    {
        // This was the first diagnostic rule to be written, so it has test cases
        // to ensure that we can detect multiple violations, and mixtures of rule
        // violations with parse errors. For subsequent rules, we'll generally have
        // just one negative case and one positive case, unless the rule is complex
        // and requires multiple test cases.
        public static readonly object[] TestCases = new object[]
        {
            new object[]
            {
                "Known method name",
                "a = $CPUPercent.GetSample(10)",
                new Diagnostic[0]
            },

            new object[]
            {
                "Unknown method name",
                "a = $CPUPercent.GetStuff()",
                new []
                {
                    new Diagnostic(UnknownMethodNameRule.Descriptor, UnknownMethodNameRule.FormatMessage("GetStuff"), 16, 23),
                }
            },

            new object[]
            {
                "Multiple unknown method names",
                "a = $CPUPercent.GetStuff() + $CPUPercent.Other();\nb = $CPUPercent.Another()",
                new []
                {
                    new Diagnostic(UnknownMethodNameRule.Descriptor, UnknownMethodNameRule.FormatMessage("GetStuff"), 16, 23),
                    new Diagnostic(UnknownMethodNameRule.Descriptor, UnknownMethodNameRule.FormatMessage("Other"), 41, 45),
                    new Diagnostic(UnknownMethodNameRule.Descriptor, UnknownMethodNameRule.FormatMessage("Another"), 66, 72),
                }
            },

            new object[]
            {
                "Multiple unknown method names with parse errors",
                "a = $CPUPercent.GetStuff() + $CPUPercent.Other()^+ $CPUPercent.NotReported();\nb = $CPUPercent.Another()^",
                new []
                {
                    // All of the parse errors appear...
                    new Diagnostic(
                        ParserError.Descriptor,
                        ParserError.UnexpectedTokenMessage(
                            TokenFactory.MakeUnknownToken("^", 48),
                            AutoScaleTokenType.Semicolon),
                        48, 48),
                    new Diagnostic(
                        ParserError.Descriptor,
                        ParserError.UnexpectedTokenMessage(
                            TokenFactory.MakeUnknownToken("^", 103),
                            AutoScaleTokenType.Semicolon), 
                        103, 103),

                    // ... before any of the errors reported by the diagnostic rules.
                    new Diagnostic(UnknownMethodNameRule.Descriptor, UnknownMethodNameRule.FormatMessage("GetStuff"), 16, 23),
                    new Diagnostic(UnknownMethodNameRule.Descriptor, UnknownMethodNameRule.FormatMessage("Other"), 41, 45),
                    new Diagnostic(UnknownMethodNameRule.Descriptor, UnknownMethodNameRule.FormatMessage("Another"), 94, 100),
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
