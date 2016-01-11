// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using Lakewood.AutoScale.Diagnostics;
using Lakewood.AutoScale.Diagnostics.Rules;
using Xunit;

namespace Lakewood.AutoScale.UnitTests.Diagnostics.Rules
{
    public class InvalidFunctionSignatureRule_Tests : DiagnosticRuleTestBase
    {
        public static readonly object[] TestCases = new object[]
        {
            new object[]
            {
                "Valid call to 0-argument function",
                "a = rand()",
                new Diagnostic[0]
            },

            new object[]
            {
                "Invalid call to 0-argument function",
                "a = rand(2, 3)",
                new []
                {
                    new Diagnostic(
                        InvalidFunctionSignatureRule.Descriptor,
                        InvalidFunctionSignatureRule.FormatMessage("rand", 2),
                        8, 13)
                }
            },

            new object[]
            {
                "Valid call to 2-argument function",
                "a = percentile(v, 10)",
                new Diagnostic[0]
            },

            new object[]
            {
                "Too few arguments to 2-argument function",
                "a = percentile(v)",
                new []
                {
                    new Diagnostic(
                        InvalidFunctionSignatureRule.Descriptor,
                        InvalidFunctionSignatureRule.FormatMessage("percentile", 1),
                        14, 16)
                }
            },

            new object[]
            {
                "Too many arguments to 2-argument function",
                "a = percentile(v, 1, 2)",
                new []
                {
                    new Diagnostic(
                        InvalidFunctionSignatureRule.Descriptor,
                        InvalidFunctionSignatureRule.FormatMessage("percentile", 3),
                        14, 22)
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
