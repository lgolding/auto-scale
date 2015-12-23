using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;
using Xunit;

namespace Lakewood.AutoScale.UnitTests
{
    public class AutoScaleLanguageService_Tests
    {
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
            const int Line = 0;
            const int Col = 0;
            const ParseReason Reason = ParseReason.HighlightBraces;
            const int MaxErrors = 100;
            const bool Synchronous = false;

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

            var target = new AutoScaleLanguageService();

            IScanner scanner = target.GetScanner(null);
            scanner.SetSource(input, 0);

            // Act.

            // I'd have preferred to test through the public API, LanguageService.ParseSource.
            // But the implementation uses a Source object obtained from the view, and we don't
            // have a view. IMO it wasn't worth the effort to try to work around this.
            var tokens = target.TokenizeFile(req);
            var actualMatches = target.FindBraceMatches(tokens, input).ToArray();

            // Assert.
            actualMatches.Length.Should().Be(expectedMatches.Length);

            for (int i = 0; i < expectedMatches.Length; ++i)
            {
                actualMatches[i].Left.Should().Be(expectedMatches[i].Left);
                actualMatches[i].Right.Should().Be(expectedMatches[i].Right);
            }
        }

        public static readonly object[] MemberListTestCases = new object[]
        {
            new object[]
            {
                "$TargetDedicated."
            }
        };

        [Theory]
        [MemberData(nameof(MemberListTestCases))]
        public void ParseSource_ProducesMemberListForBuiltInObjects(string input)
        {
            const int Line = 0;
            const int Col = 17;
            const ParseReason Reason = ParseReason.MemberSelect;
            const int MaxErrors = 100;
            const bool Synchronous = false;

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

            var target = new AutoScaleLanguageService();

            IScanner scanner = target.GetScanner(null);
            scanner.SetSource(input, 0);

            // The Source object will call the authoring scope's GetDeclaration with these
            // parameters, but our implentation of AuthoringScope doesn't use them.
            IVsTextView view = null;
            int declLine = 0, declCol = 0;
            TokenInfo tokenInfo = new TokenInfo();

            // Act.
            var authoringScope = target.ParseSource(req) as AutoScaleAuthoringScope;
            Declarations declarations = authoringScope.GetDeclarations(view, declLine, declCol, tokenInfo, Reason);

            // Assert.
            declarations.GetCount().Should().Be(AutoScaleLanguageService.SystemVariables.Length);
            for (int i = 0; i < declarations.GetCount(); ++i)
            {
                declarations.GetDescription(i).Should().Be(AutoScaleLanguageService.SystemVariables[i].Description);
            }
        }
    }
}
