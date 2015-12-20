﻿using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.Package;
using Xunit;

namespace Lakewood.AutoScaleFormulaLanguageService.UnitTests
{
    public class AutoScaleFormulaScanner_Tests
    {
        public static IEnumerable<object[]> ScannerData => new[]
        {
            new object[] { "()", new TokenType[] { TokenType.Delimiter, TokenType.Delimiter } }
        };

        [Theory]
        [MemberData(nameof(ScannerData))]
        public void Scanner_produces_expected_tokens(string input, TokenType[] expectedTokenTypes)
        {
            // Arrange.
            var scanner = new AutoScaleFormulaScanner(null) as IScanner;
            scanner.SetSource(input, 0);

            var tokenTypes = new List<TokenType>();
            var tokenInfo = new TokenInfo();
            int state = 0;

            // Act.
            while (scanner.ScanTokenAndProvideInfoAboutIt(tokenInfo, ref state))
            {
                tokenTypes.Add(tokenInfo.Type);
            }

            // Assert.
            tokenTypes.Count.Should().Be(expectedTokenTypes.Length);
            tokenTypes.Should().ContainInOrder(expectedTokenTypes);
        }
    }
}
