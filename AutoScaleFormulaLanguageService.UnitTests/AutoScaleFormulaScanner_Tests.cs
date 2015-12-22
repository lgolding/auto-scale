using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.Package;
using Xunit;

namespace Lakewood.AutoScaleFormulaLanguageService.UnitTests
{
    public class AutoScaleFormulaScanner_Tests
    {
        public static IEnumerable<object[]> ScannerTestCases => new[]
        {
            new object[]
            {
                "Empty line",
                "",
                new TokenInfo[0]
            },

            new object[]
            {
                "Delimiters",
                "(,,);",
                new[]
                {
                    MakeTokenInfo(0, 0, TokenType.Delimiter),
                    MakeTokenInfo(1, 1, TokenType.Delimiter),
                    MakeTokenInfo(2, 2, TokenType.Delimiter),
                    MakeTokenInfo(3, 3, TokenType.Delimiter, TokenColor.Text, TokenTriggers.MatchBraces),
                    MakeTokenInfo(4, 4, TokenType.Delimiter)
                }
            },

            new object[]
            {
                "Single-character operators",
                "+-/*!<>?.:",
                new[]
                {
                    MakeTokenInfo(0, 0, TokenType.Operator),
                    MakeTokenInfo(1, 1, TokenType.Operator),
                    MakeTokenInfo(2, 2, TokenType.Operator),
                    MakeTokenInfo(3, 3, TokenType.Operator),
                    MakeTokenInfo(4, 4, TokenType.Operator),
                    MakeTokenInfo(5, 5, TokenType.Operator),
                    MakeTokenInfo(6, 6, TokenType.Operator),
                    MakeTokenInfo(7, 7, TokenType.Operator),
                    MakeTokenInfo(8, 8, TokenType.Operator, TokenColor.Text, TokenTriggers.MemberSelect),
                    MakeTokenInfo(9, 9, TokenType.Operator)
                }
            },

            new object[]
            {
                "Multi-character and single-character operators",
                "<<=>>====!=",
                new[]
                {
                    MakeTokenInfo(0, 0, TokenType.Operator),   // "<"
                    MakeTokenInfo(1, 2, TokenType.Operator),   // "<="
                    MakeTokenInfo(3, 3, TokenType.Operator),   // ">"
                    MakeTokenInfo(4, 5, TokenType.Operator),   // ">="
                    MakeTokenInfo(6, 7, TokenType.Operator),   // "=="
                    MakeTokenInfo(8, 8, TokenType.Operator),   // "="
                    MakeTokenInfo(9, 10, TokenType.Operator)   // "!="
                }
            },

            new object[]
            {
                "Logical operators",
                "a&&(b || c)",
                new[]
                {
                    MakeTokenInfo(0, 0, TokenType.Identifier, TokenColor.Identifier),
                    MakeTokenInfo(1, 2, TokenType.Operator),
                    MakeTokenInfo(3, 3, TokenType.Delimiter),
                    MakeTokenInfo(4, 4, TokenType.Identifier, TokenColor.Identifier),
                    MakeTokenInfo(5, 5, TokenType.WhiteSpace),
                    MakeTokenInfo(6, 7, TokenType.Operator),
                    MakeTokenInfo(8, 8, TokenType.WhiteSpace),
                    MakeTokenInfo(9, 9, TokenType.Identifier, TokenColor.Identifier),
                    MakeTokenInfo(10, 10, TokenType.Delimiter, TokenColor.Text, TokenTriggers.MatchBraces)
                }
            },

            new object[]
            {
                "Unknown tokens",
                "(^)@#(",
                new[]
                {
                    MakeTokenInfo(0, 0, TokenType.Delimiter),
                    MakeTokenInfo(1, 1, TokenType.Unknown),
                    MakeTokenInfo(2, 2, TokenType.Delimiter, TokenColor.Text, TokenTriggers.MatchBraces),
                    MakeTokenInfo(3, 3, TokenType.Unknown),
                    MakeTokenInfo(4, 4, TokenType.Unknown),
                    MakeTokenInfo(5, 5, TokenType.Delimiter)
                }
            },

            new object[]
            {
                "All white space",
                "    \t\t  \t  ",
                new[]
                {
                    MakeTokenInfo(0, 10, TokenType.WhiteSpace)
                }
            },

            new object[]
            {
                "White space mixed with other tokens",
                "(  <=!  ) ;  \t ",
                new[]
                {
                    MakeTokenInfo(0, 0, TokenType.Delimiter),
                    MakeTokenInfo(1, 2, TokenType.WhiteSpace),
                    MakeTokenInfo(3, 4, TokenType.Operator),   // "<="
                    MakeTokenInfo(5, 5, TokenType.Operator),   // "!"
                    MakeTokenInfo(6, 7, TokenType.WhiteSpace),
                    MakeTokenInfo(8, 8, TokenType.Delimiter, TokenColor.Text, TokenTriggers.MatchBraces),
                    MakeTokenInfo(9, 9, TokenType.WhiteSpace),
                    MakeTokenInfo(10, 10, TokenType.Delimiter),
                    MakeTokenInfo(11, 14, TokenType.WhiteSpace)
                }
            },

            new object[]
            {
                "Identifiers",
                "a_b_cd = xy+q_2 - $Abc;",
                new[]
                {
                    MakeTokenInfo(0, 5, TokenType.Identifier, TokenColor.Identifier),
                    MakeTokenInfo(6, 6, TokenType.WhiteSpace),
                    MakeTokenInfo(7, 7, TokenType.Operator),
                    MakeTokenInfo(8, 8, TokenType.WhiteSpace),
                    MakeTokenInfo(9, 10, TokenType.Identifier, TokenColor.Identifier),
                    MakeTokenInfo(11, 11, TokenType.Operator),
                    MakeTokenInfo(12, 14, TokenType.Identifier, TokenColor.Identifier),
                    MakeTokenInfo(15, 15, TokenType.WhiteSpace),
                    MakeTokenInfo(16, 16, TokenType.Operator),
                    MakeTokenInfo(17, 17, TokenType.WhiteSpace),
                    MakeTokenInfo(18, 21, TokenType.Identifier, TokenColor.Identifier),
                    MakeTokenInfo(22, 22, TokenType.Delimiter)
                }
            },

            new object[]
            {
                "Whole line comments",
                "// Whole line comment ",
                new[]
                {
                    MakeTokenInfo(0, 21, TokenType.LineComment, TokenColor.Comment)
                }
            },

            new object[]
            {
                "End line comments",
                "a = b; // Rest of line",
                new[]
                {
                    MakeTokenInfo(0, 0, TokenType.Identifier, TokenColor.Identifier),
                    MakeTokenInfo(1, 1, TokenType.WhiteSpace),
                    MakeTokenInfo(2, 2, TokenType.Operator),
                    MakeTokenInfo(3, 3, TokenType.WhiteSpace),
                    MakeTokenInfo(4, 4, TokenType.Identifier, TokenColor.Identifier),
                    MakeTokenInfo(5, 5, TokenType.Delimiter),
                    MakeTokenInfo(6, 6, TokenType.WhiteSpace),
                    MakeTokenInfo(7, 21, TokenType.LineComment, TokenColor.Comment),
                }
            },

            new object[]
            {
                "Numeric literals",
                "400+0.2",
                new[]
                {
                    MakeTokenInfo(0, 2, TokenType.Literal, TokenColor.String),
                    MakeTokenInfo(3, 3, TokenType.Operator),
                    MakeTokenInfo(4, 6, TokenType.Literal, TokenColor.String)
                }
            },

            new object[]
            {
                "Keywords",
                "$NodeDeallocationOption = requeue;",
                new[]
                {
                    MakeTokenInfo(0, 22, TokenType.Identifier, TokenColor.Identifier),
                    MakeTokenInfo(23, 23, TokenType.WhiteSpace),
                    MakeTokenInfo(24, 24, TokenType.Operator),
                    MakeTokenInfo(25, 25, TokenType.WhiteSpace),
                    MakeTokenInfo(26, 32, TokenType.Identifier, TokenColor.Keyword),
                    MakeTokenInfo(33, 33, TokenType.Delimiter)
                }
            },

            new object[]
            {
                "Multiple lines",
                "// Comment \na=1;",
                new[]
                {
                    MakeTokenInfo(0, 11, TokenType.LineComment, TokenColor.Comment),
                    MakeTokenInfo(12, 12, TokenType.Identifier, TokenColor.Identifier),
                    MakeTokenInfo(13, 13, TokenType.Operator),
                    MakeTokenInfo(14, 14, TokenType.Literal, TokenColor.String),
                    MakeTokenInfo(15, 15, TokenType.Delimiter)
                }
            },

            new object[]
            {
                "Realistic test",
                "$TotalNodes = (min($CPUPercent.GetSample(TimeInterval_Minute*10)) > 0.7) ? ($CurrentDedicated * 1.1) : $CurrentDedicated;",
                new[]
                {
                    MakeTokenInfo(0, 10, TokenType.Identifier, TokenColor.Identifier),      // "$TotalNodes"
                    MakeTokenInfo(11, 11, TokenType.WhiteSpace),                            // " "
                    MakeTokenInfo(12, 12, TokenType.Operator),                              // "="
                    MakeTokenInfo(13, 13, TokenType.WhiteSpace),                            // " "
                    MakeTokenInfo(14, 14, TokenType.Delimiter),                             // "("
                    MakeTokenInfo(15, 17, TokenType.Identifier, TokenColor.Identifier),     // "min"
                    MakeTokenInfo(18, 18, TokenType.Delimiter),                             // "("
                    MakeTokenInfo(19, 29, TokenType.Identifier, TokenColor.Identifier),     // "$CPUPercent"
                    MakeTokenInfo(30, 30, TokenType.Operator, TokenColor.Text, TokenTriggers.MemberSelect), // "."
                    MakeTokenInfo(31, 39, TokenType.Identifier, TokenColor.Identifier),     // "GetSample"
                    MakeTokenInfo(40, 40, TokenType.Delimiter),                             // "("
                    MakeTokenInfo(41, 59, TokenType.Identifier, TokenColor.Identifier),     // "TimeInterval_Minute"
                    MakeTokenInfo(60, 60, TokenType.Operator),                              // "*"
                    MakeTokenInfo(61, 62, TokenType.Literal, TokenColor.String),            // "10"
                    MakeTokenInfo(63, 63, TokenType.Delimiter, TokenColor.Text, TokenTriggers.MatchBraces), // ")"
                    MakeTokenInfo(64, 64, TokenType.Delimiter, TokenColor.Text, TokenTriggers.MatchBraces), // ")"
                    MakeTokenInfo(65, 65, TokenType.WhiteSpace),                            // " "
                    MakeTokenInfo(66, 66, TokenType.Operator),                              // ">"
                    MakeTokenInfo(67, 67, TokenType.WhiteSpace),                            // " "
                    MakeTokenInfo(68, 70, TokenType.Literal, TokenColor.String),            // "0.7"
                    MakeTokenInfo(71, 71, TokenType.Delimiter, TokenColor.Text, TokenTriggers.MatchBraces),                             // ")"
                    MakeTokenInfo(72, 72, TokenType.WhiteSpace),                            // " "
                    MakeTokenInfo(73, 73, TokenType.Operator),                              // ">"
                    MakeTokenInfo(74, 74, TokenType.WhiteSpace),                            // " "
                    MakeTokenInfo(75, 75, TokenType.Delimiter),                             // "("
                    MakeTokenInfo(76, 92, TokenType.Identifier, TokenColor.Identifier),     // "$CurrentDedicated"
                    MakeTokenInfo(93, 93, TokenType.WhiteSpace),                            // " "
                    MakeTokenInfo(94, 94, TokenType.Operator),                              // "*"
                    MakeTokenInfo(95, 95, TokenType.WhiteSpace),                            // " "
                    MakeTokenInfo(96, 98, TokenType.Literal, TokenColor.String),            // "1.1"
                    MakeTokenInfo(99, 99, TokenType.Delimiter, TokenColor.Text, TokenTriggers.MatchBraces), // ")"
                    MakeTokenInfo(100, 100, TokenType.WhiteSpace),                          // " "
                    MakeTokenInfo(101, 101, TokenType.Operator),                            // ":"
                    MakeTokenInfo(102, 102, TokenType.WhiteSpace),                          // " "
                    MakeTokenInfo(103, 119, TokenType.Identifier, TokenColor.Identifier),   // "$CurrentDedicated"
                    MakeTokenInfo(120, 120, TokenType.Delimiter),                           // ";"
                }
            }
        };

