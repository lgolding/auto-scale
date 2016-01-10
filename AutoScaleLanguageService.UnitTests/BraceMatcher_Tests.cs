// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Lakewood.AutoScale.UnitTests
{
    public class BraceMatcher_Tests
    {
        public static readonly object[] TestCases = new object[]
        {
            new object[]
            {
                "No parentheses",
                "a = 1",
                new BraceMatch[0]
            },

            new object[]
            {
                "Single parenthesized expression",
                "a = (1)",
                new []
                {
                    new BraceMatch(4, 6)
                }
            },

            new object[]
            {
                "Multiple parenthesized expressions",
                "a = (1 + 5) / (3 + 3)",
                new []
                {
                    new BraceMatch(4, 10),
                    new BraceMatch(14, 20)
                }
            },

            new object[]
            {
                "Nested parenthesized expressions",
                "a = (1 + (10 / 5) + 3)",
                new []
                {
                    new BraceMatch(9, 16),
                    new BraceMatch(4, 21)
                }
            },

            new object[]
            {
                "Unmatched parentheses",
                "a = 1 + (10 / 5) + 3)",
                new []
                {
                    new BraceMatch(8, 15)
                }
            },

            new object[]
            {
                "Parenthesized expressions spanning multiple lines",
                "a = (1 *\n  (2\n  + 3\n  ) - 4\n  );",
                new []
                {
                    new BraceMatch(11, 22),
                    new BraceMatch(4, 30)
                }
            },

            new object[]
            {
                "Single function call",
                "a = sum(1, 2)",
                new []
                {
                    new BraceMatch(7, 12)
                }
            },

            new object[]
            {
                "Multiple function calls",
                "a = sum(1, 2) + sum(3, 4)",
                new []
                {
                    new BraceMatch(7, 12),
                    new BraceMatch(19, 24)
                }
            },

            new object[]
            {
                "Nested function calls",
                "a = sum(1, sum(3, 4))",
                new []
                {
                    new BraceMatch(14, 19),
                    new BraceMatch(7, 20)
                }
            },

            new object[]
            {
                "Function call within parenthesized expression",
                "a = (1 + sum(2, 3))",
                new []
                {
                    new BraceMatch(12, 17),
                    new BraceMatch(4, 18)
                }
            },

            new object[]
            {
                "Parenthesized expression within function call",
                "a = sum(1, (2 + 3))",
                new []
                {
                    new BraceMatch(11, 17),
                    new BraceMatch(7, 18)
                }
            },

            new object[]
            {
                "Single method invocation",
                "a = $b.sum(1, 2)",
                new []
                {
                    new BraceMatch(10, 15)
                }
            },

            new object[]
            {
                "Multiple method invocations",
                "a = $b.sum(1, 2) + $b.sum(3, 4)",
                new []
                {
                    new BraceMatch(10, 15),
                    new BraceMatch(25, 30)
                }
            },

            new object[]
            {
                "Nested method invocations",
                "a = $b.sum(1, $b.sum(3, 4))",
                new []
                {
                    new BraceMatch(20, 25),
                    new BraceMatch(10, 26)
                }
            },

            new object[]
            {
                "Method invocation within parenthesized expression",
                "a = (1 + $b.sum(2, 3))",
                new []
                {
                    new BraceMatch(15, 20),
                    new BraceMatch(4, 21)
                }
            },

            new object[]
            {
                "Parenthesized expression within method invocation",
                "a = $b.sum(1, (2 + 3))",
                new []
                {
                    new BraceMatch(14, 20),
                    new BraceMatch(10, 21)
                }
            },

            new object[]
            {
                "Mixed parenthesized expressions, function calls, and method invocations",
                "a = $b.sum( avg(1, (2 + 3)), (sum(3, 4) + 5) )",
                new []
                {
                    new BraceMatch(19, 25),
                    new BraceMatch(15, 26),
                    new BraceMatch(33, 38),
                    new BraceMatch(29, 43),
                    new BraceMatch(10, 45)
                }
            }
        };

        [Theory]
        [MemberData(nameof(TestCases))]
        public void Produces_expected_matches(string testName, string input, BraceMatch[] expectedMatches)
        {
            var parser = new Parser();
            var formulaNode = parser.Parse(input);

            var braceMatcher = new BraceMatcher();
            braceMatcher.FindMatches(formulaNode);

            braceMatcher.Matches.Count.Should().Be(expectedMatches.Length);
            braceMatcher.Matches.Should().ContainInOrder(expectedMatches);
        }
    }
}
