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

        // This event is exposed to facilitate unit testing. When the language service detects a
        // matched pair, it informs the AuthoringSink object by calling AuthoringSink.MatchPair.
        // The AuthoringSink object adds the pair to an internal list, and there's no way to
        // query the list to determine if the appropriate pairs were added. Unit tests can hook
        // up to this event and keep track of the pairs as they are added.
        internal event EventHandler<BraceMatch> BraceMatchFound;

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
                _scanner = new AutoScaleFormulaScanner(buffer);
            }

            return _scanner;
        }

        public override AuthoringScope ParseSource(ParseRequest req)
        {
            _scanner.SetSource(req.Text, 0);

            switch (req.Reason)
            {
                case ParseReason.HighlightBraces:
                    MatchBraces(req);
                    break;
            }

            return new AutoScaleFormulaAuthoringScope();
        }

        #endregion LanguageService Methods

        private void MatchBraces(ParseRequest req)
        {
            var tokens = TokenizeFile(req);
            var braceMatches = FindBraceMatches(tokens, req.Text);
            var braceMatch = FindMatchForBrace(req.Line, req.Col);
        }

        private IEnumerable<TokenInfo> TokenizeFile(ParseRequest req)
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

        private IEnumerable<BraceMatch> FindBraceMatches(IEnumerable<TokenInfo> tokens, string text)
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

                        var handler = BraceMatchFound;
                        if (handler != null)
                        {
                            handler(this, braceMatch);
                        }

                        braceMatches.Add(braceMatch);
                    }
                }
            }

            return braceMatches;
        }

        private object FindMatchForBrace(int line, int col)
        {
            return null;
        }
    }
}
