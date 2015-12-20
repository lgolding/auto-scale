using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.Package;
using Xunit;

namespace Lakewood.AutoScaleFormulaLanguageService.UnitTests
{
    public class AutoScaleFormulaScanner_Tests
    {
        public static IEnumerable<object[]> ScannerData => new[]
        {
            // Empty line
            new object[]
            {
                "",
                new TokenInfo[0]
            },

            // Delimiters
            new object[]
            {
                "(,,);",
                new[]
                {
                    new TokenInfo(0, 0, TokenType.Delimiter),
                    new TokenInfo(1, 1, TokenType.Delimiter),
                    new TokenInfo(2, 2, TokenType.Delimiter),
                    new TokenInfo(3, 3, TokenType.Delimiter),
                    new TokenInfo(4, 4, TokenType.Delimiter)
                }
            },

            // Single-character operators
            new object[]
            {
                "+-/*!<>?.:",
                new[]
                {
                    new TokenInfo(0, 0, TokenType.Operator),
                    new TokenInfo(1, 1, TokenType.Operator),
                    new TokenInfo(2, 2, TokenType.Operator),
                    new TokenInfo(3, 3, TokenType.Operator),
                    new TokenInfo(4, 4, TokenType.Operator),
                    new TokenInfo(5, 5, TokenType.Operator),
                    new TokenInfo(6, 6, TokenType.Operator),
                    new TokenInfo(7, 7, TokenType.Operator),
                    new TokenInfo(8, 8, TokenType.Operator),
                    new TokenInfo(9, 9, TokenType.Operator)
                }
            },

            // Multi-character and single-character operators
            new object[]
            {
                "<<=>>====!=",
                new[]
                {
                    new TokenInfo(0, 0, TokenType.Operator),   // "<"
                    new TokenInfo(1, 2, TokenType.Operator),   // "<="
                    new TokenInfo(3, 3, TokenType.Operator),   // ">"
                    new TokenInfo(4, 5, TokenType.Operator),   // ">="
                    new TokenInfo(6, 7, TokenType.Operator),   // "=="
                    new TokenInfo(8, 8, TokenType.Operator),   // "="
                    new TokenInfo(9, 10, TokenType.Operator)   // "!="
                }
            },

            // Unknown tokens.
            new object[]
            {
                "(^)@#(",
                new[]
                {
                    new TokenInfo(0, 0, TokenType.Delimiter),
                    new TokenInfo(1, 1, TokenType.Unknown),
                    new TokenInfo(2, 2, TokenType.Delimiter),
                    new TokenInfo(3, 3, TokenType.Unknown),
                    new TokenInfo(4, 4, TokenType.Unknown),
                    new TokenInfo(5, 5, TokenType.Delimiter)
                }
            },

            // All white space.
            new object[]
            {
                "    \t\t  \t  ",
                new[]
                {
                    new TokenInfo(0, 10, TokenType.WhiteSpace)
                }
            },

            // White space mixed with other tokens.
            new object[]
            {
                "(  <=!  ) ;  \t ",
                new[]
                {
                    new TokenInfo(0, 0, TokenType.Delimiter),
                    new TokenInfo(1, 2, TokenType.WhiteSpace),
                    new TokenInfo(3, 4, TokenType.Operator),   // "<="
                    new TokenInfo(5, 5, TokenType.Operator),   // "!"
                    new TokenInfo(6, 7, TokenType.WhiteSpace),
                    new TokenInfo(8, 8, TokenType.Delimiter),
                    new TokenInfo(9, 9, TokenType.WhiteSpace),
                    new TokenInfo(10, 10, TokenType.Delimiter),
                    new TokenInfo(11, 14, TokenType.WhiteSpace)
                }
            },

            // Identifiers.
            new object[]
            {
                "a_b_cd = xy+q_2 - $Abc;",
                new[]
                {
                    new TokenInfo(0, 5, TokenType.Identifier),
                    new TokenInfo(6, 6, TokenType.WhiteSpace),
                    new TokenInfo(7, 7, TokenType.Operator),
                    new TokenInfo(8, 8, TokenType.WhiteSpace),
                    new TokenInfo(9, 10, TokenType.Identifier),
                    new TokenInfo(11, 11, TokenType.Operator),
                    new TokenInfo(12, 14, TokenType.Identifier),
                    new TokenInfo(15, 15, TokenType.WhiteSpace),
                    new TokenInfo(16, 16, TokenType.Operator),
                    new TokenInfo(17, 17, TokenType.WhiteSpace),
                    new TokenInfo(18, 21, TokenType.Identifier),
                    new TokenInfo(22, 22, TokenType.Delimiter)
                }
            },

            // Whole line comments.
            new object[]
            {
                "// Whole line comment ",
                new[]
                {
                    new TokenInfo(0, 21, TokenType.Comment)
                }
            },

            // End line comments.
            new object[]
            {
                "a = b; // Rest of line",
                new[]
                {
                    new TokenInfo(0, 0, TokenType.Identifier),
                    new TokenInfo(1, 1, TokenType.WhiteSpace),
                    new TokenInfo(2, 2, TokenType.Operator),
                    new TokenInfo(3, 3, TokenType.WhiteSpace),
                    new TokenInfo(4, 4, TokenType.Identifier),
                    new TokenInfo(5, 5, TokenType.Delimiter),
                    new TokenInfo(6, 6, TokenType.WhiteSpace),
                    new TokenInfo(7, 21, TokenType.Comment),
                }
            },

            // Numeric literals
            new object[]
            {
                "400+0.2",
                new[]
                {
                    new TokenInfo(0, 2, TokenType.Literal),
                    new TokenInfo(3, 3, TokenType.Operator),
                    new TokenInfo(4, 6, TokenType.Literal)
                }
            },

            // Realistic test
            new object[]
            {
                "$TotalNodes = (min($CPUPercent.GetSample(TimeInterval_Minute*10)) > 0.7) ? ($CurrentDedicated * 1.1) : $CurrentDedicated;",
                new[]
                {
                    new TokenInfo(0, 10, TokenType.Identifier),     // "$TotalNodes"
                    new TokenInfo(11, 11, TokenType.WhiteSpace),    // " "
                    new TokenInfo(12, 12, TokenType.Operator),      // "="
                    new TokenInfo(13, 13, TokenType.WhiteSpace),    // " "
                    new TokenInfo(14, 14, TokenType.Delimiter),     // "("
                    new TokenInfo(15, 17, TokenType.Identifier),    // "min"
                    new TokenInfo(18, 18, TokenType.Delimiter),     // "("
                    new TokenInfo(19, 29, TokenType.Identifier),    // "$CPUPercent"
                    new TokenInfo(30, 30, TokenType.Operator),      // "."
                    new TokenInfo(31, 39, TokenType.Identifier),    // "GetSample"
                    new TokenInfo(40, 40, TokenType.Delimiter),     // "("
                    new TokenInfo(41, 59, TokenType.Identifier),    // "TimeInterval_Minute"
                    new TokenInfo(60, 60, TokenType.Operator),      // "*"
                    new TokenInfo(61, 62, TokenType.Literal),       // "10"
                    new TokenInfo(63, 63, TokenType.Delimiter),     // ")"
                    new TokenInfo(64, 64, TokenType.Delimiter),     // ")"
                    new TokenInfo(65, 65, TokenType.WhiteSpace),    // " "
                    new TokenInfo(66, 66, TokenType.Operator),      // ">"
                    new TokenInfo(67, 67, TokenType.WhiteSpace),    // " "
                    new TokenInfo(68, 70, TokenType.Literal),       // "0.7"
                    new TokenInfo(71, 71, TokenType.Delimiter),     // ")"
                    new TokenInfo(72, 72, TokenType.WhiteSpace),    // " "
                    new TokenInfo(73, 73, TokenType.Operator),      // ">"
                    new TokenInfo(74, 74, TokenType.WhiteSpace),    // " "
                    new TokenInfo(75, 75, TokenType.Delimiter),     // "("
                    new TokenInfo(76, 92, TokenType.Identifier),    // "$CurrentDedicated"
                    new TokenInfo(93, 93, TokenType.WhiteSpace),    // " "
                    new TokenInfo(94, 94, TokenType.Operator),      // "*"
                    new TokenInfo(95, 95, TokenType.WhiteSpace),    // " "
                    new TokenInfo(96, 98, TokenType.Literal),       // "1.1"
                    new TokenInfo(99, 99, TokenType.Delimiter),     // ")"
                    new TokenInfo(100, 100, TokenType.WhiteSpace),  // " "
                    new TokenInfo(101, 101, TokenType.Operator),    // ":"
                    new TokenInfo(102, 102, TokenType.WhiteSpace),  // " "
                    new TokenInfo(103, 119, TokenType.Identifier),  // "$CurrentDedicated"
                    new TokenInfo(120, 120, TokenType.Delimiter),   // ";"
                }
            }
        };

        [Theory]
        [MemberData(nameof(ScannerData))]
        public void Scanner_produces_expected_tokens(string input, TokenInfo[] expectedTokens)
        {
            // Arrange.
            var scanner = new AutoScaleFormulaScanner(null) as IScanner;
            scanner.SetSource(input, 0);

            var tokens = new List<TokenInfo>();
            var tokenInfo = new TokenInfo();
            int state = 0;

            // Act.
            while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state))
            {
                tokens.Add(new TokenInfo(tokenInfo.StartIndex, tokenInfo.EndIndex, tokenInfo.Type));
            }

            // Assert.
            tokens.Count.Should().Be(expectedTokens.Length);
            tokens.Select(t => t.StartIndex).Should().ContainInOrder(expectedTokens.Select(t => t.StartIndex));
            tokens.Select(t => t.EndIndex).Should().ContainInOrder(expectedTokens.Select(t => t.EndIndex));
            tokens.Select(t => t.Type).Should().ContainInOrder(expectedTokens.Select(t => t.Type));
        }
    }
}
