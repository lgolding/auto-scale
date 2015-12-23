using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Lakewood.AutoScaleFormulaLanguageService
{
    public class AutoScaleFormulaLanguageService : LanguageService
    {
        public const string LanguageName = "AutoScaleFormula";

        private LanguagePreferences _preferences;
        private IScanner _scanner;

        public override string Name => LanguageName;

        public AutoScaleFormulaLanguageService()
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
                    typeof(AutoScaleFormulaLanguageService).GUID,
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
            var authoringScope = new AutoScaleFormulaAuthoringScope();

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
        private void OnMemberSelect(ParseRequest req, AuthoringScope authoringScope)
        {
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
