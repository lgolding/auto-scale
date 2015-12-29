using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.Package;
using Xunit;

namespace Lakewood.AutoScale.UnitTests
{
    public class Lexer_Tests
    {
        public static IEnumerable<object[]> LexerTestCases => new[]
        {
            new object[]
            {
                "Empty line",
                "",
                new AutoScaleToken[0]
            },

            new object[]
            {
                "Delimiters",
                "(,,);",
                new[]
                {
                    new AutoScaleToken(AutoScaleTokenType.ParenOpen, 0, 0, "("),
                    new AutoScaleToken(AutoScaleTokenType.Comma, 1, 1, ","),
                    new AutoScaleToken(AutoScaleTokenType.Comma, 2, 2, ","),
                    new AutoScaleToken(AutoScaleTokenType.ParenClose, 3, 3, ")"),
                    new AutoScaleToken(AutoScaleTokenType.Semicolon, 4, 4, ";")
                }
            },

            new object[]
            {
                "Single-character operators",
                "+-/*!<>?.:",
                new[]
                {
                    new AutoScaleToken(AutoScaleTokenType.OperatorAddition, 0, 0, "+"),
                    new AutoScaleToken(AutoScaleTokenType.OperatorSubtraction, 1, 1, "-"),
                    new AutoScaleToken(AutoScaleTokenType.OperatorDivision, 2, 2, "/"),
                    new AutoScaleToken(AutoScaleTokenType.OperatorMultiplication, 3, 3, "*"),
                    new AutoScaleToken(AutoScaleTokenType.OperatorNot, 4, 4, "!"),
                    new AutoScaleToken(AutoScaleTokenType.OperatorLessThan, 5, 5, "<"),
                    new AutoScaleToken(AutoScaleTokenType.OperatorGreaterThan, 6, 6, ">"),
                    new AutoScaleToken(AutoScaleTokenType.OperatorTernaryQuestion, 7, 7, "?"),
                    new AutoScaleToken(AutoScaleTokenType.OperatorMemberSelect, 8, 8, "."),
                    new AutoScaleToken(AutoScaleTokenType.OperatorTernaryColon, 9, 9, ":")
                }
            },

            new object[]
            {
                "Multi-character and single-character operators",
                "<<=>>====!=",
                new[]
                {
                    new AutoScaleToken(AutoScaleTokenType.OperatorLessThan, 0, 0, "<"),
                    new AutoScaleToken(AutoScaleTokenType.OperatorLessThanOrEqual, 1, 2, "<="),
                    new AutoScaleToken(AutoScaleTokenType.OperatorGreaterThan, 3, 3, ">"),
                    new AutoScaleToken(AutoScaleTokenType.OperatorGreaterThanOrEqual, 4, 5, ">="),
                    new AutoScaleToken(AutoScaleTokenType.OperatorEquality, 6, 7, "=="),
                    new AutoScaleToken(AutoScaleTokenType.OperatorAssign, 8, 8, "="),
                    new AutoScaleToken(AutoScaleTokenType.OperatorNotEqual, 9, 10, "!=")
                }
            },

            new object[]
            {
                "Logical operators",
                "a&&(b || c)",
                new[]
                {
                    new AutoScaleToken(AutoScaleTokenType.Identifier, 0, 0, "a"),
                    new AutoScaleToken(AutoScaleTokenType.OperatorLogicalAnd, 1, 2, "&&"),
                    new AutoScaleToken(AutoScaleTokenType.ParenOpen, 3, 3, "("),
                    new AutoScaleToken(AutoScaleTokenType.Identifier, 4, 4, "b"),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 5, 5, " "),
                    new AutoScaleToken(AutoScaleTokenType.OperatorLogicalOr, 6, 7, "||"),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 8, 8, " "),
                    new AutoScaleToken(AutoScaleTokenType.Identifier, 9, 9, "c"),
                    new AutoScaleToken(AutoScaleTokenType.ParenClose, 10, 10, ")")
                }
            },

            new object[]
            {
                "Unknown tokens",
                "(^)@#(",
                new[]
                {
                    new AutoScaleToken(AutoScaleTokenType.ParenOpen, 0, 0, "("),
                    new AutoScaleToken(AutoScaleTokenType.Unknown, 1, 1, "^"),
                    new AutoScaleToken(AutoScaleTokenType.ParenClose, 2, 2, ")"),
                    new AutoScaleToken(AutoScaleTokenType.Unknown, 3, 3, "@"),
                    new AutoScaleToken(AutoScaleTokenType.Unknown, 4, 4, "#"),
                    new AutoScaleToken(AutoScaleTokenType.ParenOpen, 5, 5, "(")
                }
            },

