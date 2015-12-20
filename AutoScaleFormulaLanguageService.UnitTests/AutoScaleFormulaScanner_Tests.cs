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
                    new TokenInfo { StartIndex = 0, EndIndex = 0, Type = TokenType.Delimiter },
                    new TokenInfo { StartIndex = 1, EndIndex = 1, Type = TokenType.Delimiter },
                    new TokenInfo { StartIndex = 2, EndIndex = 2, Type = TokenType.Delimiter },
                    new TokenInfo { StartIndex = 3, EndIndex = 3, Type = TokenType.Delimiter },
                    new TokenInfo { StartIndex = 4, EndIndex = 4, Type = TokenType.Delimiter }
                }
            },

            // Single-character operators
            new object[]
            {
                "+-/*!<>?.:",
                new[]
                {
                    new TokenInfo { StartIndex = 0, EndIndex = 0, Type = TokenType.Operator },
                    new TokenInfo { StartIndex = 1, EndIndex = 1, Type = TokenType.Operator },
                    new TokenInfo { StartIndex = 2, EndIndex = 2, Type = TokenType.Operator },
                    new TokenInfo { StartIndex = 3, EndIndex = 3, Type = TokenType.Operator },
                    new TokenInfo { StartIndex = 4, EndIndex = 4, Type = TokenType.Operator },
                    new TokenInfo { StartIndex = 5, EndIndex = 5, Type = TokenType.Operator },
                    new TokenInfo { StartIndex = 6, EndIndex = 6, Type = TokenType.Operator },
                    new TokenInfo { StartIndex = 7, EndIndex = 7, Type = TokenType.Operator },
                    new TokenInfo { StartIndex = 8, EndIndex = 8, Type = TokenType.Operator },
                    new TokenInfo { StartIndex = 9, EndIndex = 9, Type = TokenType.Operator }
                }
            },

            // Multi-character and single-character operators
            new object[]
            {
                "<<=>>====!=",
                new[]
                {
                    new TokenInfo { StartIndex = 0, EndIndex = 0, Type = TokenType.Operator },  // "<"
                    new TokenInfo { StartIndex = 1, EndIndex = 2, Type = TokenType.Operator },  // "<="
                    new TokenInfo { StartIndex = 3, EndIndex = 3, Type = TokenType.Operator },  // ">"
                    new TokenInfo { StartIndex = 4, EndIndex = 5, Type = TokenType.Operator },  // ">="
                    new TokenInfo { StartIndex = 6, EndIndex = 7, Type = TokenType.Operator },  // "=="
                    new TokenInfo { StartIndex = 8, EndIndex = 8, Type = TokenType.Operator },  // "="
                    new TokenInfo { StartIndex = 9, EndIndex = 10, Type = TokenType.Operator }  // "!="
                }
            },

            // Unknown tokens.
            new object[]
            {
                "(^)@#(",
                new[]
                {
                    new TokenInfo { StartIndex = 0, EndIndex = 0, Type = TokenType.Delimiter },
                    new TokenInfo { StartIndex = 1, EndIndex = 1, Type = TokenType.Unknown },
                    new TokenInfo { StartIndex = 2, EndIndex = 2, Type = TokenType.Delimiter },
                    new TokenInfo { StartIndex = 3, EndIndex = 3, Type = TokenType.Unknown },
                    new TokenInfo { StartIndex = 4, EndIndex = 4, Type = TokenType.Unknown },
                    new TokenInfo { StartIndex = 5, EndIndex = 5, Type = TokenType.Delimiter }
                }
            },

            // All white space.
            new object[]
            {
                "    \t\t  \t  ",
                new[]
                {
                    new TokenInfo { StartIndex = 0, EndIndex = 10, Type = TokenType.WhiteSpace }
                }
            },

            // White space mixed with other tokens.
            new object[]
            {
                "(  <=!  ) ;  \t ",
                new[]
                {
                    new TokenInfo { StartIndex = 0, EndIndex = 0, Type = TokenType.Delimiter },
                    new TokenInfo { StartIndex = 1, EndIndex = 2, Type = TokenType.WhiteSpace },
                    new TokenInfo { StartIndex = 3, EndIndex = 4, Type = TokenType.Operator }, // "<="
                    new TokenInfo { StartIndex = 5, EndIndex = 5, Type = TokenType.Operator }, // "!"
                    new TokenInfo { StartIndex = 6, EndIndex = 7, Type = TokenType.WhiteSpace },
                    new TokenInfo { StartIndex = 8, EndIndex = 8, Type = TokenType.Delimiter },
                    new TokenInfo { StartIndex = 9, EndIndex = 9, Type = TokenType.WhiteSpace },
                    new TokenInfo { StartIndex = 10, EndIndex = 10, Type = TokenType.Delimiter },
                    new TokenInfo { StartIndex = 11, EndIndex = 14, Type = TokenType.WhiteSpace }
                }
            },

            // Identifiers.
            new object[]
            {
                "a_b_cd = xy+q_2 - $Abc;",
                new[]
                {
                    new TokenInfo { StartIndex = 0, EndIndex = 5, Type = TokenType.Identifier },
                    new TokenInfo { StartIndex = 6, EndIndex = 6, Type = TokenType.WhiteSpace },
                    new TokenInfo { StartIndex = 7, EndIndex = 7, Type = TokenType.Operator },
                    new TokenInfo { StartIndex = 8, EndIndex = 8, Type = TokenType.WhiteSpace },
                    new TokenInfo { StartIndex = 9, EndIndex = 10, Type = TokenType.Identifier },
                    new TokenInfo { StartIndex = 11, EndIndex = 11, Type = TokenType.Operator },
                    new TokenInfo { StartIndex = 12, EndIndex = 14, Type = TokenType.Identifier },
                    new TokenInfo { StartIndex = 15, EndIndex = 15, Type = TokenType.WhiteSpace },
                    new TokenInfo { StartIndex = 16, EndIndex = 16, Type = TokenType.Operator },
                    new TokenInfo { StartIndex = 17, EndIndex = 17, Type = TokenType.WhiteSpace },
                    new TokenInfo { StartIndex = 18, EndIndex = 21, Type = TokenType.Identifier },
                    new TokenInfo { StartIndex = 22, EndIndex = 22, Type = TokenType.Delimiter }
                }
            },

            // Whole line comments.
            new object[]
            {
                "// Whole line comment ",
                new[]
                {
                    new TokenInfo { StartIndex = 0, EndIndex = 21, Type = TokenType.Comment }
                }
            },

            // End line comments.
            new object[]
            {
                "a = b; // Rest of line",
                new[]
                {
                    new TokenInfo { StartIndex = 0, EndIndex = 0, Type = TokenType.Identifier },
                    new TokenInfo { StartIndex = 1, EndIndex = 1, Type = TokenType.WhiteSpace },
                    new TokenInfo { StartIndex = 2, EndIndex = 2, Type = TokenType.Operator },
                    new TokenInfo { StartIndex = 3, EndIndex = 3, Type = TokenType.WhiteSpace },
                    new TokenInfo { StartIndex = 4, EndIndex = 4, Type = TokenType.Identifier },
                    new TokenInfo { StartIndex = 5, EndIndex = 5, Type = TokenType.Delimiter },
                    new TokenInfo { StartIndex = 6, EndIndex = 6, Type = TokenType.WhiteSpace },
                    new TokenInfo { StartIndex = 7, EndIndex = 21, Type = TokenType.Comment },
                }
            },

            // Numeric literals
            new object[]
            {
                "400+0.2",
                new[]
                {
                    new TokenInfo { StartIndex = 0, EndIndex = 2, Type = TokenType.Literal },
                    new TokenInfo { StartIndex = 3, EndIndex = 3, Type = TokenType.Operator },
                    new TokenInfo { StartIndex = 4, EndIndex = 6, Type = TokenType.Literal }
                }
            },

            // Realistic test
            new object[]
            {
                "$TotalNodes = (min($CPUPercent.GetSample(TimeInterval_Minute*10)) > 0.7) ? ($CurrentDedicated * 1.1) : $CurrentDedicated;",
                new[]
                {
                    new TokenInfo { StartIndex =   0, EndIndex =  10, Type = TokenType.Identifier },    // "$TotalNodes"
                    new TokenInfo { StartIndex =  11, EndIndex =  11, Type = TokenType.WhiteSpace },    // " "
                    new TokenInfo { StartIndex =  12, EndIndex =  12, Type = TokenType.Operator },      // "="
                    new TokenInfo { StartIndex =  13, EndIndex =  13, Type = TokenType.WhiteSpace },    // " "
                    new TokenInfo { StartIndex =  14, EndIndex =  14, Type = TokenType.Delimiter },     // "("
                    new TokenInfo { StartIndex =  15, EndIndex =  17, Type = TokenType.Identifier },    // "min"
                    new TokenInfo { StartIndex =  18, EndIndex =  18, Type = TokenType.Delimiter },     // "("
                    new TokenInfo { StartIndex =  19, EndIndex =  29, Type = TokenType.Identifier },    // "$CPUPercent"
                    new TokenInfo { StartIndex =  30, EndIndex =  30, Type = TokenType.Operator },      // "."
                    new TokenInfo { StartIndex =  31, EndIndex =  39, Type = TokenType.Identifier },    // "GetSample"
                    new TokenInfo { StartIndex =  40, EndIndex =  40, Type = TokenType.Delimiter },     // "("
                    new TokenInfo { StartIndex =  41, EndIndex =  59, Type = TokenType.Identifier },    // "TimeInterval_Minute"
                    new TokenInfo { StartIndex =  60, EndIndex =  60, Type = TokenType.Operator },      // "*"
                    new TokenInfo { StartIndex =  61, EndIndex =  62, Type = TokenType.Literal },       // "10"
                    new TokenInfo { StartIndex =  63, EndIndex =  63, Type = TokenType.Delimiter },     // ")"
                    new TokenInfo { StartIndex =  64, EndIndex =  64, Type = TokenType.Delimiter },     // ")"
                    new TokenInfo { StartIndex =  65, EndIndex =  65, Type = TokenType.WhiteSpace },    // " "
                    new TokenInfo { StartIndex =  66, EndIndex =  66, Type = TokenType.Operator },      // ">"
                    new TokenInfo { StartIndex =  67, EndIndex =  67, Type = TokenType.WhiteSpace },    // " "
                    new TokenInfo { StartIndex =  68, EndIndex =  70, Type = TokenType.Literal },       // "0.7"
                    new TokenInfo { StartIndex =  71, EndIndex =  71, Type = TokenType.Delimiter },     // ")"
                    new TokenInfo { StartIndex =  72, EndIndex =  72, Type = TokenType.WhiteSpace },    // " "
                    new TokenInfo { StartIndex =  73, EndIndex =  73, Type = TokenType.Operator },      // ">"
                    new TokenInfo { StartIndex =  74, EndIndex =  74, Type = TokenType.WhiteSpace },    // " "
                    new TokenInfo { StartIndex =  75, EndIndex =  75, Type = TokenType.Delimiter },     // "("
                    new TokenInfo { StartIndex =  76, EndIndex =  92, Type = TokenType.Identifier },    // "$CurrentDedicated"
                    new TokenInfo { StartIndex =  93, EndIndex =  93, Type = TokenType.WhiteSpace },    // " "
                    new TokenInfo { StartIndex =  94, EndIndex =  94, Type = TokenType.Operator },      // "*"
                    new TokenInfo { StartIndex =  95, EndIndex =  95, Type = TokenType.WhiteSpace },    // " "
                    new TokenInfo { StartIndex =  96, EndIndex =  98, Type = TokenType.Literal },       // "1.1"
                    new TokenInfo { StartIndex =  99, EndIndex =  99, Type = TokenType.Delimiter },     // ")"
                    new TokenInfo { StartIndex = 100, EndIndex = 100, Type = TokenType.WhiteSpace },    // " "
                    new TokenInfo { StartIndex = 101, EndIndex = 101, Type = TokenType.Operator },      // ":"
                    new TokenInfo { StartIndex = 102, EndIndex = 102, Type = TokenType.WhiteSpace },    // " "
                    new TokenInfo { StartIndex = 103, EndIndex = 119, Type = TokenType.Identifier },    // "$CurrentDedicated"
                    new TokenInfo { StartIndex = 120, EndIndex = 120, Type = TokenType.Delimiter },     // ";"
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
