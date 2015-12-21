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
        private AuthoringSink _sink;

        public override string Name => LanguageName;

        // This event is exposed to facilitate unit testing. When the language service detects a
        // matched pair, it informs the AuthoringSink object by calling AuthoringSink.MatchPair.
        // The AuthoringSink object adds the pair to an internal list, and there's no way to
        // query the list to determine if the appropriate pairs were added. Unit tests can hook
        // up to this event and keep track of the pairs as they are added.
        internal event EventHandler<MatchedPairFoundEventArgs> MatchedPairFound;

        public AutoScaleFormulaLanguageService()
        {
            // For uniformity, the language service itself subscribes to the event, and the event
            // handler calls AuthoringSink.MatchPair. We could have called AuthoringSink.MatchPair
            // directly, but it seems more elegant to have everything done by the event handlers.
            MatchedPairFound += OnMatchedPairFound;
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
            _sink = req.Sink;

            switch (req.Reason)
            {
                case ParseReason.MatchBraces:
                    ParseEntireFile(req);
                    break;
            }

            return new AutoScaleFormulaAuthoringScope();
        }

        #endregion LanguageService Methods

        private void ParseEntireFile(ParseRequest req)
        {
            string[] lines = req.Text.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None);

            var tokens = new List<Token>();
            int state = 0;

            int lineNumber = 1;
            foreach (var line in lines)
            {
                _scanner.SetSource(line, 0);

                for (var info = new TokenInfo();
                    _scanner.ScanTokenAndProvideInfoAboutIt(info, ref state);
                    info = new TokenInfo())
                {
                    tokens.Add(
                        new Token(
                            lineNumber,
                            info.StartIndex,
                            info.EndIndex,
                            line.Substring(info.StartIndex, info.EndIndex - info.StartIndex + 1),
                            info.Type));
                }

                ++lineNumber;
            }

            FindMatchedPairs(tokens);
        }

        private void FindMatchedPairs(List<Token> tokens)
        {
            var parenStack = new Stack<Token>();

            foreach (var token in tokens.Where(t => t.Type == TokenType.Delimiter))
            {
                if (token.Text == "(")
                {
                    parenStack.Push(token);
                }
                else if (token.Text == ")")
                {
                    if (parenStack.Count > 0)
                    {
                        Token openParen = parenStack.Pop();

                        var start = new TextSpan
                        {
                            iStartLine = openParen.LineNumber,
                            iStartIndex = openParen.StartIndex,
                            iEndLine = openParen.LineNumber,
                            iEndIndex = openParen.EndIndex
                        };

                        var end = new TextSpan
                        {
                            iStartLine = token.LineNumber,
                            iStartIndex = token.StartIndex,
                            iEndLine = token.LineNumber,
                            iEndIndex = token.EndIndex
                        };

                        int priority = parenStack.Count;

                        var handler = MatchedPairFound;
                        if (handler != null)
                        {
                            handler(this, new MatchedPairFoundEventArgs(start, end, priority));
                        }
                    }
                }
            }
        }

        private void OnMatchedPairFound(object sender, MatchedPairFoundEventArgs e)
        {
            _sink.MatchPair(e.Start, e.End, e.Priority);
        }
    }
}
