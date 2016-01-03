﻿using FluentAssertions;
using Lakewood.AutoScale.Diagnostics;
using Xunit;

namespace Lakewood.AutoScale.UnitTests.Diagnostics
{
    public class InvalidAssignmentToNodeDeallocationOption_Tests
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
                "Invalid assignment to $NodeDeallocationOption",
                "$NodeDeallocationOption = 2 + 3",
                new []
                {
                    new Diagnostic(
                        InvalidAssignmentToNodeDeallocationOptionRule.Descriptor,
                        InvalidAssignmentToNodeDeallocationOptionRule.FormatMessage(),
                        26, 30)
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