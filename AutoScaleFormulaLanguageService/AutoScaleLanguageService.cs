using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Lakewood.AutoScale
{
    public class AutoScaleLanguageService : LanguageService
    {
        public const string LanguageName = "AutoScaleFormula";

        private LanguagePreferences _preferences;
        private IScanner _scanner;

        internal static readonly AutoScaleDeclaration[] SystemVariables = new AutoScaleDeclaration[]
        {
            // TODO: Descriptions are localized.
            new AutoScaleDeclaration("Count", "Returns the total number of samples in the metric history.", 0),
            new AutoScaleDeclaration("GetSample", "Returns a vector of data samples.", 0),
            new AutoScaleDeclaration("GetSamplePeriod", "Returns the period of the samples taken in a historical sample data set.", 0),
            new AutoScaleDeclaration("HistoryBeginTime", "Returns the timestamp of the oldest available data sample for the metric.", 0),
            new AutoScaleDeclaration("GetSamplePercent", "Returns the percent of samples a history currently has for a given time interval.", 0),
        };

        public override string Name => LanguageName;

        public AutoScaleLanguageService()
        {
        }

        public override string GetFormatFilterList()
        {
            return null;
        }

        #region LanguageService Methods

        public override LanguagePreferences GetLanguagePreferences()
        {
            if (_preferences == null)
            {
                _preferences = new LanguagePreferences(
                    Site,
                    typeof(AutoScaleLanguageService).GUID,
                    Name);

                _preferences.Init();
            }

            return _preferences;
        }

        public override IScanner GetScanner(IVsTextLines buffer)
        {
            if (_scanner == null)
            {
                _scanner = new Scanner(buffer);
            }

            return _scanner;
        }

        public override AuthoringScope ParseSource(ParseRequest req)
        {
            _scanner.SetSource(req.Text, 0);
            var authoringScope = new AutoScaleAuthoringScope();

            switch (req.Reason)
            {
                case ParseReason.DisplayMemberList:
                    OnDisplayMemberList(req);
                    break;

                case ParseReason.MemberSelect:
                    OnMemberSelect(req, authoringScope);
                    break;

                case ParseReason.HighlightBraces:
                    OnHighlightBraces(req);
                    break;
            }

            return authoringScope;
        }

        // The user placed the cursor on an identifier and selected Edit, Intellisense,
        // List Members. Do what, exactly?
        private void OnDisplayMemberList(ParseRequest req)
        {
        }

        // The user typed a member select operator. Provide the list of members of the
        // identifier preceding the member select operator.
        private void OnMemberSelect(ParseRequest req, AutoScaleAuthoringScope authoringScope)
        {
            foreach (var declaration in SystemVariables)
            {
                authoringScope.AddDeclaration(declaration);
            }
        }

        #endregion LanguageService Methods

        // The user typed a closing brace. Highlight the matching opening brace.
        private void OnHighlightBraces(ParseRequest req)
        {
            var tokens = TokenizeFile(req);
            var braceMatches = FindBraceMatches(tokens, req.Text);
            int? matchIndex = FindMatchForBrace(req, braceMatches);

            if (matchIndex.HasValue)
            {
                req.Sink.FoundMatchingBrace = true;

                int nextLine, nextCol;

                Source source = GetSource(req.View);
                source.GetLineIndexOfPosition(matchIndex.Value, out nextLine, out nextCol);

                req.Sink.MatchPair(
                    new TextSpan
                    {
                        iStartLine = req.Line,
                        iEndLine = req.Line,
                        // The caret is after the closing brace, so back up one column.
                        iStartIndex = req.Col - 1,
                        iEndIndex = req.Col
                    },

                    new TextSpan
                    {
                        iStartLine = nextLine,
                        iEndLine = nextLine,
                        iStartIndex = nextCol,
                        iEndIndex = nextCol + 1
                    }, 0);
            }
        }

        internal IEnumerable<TokenInfo> TokenizeFile(ParseRequest req)
        {
            var tokens = new List<TokenInfo>();
            int state = 0;

            for (var info = new TokenInfo();
                _scanner.ScanTokenAndProvideInfoAboutIt(info, ref state);
                info = new TokenInfo())
            {
                tokens.Add(info);
            }

            return tokens;
        }

        internal IEnumerable<BraceMatch> FindBraceMatches(IEnumerable<TokenInfo> tokens, string text)
        {
            var braceMatches = new List<BraceMatch>();
            var parenStack = new Stack<TokenInfo>();

            foreach (var token in tokens.Where(t => t.Type == TokenType.Delimiter))
            {
                if (text[token.StartIndex] == '(')
                {
                    parenStack.Push(token);
                }
                else if (text[token.EndIndex] == ')')
                {
                    if (parenStack.Count > 0)
                    {
                        TokenInfo closeParen = token;
                        TokenInfo openParen = parenStack.Pop();
                        var braceMatch = new BraceMatch(openParen.StartIndex, closeParen.StartIndex);
                        braceMatches.Add(braceMatch);
                    }
                }
            }

            return braceMatches;
        }

        private int? FindMatchForBrace(ParseRequest req, IEnumerable<BraceMatch> braceMatches)
        {
            Source source = GetSource(req.View);
            int indexOfCaret = source.GetPositionOfLineIndex(req.Line, req.Col);

            foreach (var braceMatch in braceMatches)
            {
                if (indexOfCaret == braceMatch.Left + 1)
                {
                    return braceMatch.Right;
                }
                else if (indexOfCaret == braceMatch.Right + 1)
                {
                    return braceMatch.Left;
                }
            }

            return null;
        }
    }
}
