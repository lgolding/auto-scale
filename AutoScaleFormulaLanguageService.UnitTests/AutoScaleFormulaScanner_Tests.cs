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
            // Delimiters
            new object[]
            {
                "();",
                new[]
                {
                    new TokenInfo { StartIndex = 0, EndIndex = 0, Type = TokenType.Delimiter },
                    new TokenInfo { StartIndex = 1, EndIndex = 1, Type = TokenType.Delimiter },
                    new TokenInfo { StartIndex = 2, EndIndex = 2, Type = TokenType.Delimiter }
                }
            },

            // Single-character operators
            new object[]
            {
                "+-/*!<>",
                new[]
                {
                    new TokenInfo { StartIndex = 0, EndIndex = 0, Type = TokenType.Operator },
                    new TokenInfo { StartIndex = 1, EndIndex = 1, Type = TokenType.Operator },
                    new TokenInfo { StartIndex = 2, EndIndex = 2, Type = TokenType.Operator },
                    new TokenInfo { StartIndex = 3, EndIndex = 3, Type = TokenType.Operator },
                    new TokenInfo { StartIndex = 4, EndIndex = 4, Type = TokenType.Operator },
                    new TokenInfo { StartIndex = 5, EndIndex = 5, Type = TokenType.Operator },
                    new TokenInfo { StartIndex = 6, EndIndex = 6, Type = TokenType.Operator }
                }
            },

            // Multi-character and single-character operators
            new object[]
            {
                "<<=>>===!=",
                new[]
                {
                    new TokenInfo { StartIndex = 0, EndIndex = 0, Type = TokenType.Operator }, // "<"
                    new TokenInfo { StartIndex = 1, EndIndex = 2, Type = TokenType.Operator }, // "<="
                    new TokenInfo { StartIndex = 3, EndIndex = 3, Type = TokenType.Operator }, // ">"
                    new TokenInfo { StartIndex = 4, EndIndex = 5, Type = TokenType.Operator }, // ">="
                    new TokenInfo { StartIndex = 6, EndIndex = 7, Type = TokenType.Operator }, // "=="
                    new TokenInfo { StartIndex = 8, EndIndex = 9, Type = TokenType.Operator }  // "!="
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
