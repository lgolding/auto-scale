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
        private Source _source;

        // NOTE: $TargetDedicated and $NodeDeallocationOption are also system variables,
        // but they do not (AFAIK) allow you to sample them, so  we don't include them in
        // the list of variables which will get the Intellisense member completion list if
        // you type a "." after them.
        internal static readonly string[] SystemVariableNames = new string[]
        {
            "$CPUPercent",
            "$WallClockSeconds",
            "$MemoryBytes",

            "$DiskBytes",
            "$DiskReadBytes",
            "$DiskWriteBytes",
            "$DiskReadOps",
            "$DiskWriteOps",

            "$NetworkInBytes",
            "$NetworkOutBytes",

            "$SampleNodeCount",

            "$ActiveTasks",
            "$RunningTasks",
            "$SucceededTasks",
            "$FailedTasks",

            "$CurrentDedicated"
        };

        internal static readonly AutoScaleDeclaration[] SystemVariableMembers = new[]
        {
            // TODO: Descriptions are localized.
            new AutoScaleDeclaration("Count", "Returns the total number of samples in the metric history.", 0),
            new AutoScaleDeclaration("GetSample", "Returns a vector of data samples.", 0),
            new AutoScaleDeclaration("GetSamplePeriod", "Returns the period of the samples taken in a historical sample data set.", 0),
            new AutoScaleDeclaration("HistoryBeginTime", "Returns the timestamp of the oldest available data sample for the metric.", 0),
            new AutoScaleDeclaration("GetSamplePercent", "Returns the percent of samples a history currently has for a given time interval.", 0),
        };

        #region LanguageService Members

        public override string Name => LanguageName;

        public override string GetFormatFilterList()
        {
            return null;
        }

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
            _source = GetSource(req.View);

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

        #endregion LanguageService Members

        #region Test Helpers

        // These helper methods facilitate writing unit tests. In unit tests, there is no
        // IVsTextView object available, so LanguageService.GetSource (called from ParseRequest)
        // returns null, so there is no Source object available either. And the source object
        // is needed for functionality such as brace matching and Intellisense member selection.
        //
        // Unit tests that require conversion between index and line/column positions can
        // derive a class from this class (AutoScaleLanguageService), and override these methods
        // to return the appropriate values.
        public virtual void GetLineIndexOfPosition(int index, out int line, out int col)
        {
            _source.GetLineIndexOfPosition(index, out line, out col);
        }

        public virtual int GetPositionOfLineIndex(int line, int col)
        {
            return _source.GetPositionOfLineIndex(line, col);
        }

        #endregion Test Helpers

        // The user placed the cursor on an identifier and selected Edit, Intellisense,
        // List Members. Do what, exactly?
        private void OnDisplayMemberList(ParseRequest req)
        {
        }

        // The user typed a member select operator. Provide the list of members of the
        // identifier preceding the member select operator.
        private void OnMemberSelect(ParseRequest req, AutoScaleAuthoringScope authoringScope)
        {
            var tokens = TokenizeFile(req);
            string identifier = FindPrecedingIdentifier(req, tokens);

            if (IsSystemVariable(identifier))
            {
                foreach (var declaration in SystemVariableMembers)
                {
                    authoringScope.AddDeclaration(declaration);
                }
            }
        }

        private bool IsSystemVariable(string identifier)
        {
            return SystemVariableNames.Contains(identifier);
        }

        // The user typed a closing brace. Highlight the matching opening brace.
        private void OnHighlightBraces(ParseRequest req)
        {
            var tokens = TokenizeFile(req);
            var braceMatches = FindBraceMatches(tokens, req.Text);

            int indexOfCaret = GetPositionOfLineIndex(req.Line, req.Col);
            int? matchIndex = FindMatchForBrace(indexOfCaret, braceMatches);

            if (matchIndex.HasValue)
            {
                int matchLine, matchCol;
                GetLineIndexOfPosition(matchIndex.Value, out matchLine, out matchCol);

                var spanAtCaret = new TextSpan
                {
                    iStartLine = req.Line,
                    iEndLine = req.Line,
                    // The caret is after the closing brace, so back up one column.
                    iStartIndex = req.Col - 1,
                    iEndIndex = req.Col
                };

                var spanAtMatch = new TextSpan
                {
                    iStartLine = matchLine,
                    iEndLine = matchLine,
                    iStartIndex = matchCol,
                    iEndIndex = matchCol + 1
                };

                req.Sink.FoundMatchingBrace = true;
                req.Sink.MatchPair(spanAtCaret, spanAtMatch, priority: 0);
            }
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
                        braceMatches.Add(braceMatch);
                    }
                }
            }

            return braceMatches;
        }

        private int? FindMatchForBrace(int indexOfCaret, IEnumerable<BraceMatch> braceMatches)
        {
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

        private string FindPrecedingIdentifier(ParseRequest req, IEnumerable<TokenInfo> tokens)
        {
            int indexOfCaret = GetPositionOfLineIndex(req.Line, req.Col);

            TokenInfo precedingIdentifier = null;
            foreach (var token in tokens)
            {
                if (token.StartIndex > indexOfCaret)
                {
                    break;
                }

                if (token.Type == TokenType.Identifier)
                {
                    precedingIdentifier = token;
                }
            }

            return precedingIdentifier != null
                ? req.Text.Substring(
                    precedingIdentifier.StartIndex,
                    precedingIdentifier.EndIndex - precedingIdentifier.StartIndex + 1)
                : null;
        }
    }
}
