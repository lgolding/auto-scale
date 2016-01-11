// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using Lakewood.AutoScale.Diagnostics;
using Lakewood.AutoScale.Diagnostics.Rules;
using Xunit;

namespace Lakewood.AutoScale.UnitTests.Diagnostics.Rules
{
    public class InvalidMethodSignatureRule_Tests : DiagnosticRuleTestBase
    {
        public static readonly object[] TestCases = new object[]
        {
            new object[]
            {
                "Valid call to Count",
                "a = $ActiveTasks.Count()",
                new Diagnostic[0]
            },

            new object[]
            {
                "Invalid call to Count",
                "a = $ActiveTasks.Count(42, 54)",
                new []
                {
                    new Diagnostic(
                        InvalidMethodSignatureRule.Descriptor,
                        InvalidMethodSignatureRule.FormatMessage("Count", 2),
                        22, 29)
                }
            },

            new object[]
            {
                "Valid one-argument call to GetSample",
                "a = $ActiveTasks.GetSample(10)",
                new Diagnostic[0]
            },

            new object[]
            {
                "Valid two-argument call to GetSample",
                "a = $ActiveTasks.GetSample(TimeInterval_Minute * 15, 80)",
                new Diagnostic[0]
            },

            new object[]
            {
                "Valid three-argument call to GetSample",
                "a = $ActiveTasks.GetSample(TimeInterval_Minute * 15, TimeInterval_Minute * 30, 80)",
                new Diagnostic[0]
            },

            new object[]
            {
                "Invalid call to GetSample",
                "a = $ActiveTasks.GetSample(TimeInterval_Minute * 15, TimeInterval_Minute * 30, 80, 42)",
                new Diagnostic[]
                {
                    new Diagnostic(
                        InvalidMethodSignatureRule.Descriptor,
                        InvalidMethodSignatureRule.FormatMessage("GetSample", 4),
                        26, 85)
                }
            },

            new object[]
            {
                "Valid one-argument call to GetSamplePercent",
                "a = $ActiveTasks.GetSamplePercent(TimeInterval_Minute * 15)",
                new Diagnostic[0]
            },

            new object[]
            {
                "Valid two-argument call to GetSamplePercent",
                "a = $ActiveTasks.GetSamplePercent(TimeInterval_Minute * 15, TimeInterval_Minute * 30)",
                new Diagnostic[0]
            },

            new object[]
            {
                "Invalid call to GetSamplePercent",
                "a = $ActiveTasks.GetSamplePercent()",
                new []
                {
                    new Diagnostic(
                        InvalidMethodSignatureRule.Descriptor,
                        InvalidMethodSignatureRule.FormatMessage("GetSamplePercent", 0),
                        33, 34)
                }
            },

            new object[]
            {
                "Valid call to GetSamplePeriod",
                "a = $ActiveTasks.GetSamplePeriod()",
                new Diagnostic[0]
            },

            new object[]
            {
                "Invalid call to GetSamplePeriod",
                "a = $ActiveTasks.GetSamplePeriod(42)",
                new []
                {
                    new Diagnostic(
                        InvalidMethodSignatureRule.Descriptor,
                        InvalidMethodSignatureRule.FormatMessage("GetSamplePeriod", 1),
                        32, 35)
                }
            },

            new object[]
            {
                "Valid call to HistoryBeginTime",
                "a = $ActiveTasks.HistoryBeginTime()",
                new Diagnostic[0]
            },

            new object[]
            {
                "Invalid call to HistoryBeginTime",
                "a = $ActiveTasks.HistoryBeginTime(42)",
                new []
                {
                    new Diagnostic(
                        InvalidMethodSignatureRule.Descriptor,
                        InvalidMethodSignatureRule.FormatMessage("HistoryBeginTime", 1),
                        33, 36)
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
