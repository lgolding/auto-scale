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
        // Parameters common to all tests. The values are arbitrary; we don't have any
        // logic that depends on them.
        private const int Sink_MaxErrors = 100;
        private const bool Request_Synchronous = true;

        public static IEnumerable<object[]> BraceMatchingTestCases => new[]
        {
            new object[]
            {
                "Single pair",
                /* input: */ "x = (y + z);",
                /* caretLine, caretCol: */ 0, 11,
                /* matchLine, matchCol: */ 0, 4
            },

            new object[]
            {
                "Multiple pairs",
                /* input: */ "x = (y + z) / (r + q);",
                /* caretLine, caretCol: */ 0, 21,
                /* matchLine, matchCol: */ 0, 14
            },

            new object[]
            {
                "Nested pairs",
                /* input: */ "x = (y * (r + q) - d);",
                /* caretLine, caretCol: */ 0, 16,
                /* matchLine, matchCol: */ 0, 9
            },

            new object[]
            {
                "Unmatched parens",
                /* input: */ "x = y * (r + q) - d);",
                /* caretLine, caretCol: */ 0, 20,
                /* matchLine, matchCol: */ null, null
            },

            new object[]
            {
                "Multiple lines",
                // /* input: */ "x = (y *\n  (r\n  + q\n  ) - d\n  );",
                // /* caretLine, caretCol: */ 3, 3,
                // /* matchIndex: */ 11
                /* input: */ "a=\n (1+2);",
                /* caretLine, caretCol: */ 1, 6,
                /* matchLine, matchCol: */ 1, 1
            }
        };

        [Theory]
        [MemberData(nameof(BraceMatchingTestCases))]
        public void ParseSource_FindsMatchingParens(string testName, string input, int caretLine, int caretCol, int? matchLine, int? matchCol)
        {
            const ParseReason Reason = ParseReason.HighlightBraces;

            var sink = new TestAuthoringSink(Reason, caretLine, caretCol, Sink_MaxErrors);
            var req = new ParseRequest(
                caretLine,
                caretCol,
                null, // info
                input,
                null, // fname
                Reason,
                null, // view
                sink,
                Request_Synchronous);

            var target = new TestAutoScaleLanguageService(input);

            IScanner scanner = target.GetScanner(null);
            scanner.SetSource(input, 0);

            // Act.
            target.ParseSource(req);

            // Assert.
            sink.FoundMatchingBrace.Should().Be(matchLine.HasValue);

            if (sink.FoundMatchingBrace)
            {
                // The product code stores that matches with the span containing the
                // caret as the "start", and the span containing the matching brace as
                // the "end".
                var match = sink.SpanMatches.Single().End;

                match.iStartLine.Should().Be(matchLine);
                match.iStartIndex.Should().Be(matchCol);
            }
        }

        public static readonly object[] MemberSelectTestCases = new object[]
        {
            new object[]
            {
                "System variables get the Intellisense list",
                "$CPUPercent.",
                /* caretLine, caretCol: */ 0, 12,
                AutoScaleLanguageService.SamplingVariableMethods
            },

            new object[]
            {
                "Other variables get an empty list",
                "myVariable.",
                /* caretLine, caretCol: */ 0, 11,
                new AutoScaleDeclaration[0]
            },

            new object[]
            {
                "No list if immediately preceding token is not the identifier",
                "$CPUPercent..",
                /* caretLine, caretCol: */ 0, 13,
                new AutoScaleDeclaration[0]
            },

            new object[]
            {
                "Inserting . between identifiers gives the correct list",
                "$CPUPercent.GetSample()",
                /* caretLine, caretCol: */ 0, 12,
                AutoScaleLanguageService.SamplingVariableMethods
            },

            new object[]
            {
                "Intervening white space does not prevent list from being shown",
                "$CPUPercent .",
                /* caretLine, caretCol: */ 0, 13,
                AutoScaleLanguageService.SamplingVariableMethods
            },

            new object[]
            {
                "Intervening comment space does not prevent list from being shown",
                "$CPUPercent//\n.",
                /* caretLine, caretCol: */ 1, 0,
                AutoScaleLanguageService.SamplingVariableMethods
            }
        };

        [Theory]
        [MemberData(nameof(MemberSelectTestCases))]
        public void ParseSource_ProducesMemberSelectionList(string testName, string input, int caretLine, int caretCol, AutoScaleDeclaration[] expectedDeclarations)
        {
            const ParseReason Reason = ParseReason.MemberSelect;

            var sink = new TestAuthoringSink(Reason, caretLine, caretCol, Sink_MaxErrors);
            var req = new ParseRequest(
                caretLine,
                caretCol,
                null, // info
                input,
                null, // fname
                Reason,
                null, // view
                sink,
                Request_Synchronous);

            var target = new TestAutoScaleLanguageService(input);

            IScanner scanner = target.GetScanner(null);
            scanner.SetSource(input, 0);

            // The Source object will call the authoring scope's GetDeclaration with these
            // parameters, but our implementation of AuthoringScope doesn't use them.
            IVsTextView view = null;
            int declLine = 0, declCol = 0;
            TokenInfo tokenInfo = new TokenInfo();

            // Act.
            var authoringScope = target.ParseSource(req) as AutoScaleAuthoringScope;
            Declarations declarations = authoringScope.GetDeclarations(view, declLine, declCol, tokenInfo, Reason);

            // Assert.
            declarations.GetCount().Should().Be(expectedDeclarations.Length);
            for (int i = 0; i < declarations.GetCount(); ++i)
            {
                declarations.GetName(i).Should().Be(expectedDeclarations[i].Name);
                declarations.GetDisplayText(i).Should().Be(expectedDeclarations[i].Name);
                declarations.GetDescription(i).Should().Be(expectedDeclarations[i].Description);
                declarations.GetGlyph(i).Should().Be(expectedDeclarations[i].TypeImageIndex);
            }
        }

        public static readonly object[] DisplayMemberListTestCases = new object[]
        {
            new object[]
            {
                "System variables and functions",
                "$CPUPercent",
                /* caretLine, caretCol: */ 0, 5,
                AutoScaleLanguageService.AllBuiltInIdentifiers.Select(decl => decl.Name)
            },

            new object[]
            {
                "Sampling system variable members",
                "$CPUPercent.GetSample()",
                /* caretLine, caretCol: */ 0, 14,
                SamplingVariableMethodName.All
            },

            new object[]
            {
                "System variables, functions, and user-defined variables",
                "myVariable = $CPUPercent.GetSample();\navgVal = avg(myVariable);",
                /* caretLine, caretCol: */ 0, 20,
                AutoScaleLanguageService.AllBuiltInIdentifiers.Select(decl => decl.Name)
                    .Union(new[] { "myVariable", "avgVal" })
                    .OrderBy(s => s)
            }
        };

        [Theory]
        [MemberData(nameof(DisplayMemberListTestCases))]
        public void ParseSource_ProducesMemberList(string testName, string input, int caretLine, int caretCol, string[] expectedDeclarationNames)
        {
            const ParseReason Reason = ParseReason.DisplayMemberList;

            var sink = new TestAuthoringSink(Reason, caretLine, caretCol, Sink_MaxErrors);
            var req = new ParseRequest(
                caretLine,
                caretCol,
                null, // info
                input,
                null, // fname
                Reason,
                null, // view
                sink,
                Request_Synchronous);

            var target = new TestAutoScaleLanguageService(input);

            IScanner scanner = target.GetScanner(null);
            scanner.SetSource(input, 0);

            // The Source object will call the authoring scope's GetDeclaration with these
            // parameters, but our implementation of AuthoringScope doesn't use them.
            IVsTextView view = null;
            int declLine = 0, declCol = 0;
            TokenInfo tokenInfo = new TokenInfo();

            // Act.
            var authoringScope = target.ParseSource(req) as AutoScaleAuthoringScope;
            var declarations = authoringScope.GetDeclarations(view, declLine, declCol, tokenInfo, Reason)
                as AutoScaleDeclarations;

            // Assert.
            declarations.Names.ShouldBeEquivalentTo(expectedDeclarationNames);
        }

        private class TestAutoScaleLanguageService : AutoScaleLanguageService
        {
            private readonly string _input;

            public TestAutoScaleLanguageService(string input)
            {
                _input = input;
            }

            // The unit tests only use newlines for line breaks; no need to worry about
            // CRLF.
            public override void GetLineIndexOfPosition(int index, out int line, out int col)
            {
                string beforeCaret = _input.Substring(0, index);

                line = beforeCaret.Count(ch => ch == '\n');
                col = line == 0 ? index : index - beforeCaret.LastIndexOf('\n') - 1;
            }

            public override int GetPositionOfLineIndex(int line, int col)
            {
                int caretLine = 0;
                int caretCol = 0;
                int index = 0;

                while ((caretLine != line || caretCol != col) && index <= _input.Length)
                {
                    ++index;

                    if (_input[index - 1] == '\n')
                    {
                        ++caretLine;
                        caretCol = 0;
                    }
                    else
                    {
                        ++caretCol;
                    }
                }

                return index;
            }
        }

        private class SpanMatch
        {
            private readonly TextSpan _start;
            private readonly TextSpan _end;

            public SpanMatch(TextSpan start, TextSpan end)
            {
                _start = start;
                _end = end;
            }

            public TextSpan Start => _start;
            public TextSpan End => _end;
        }

        private class TestAuthoringSink : AuthoringSink
        {
            private List<SpanMatch> _spanMatches = new List<SpanMatch>();

            public TestAuthoringSink(ParseReason reason, int line, int col, int maxErrors)
                : base(reason, line, col, maxErrors)
            {
            }

            public override void MatchPair(TextSpan span, TextSpan endContext, int priority)
            {
                _spanMatches.Add(new SpanMatch(span, endContext));

                base.MatchPair(span, endContext, priority);
            }

            public List<SpanMatch> SpanMatches => _spanMatches;
        }
    }
}
