using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;
using Xunit;

namespace Lakewood.AutoScaleFormulaLanguageService.UnitTests
{
    public class AutoScaleFormulaLanguageService_Tests
    {
        private List<BraceMatch> _matchedPairs;

        public static IEnumerable<object[]> ParenMatchingTestCases => new[]
        {
            new object[]
            {
                "Single pair",
                "x = (y + z);",
                new BraceMatch[]
                {
                    new BraceMatch(
                        MakeTextSpan(1, 4, 1, 4),
                        MakeTextSpan(1, 10, 1, 10))
                }
            },

            new object[]
            {
                "Multiple pairs",
                "x = (y + z) / (r + q);",
                new BraceMatch[]
                {
                    new BraceMatch(
                        MakeTextSpan(1, 4, 1, 4),
                        MakeTextSpan(1, 10, 1, 10)),
                    new BraceMatch(
                        MakeTextSpan(1, 14, 1, 14),
                        MakeTextSpan(1, 20, 1, 20))
                }
            },

            new object[]
            {
                "Nested pairs",
                "x = (y * (r + q) - d);",
                new BraceMatch[]
                {
                    new BraceMatch(
                        MakeTextSpan(1, 9, 1, 9),
                        MakeTextSpan(1, 15, 1, 15),
                        priority: 1),
                    new BraceMatch(
                        MakeTextSpan(1, 4, 1, 4),
                        MakeTextSpan(1, 20, 1, 20))
                }
            },

            new object[]
            {
                "Unmatched parens",
                "x = (y * (r + q) - d;",
                new BraceMatch[]
                {
                    new BraceMatch(
                        MakeTextSpan(1, 9, 1, 9),
                        MakeTextSpan(1, 15, 1, 15),
                        priority: 1)
                }
            },

            new object[]
            {
                "Multiple lines",
@"x = (y *
        (r 
         + q
         ) - d
       );",
                new BraceMatch[]
                {
                    new BraceMatch(
                        MakeTextSpan(2, 8, 2, 8),
                        MakeTextSpan(4, 9, 4, 9),
                        priority: 1),
                    new BraceMatch(
                        MakeTextSpan(1, 4, 1, 4),
                        MakeTextSpan(5, 7, 5, 7))
                }
            }
        };

        [Theory]
        [MemberData(nameof(ParenMatchingTestCases))]
        public void ParseSource_Check_FindsMatchingParens(string testName, string input, BraceMatch[] expectedPairs)
        {
            const int Line = 1;
            const int Col = 1;
            const ParseReason Reason = ParseReason.HighlightBraces;
            const int MaxErrors = 100;
            const bool Synchronous = false;

            _matchedPairs = new List<BraceMatch>();

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
            var actualPairs = _matchedPairs.ToArray();
            actualPairs.Length.Should().Be(expectedPairs.Length);

            for (int i = 0; i < expectedPairs.Length; ++i)
            {
                actualPairs[i].Start.iStartLine.Should().Be(expectedPairs[i].Start.iStartLine);
                actualPairs[i].Start.iStartIndex.Should().Be(expectedPairs[i].Start.iStartIndex);
                actualPairs[i].Start.iEndLine.Should().Be(expectedPairs[i].Start.iEndLine);
                actualPairs[i].Start.iEndIndex.Should().Be(expectedPairs[i].Start.iEndIndex);
                actualPairs[i].End.iStartLine.Should().Be(expectedPairs[i].End.iStartLine);
                actualPairs[i].End.iStartIndex.Should().Be(expectedPairs[i].End.iStartIndex);
                actualPairs[i].End.iEndLine.Should().Be(expectedPairs[i].End.iEndLine);
                actualPairs[i].End.iEndIndex.Should().Be(expectedPairs[i].End.iEndIndex);
                actualPairs[i].Priority.Should().Be(expectedPairs[i].Priority);
            }
        }

        private static TextSpan MakeTextSpan(int startLine, int startIndex, int endLine, int endIndex)
        {
            return new TextSpan
            {
                iStartLine = startLine,
                iStartIndex = startIndex,
                iEndLine = endLine,
                iEndIndex = endIndex
            };
        }

        private void OnMatchedPairFound(object sender, BraceMatch e)
        {
            _matchedPairs.Add(e);
        }
    }
}