        [Theory]
        [MemberData(nameof(ScannerTestCases))]
        public void Scanner_produces_expected_tokens(string testName, string input, TokenInfo[] expectedTokens)
        {
            // Arrange.
            var scanner = new AutoScaleFormulaScanner(null) as IScanner;
            scanner.SetSource(input, 0);

            var tokens = new List<TokenInfo>();
            int state = 0;

            // Act.
            for (var tokenInfo = new TokenInfo();
                scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state);
                tokenInfo = new TokenInfo())
            {
                tokens.Add(
                    MakeTokenInfo(
                        tokenInfo.StartIndex,
                        tokenInfo.EndIndex,
                        tokenInfo.Type,
                        tokenInfo.Color,
                        tokenInfo.Trigger));
            }

            // Assert.
            tokens.Count.Should().Be(expectedTokens.Length);
            tokens.Select(t => t.StartIndex).Should().ContainInOrder(expectedTokens.Select(t => t.StartIndex));
            tokens.Select(t => t.EndIndex).Should().ContainInOrder(expectedTokens.Select(t => t.EndIndex));
            tokens.Select(t => t.Type).Should().ContainInOrder(expectedTokens.Select(t => t.Type));
            tokens.Select(t => t.Color).Should().ContainInOrder(expectedTokens.Select(t => t.Color));
            tokens.Select(t => t.Trigger).Should().ContainInOrder(expectedTokens.Select(t => t.Trigger));
        }

        private static TokenInfo MakeTokenInfo(
            int startIndex,
            int endIndex,
            TokenType type,
            TokenColor color = TokenColor.Text,
            TokenTriggers triggers = TokenTriggers.None)
        {
            return new TokenInfo(startIndex, endIndex, type)
            {
                Color = color,
                Trigger = triggers
            };
        }
    }
}
