using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.Package;
using Xunit;

namespace Lakewood.AutoScaleFormulaLanguageService.UnitTests
{
    public class AutoScaleFormulaLanguageService_Tests
    {
        private List<BraceMatch> _matches;

        public static IEnumerable<object[]> ParenMatchingTestCases => new[]
        {
            new object[]
            {
                "Single pair",
                "x = (y + z);",
                new []
                {
                    new BraceMatch(4, 10)
                }
            },

            new object[]
            {
                "Multiple pairs",
                "x = (y + z) / (r + q);",
                new []
                {
                    new BraceMatch(4, 10),
                    new BraceMatch(14, 20)
                }
            },

            new object[]
            {
                "Nested pairs",
                "x = (y * (r + q) - d);",
                new []
                {
                    new BraceMatch(9, 15),
                    new BraceMatch(4, 20)
                }
            },

            new object[]
            {
                "Unmatched parens",
                "x = (y * (r + q) - d;",
                new []
                {
                    new BraceMatch(9, 15)
                }
            },

            new object[]
            {
                "Multiple lines",
                "x = (y *\n  (r\n  + q\n  ) - d\n  );",
                new []
                {
                    new BraceMatch(11, 22),
                    new BraceMatch(4, 30)
                }
            }
        };

        [Theory]
        [MemberData(nameof(ParenMatchingTestCases))]
        public void ParseSource_FindsMatchingParens(string testName, string input, BraceMatch[] expectedMatches)
        {
            const int Line = 1;
            const int Col = 1;
            const ParseReason Reason = ParseReason.HighlightBraces;
            const int MaxErrors = 100;
            const bool Synchronous = false;

            _matches = new List<BraceMatch>();

            var sink = new AuthoringSink(Reason, Line, Col, MaxErrors);
            var req = new ParseRequest(
                Line,
                Col,
                null, // info
                input,
                null, // fname
                Reason,
                null, // view
                sink,
                Synchronous);

            var target = new AutoScaleFormulaLanguageService();

            // Force the service to create its scanner. In the product, VS will call this
            // method, providing an IVsTextLines buffer object, before it calls ParseSource.
            target.GetScanner(null);

            target.BraceMatchFound += OnMatchedPairFound;

            // Act.
            target.ParseSource(req);

            // Assert.
            var actualMatches = _matches.ToArray();
            actualMatches.Length.Should().Be(expectedMatches.Length);

            for (int i = 0; i < expectedMatches.Length; ++i)
            {
                actualMatches[i].Left.Should().Be(expectedMatches[i].Left);
                actualMatches[i].Right.Should().Be(expectedMatches[i].Right);
            }
        }

        private void OnMatchedPairFound(object sender, BraceMatch e)
        {
            _matches.Add(e);
        }
    }
}