            new object[]
            {
                "All white space",
                "    \t\t  \t  ",
                new[]
                {
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 0, 10, "    \t\t  \t  ")
                }
            },

            new object[]
            {
                "White space mixed with other tokens",
                "(  <=!  ) ;  \t ",
                new[]
                {
                    new AutoScaleToken(AutoScaleTokenType.ParenOpen, 0, 0, "("),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 1, 2, "  "),
                    new AutoScaleToken(AutoScaleTokenType.OperatorLessThanOrEqual, 3, 4, "<="),
                    new AutoScaleToken(AutoScaleTokenType.OperatorNot, 5, 5, "!"),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 6, 7, "  "),
                    new AutoScaleToken(AutoScaleTokenType.ParenClose, 8, 8, ")"),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 9, 9, " "),
                    new AutoScaleToken(AutoScaleTokenType.Semicolon, 10, 10, ";"),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 11, 14, "  \t ")
                }
            },

            new object[]
            {
                "Identifiers",
                "a_b_cd = xy+q_2 - $Abc;",
                new[]
                {
                    new AutoScaleToken(AutoScaleTokenType.Identifier, 0, 5, "a_b_cd"),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 6, 6, " "),
                    new AutoScaleToken(AutoScaleTokenType.OperatorAssign, 7, 7, "="),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 8, 8, " "),
                    new AutoScaleToken(AutoScaleTokenType.Identifier, 9, 10, "xy"),
                    new AutoScaleToken(AutoScaleTokenType.OperatorAddition, 11, 11, "+"),
                    new AutoScaleToken(AutoScaleTokenType.Identifier, 12, 14, "q_2"),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 15, 15, " "),
                    new AutoScaleToken(AutoScaleTokenType.OperatorSubtraction, 16, 16, "-"),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 17, 17, " "),
                    new AutoScaleToken(AutoScaleTokenType.Identifier, 18, 21, "$Abc"),
                    new AutoScaleToken(AutoScaleTokenType.Semicolon, 22, 22, ";")
                }
            },

            new object[]
            {
                "Whole line comments",
                "// Whole line comment ",
                new[]
                {
                    new AutoScaleToken(AutoScaleTokenType.LineComment, 0, 21, "// Whole line comment ")
                }
            },

            new object[]
            {
                "End line comments",
                "a = b; // Rest of line",
                new[]
                {
                    new AutoScaleToken(AutoScaleTokenType.Identifier, 0, 0, "a"),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 1, 1, " "),
                    new AutoScaleToken(AutoScaleTokenType.OperatorAssign, 2, 2, "="),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 3, 3, " "),
                    new AutoScaleToken(AutoScaleTokenType.Identifier, 4, 4, "b"),
                    new AutoScaleToken(AutoScaleTokenType.Semicolon, 5, 5, ";"),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 6, 6, " "),
                    new AutoScaleToken(AutoScaleTokenType.LineComment, 7, 21, "// Rest of line")
                }
            },

            new object[]
            {
                "Numeric literals",
                "400+0.2",
                new[]
                {
                    new AutoScaleToken(AutoScaleTokenType.DoubleLiteral, 0, 2, "400"),
                    new AutoScaleToken(AutoScaleTokenType.OperatorAddition, 3, 3, "+"),
                    new AutoScaleToken(AutoScaleTokenType.DoubleLiteral, 4, 6, "0.2")
                }
            },

            new object[]
            {
                "Keywords",
                "$NodeDeallocationOption = requeue;",
                new[]
                {
                    new AutoScaleToken(AutoScaleTokenType.Identifier, 0, 22, "$NodeDeallocationOption"),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 23, 23, " "),
                    new AutoScaleToken(AutoScaleTokenType.OperatorAssign, 24, 24, "="),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 25, 25, " "),
                    new AutoScaleToken(AutoScaleTokenType.Keyword, 26, 32, "requeue"),
                    new AutoScaleToken(AutoScaleTokenType.Semicolon, 33, 33, ";")
                }
            },

            new object[]
            {
                "Multiple lines",
                "// Comment \na=1;",
                new[]
                {
                    new AutoScaleToken(AutoScaleTokenType.LineComment, 0, 11, "// Comment \n"),
                    new AutoScaleToken(AutoScaleTokenType.Identifier, 12, 12, "a"),
                    new AutoScaleToken(AutoScaleTokenType.OperatorAssign, 13, 13, "="),
                    new AutoScaleToken(AutoScaleTokenType.DoubleLiteral, 14, 14, "1"),
                    new AutoScaleToken(AutoScaleTokenType.Semicolon, 15, 15, ";")
                }
            },

            new object[]
            {
                "Realistic test",
                "$TotalNodes = (min($CPUPercent.GetSample(TimeInterval_Minute*10)) > 0.7) ? ($CurrentDedicated * 1.1) : $CurrentDedicated;",
                new[]
                {
                    new AutoScaleToken(AutoScaleTokenType.Identifier, 0, 10, "$TotalNodes"),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 11, 11, " "),
                    new AutoScaleToken(AutoScaleTokenType.OperatorAssign, 12, 12, "="),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 13, 13, " "),
                    new AutoScaleToken(AutoScaleTokenType.ParenOpen, 14, 14, "("),
                    new AutoScaleToken(AutoScaleTokenType.Identifier, 15, 17, "min"),
                    new AutoScaleToken(AutoScaleTokenType.ParenOpen, 18, 18, "("),
                    new AutoScaleToken(AutoScaleTokenType.Identifier, 19, 29, "$CPUPercent"),
                    new AutoScaleToken(AutoScaleTokenType.OperatorMemberSelect, 30, 30, "."),
                    new AutoScaleToken(AutoScaleTokenType.Identifier, 31, 39, "GetSample"),
                    new AutoScaleToken(AutoScaleTokenType.ParenOpen, 40, 40, "("),
                    new AutoScaleToken(AutoScaleTokenType.Identifier, 41, 59, "TimeInterval_Minute"),
                    new AutoScaleToken(AutoScaleTokenType.OperatorMultiplication, 60, 60, "*"),
                    new AutoScaleToken(AutoScaleTokenType.DoubleLiteral, 61, 62, "10"),
                    new AutoScaleToken(AutoScaleTokenType.ParenClose, 63, 63, ")"),
                    new AutoScaleToken(AutoScaleTokenType.ParenClose, 64, 64, ")"),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 65, 65, " "),
                    new AutoScaleToken(AutoScaleTokenType.OperatorGreaterThan, 66, 66, ">"),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 67, 67, " "),
                    new AutoScaleToken(AutoScaleTokenType.DoubleLiteral, 68, 70, "0.7"),
                    new AutoScaleToken(AutoScaleTokenType.ParenClose, 71, 71, ")"),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 72, 72, " "),
                    new AutoScaleToken(AutoScaleTokenType.OperatorTernaryQuestion, 73, 73, "?"),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 74, 74, " "),
                    new AutoScaleToken(AutoScaleTokenType.ParenOpen, 75, 75, "("),
                    new AutoScaleToken(AutoScaleTokenType.Identifier, 76, 92, "$CurrentDedicated"),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 93, 93, " "),
                    new AutoScaleToken(AutoScaleTokenType.OperatorMultiplication, 94, 94, "*"),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 95, 95, " "),
                    new AutoScaleToken(AutoScaleTokenType.DoubleLiteral, 96, 98, "1.1"),
                    new AutoScaleToken(AutoScaleTokenType.ParenClose, 99, 99, ")"),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 100, 100, " "),
                    new AutoScaleToken(AutoScaleTokenType.OperatorTernaryColon, 101, 101, ":"),
                    new AutoScaleToken(AutoScaleTokenType.WhiteSpace, 102, 102, " "),
                    new AutoScaleToken(AutoScaleTokenType.Identifier, 103, 119, "$CurrentDedicated"),
                    new AutoScaleToken(AutoScaleTokenType.Semicolon, 120, 120, ";")
                }
            }
        };

        [Theory]
        [MemberData(nameof(LexerTestCases))]
        public void Lexer_produces_expected_tokens(string testName, string input, AutoScaleToken[] expectedTokens)
        {
            // Arrange.
            var lexer = new Lexer(input);

            var tokens = new List<AutoScaleToken>();

            // Act.
            AutoScaleToken token;
            while (true)
            {
                token = lexer.GetNextToken();
                if (token.Type == AutoScaleTokenType.End)
                {
                    break;
                }

                tokens.Add(token);
            }

            // Assert.
            tokens.Count.Should().Be(expectedTokens.Length);
            tokens.Should().ContainInOrder(expectedTokens);
        }
    }
}
