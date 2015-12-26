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
        internal static readonly AutoScaleDeclaration[] SamplingSystemVariables = new AutoScaleDeclaration[]
        {
            // TODO: Descriptions are localized.
            new AutoScaleDeclaration("$CPUPercent", "The average percentage of CPU usage."),
            new AutoScaleDeclaration("$WallClockSeconds", "The number of seconds consumed."),
            new AutoScaleDeclaration("$MemoryBytes", "The average number of megabytes used."),

            new AutoScaleDeclaration("$DiskBytes", "The average number of gigabytes used on the local disks."),
            new AutoScaleDeclaration("$DiskReadBytes", "The number of bytes read."),
            new AutoScaleDeclaration("$DiskWriteBytes", "The number of bytes written."),
            new AutoScaleDeclaration("$DiskReadOps", "	The count of read disk operations performed."),
            new AutoScaleDeclaration("$DiskWriteOps", "The count of write disk operations performed."),

            new AutoScaleDeclaration("$NetworkInBytes", "The number of inbound bytes."),
            new AutoScaleDeclaration("$NetworkOutBytes", "The number of outbound bytes."),

            new AutoScaleDeclaration("$SampleNodeCount", "The count of compute nodes."),

            new AutoScaleDeclaration("$ActiveTasks", "The number of tasks that are in an active state."),
            new AutoScaleDeclaration("$RunningTasks", "The number of tasks in a running state."),
            new AutoScaleDeclaration("$SucceededTasks", "The number of tasks that finished successfully."),
            new AutoScaleDeclaration("$FailedTasks", "The number of tasks that failed."),

            new AutoScaleDeclaration("$CurrentDedicated", "The current number of dedicated compute nodes."),
        };

        internal static readonly AutoScaleDeclaration[] SystemVariableMembers = new[]
        {
            // TODO: Descriptions are localized.
            new AutoScaleDeclaration("Count", "Returns the total number of samples in the metric history."),
            new AutoScaleDeclaration("GetSample", "Returns a vector of data samples."),
            new AutoScaleDeclaration("GetSamplePeriod", "Returns the period of the samples taken in a historical sample data set."),
            new AutoScaleDeclaration("HistoryBeginTime", "Returns the timestamp of the oldest available data sample for the metric."),
            new AutoScaleDeclaration("GetSamplePercent", "Returns the percent of samples a history currently has for a given time interval."),
        };

        internal static readonly AutoScaleDeclaration[] AssignableSystemVariables = new AutoScaleDeclaration[]
        {
            new AutoScaleDeclaration("$TargetDedicated", "The target number of dedicated compute nodes for the pool. The value can be changed based upon actual usage for tasks."),
            new AutoScaleDeclaration("$NodeDeallocationOption", "The action that occurs when compute nodes are removed from a pool.")
        };

        internal static readonly AutoScaleDeclaration[] AllSystemVariables = 
            SamplingSystemVariables.Union(AssignableSystemVariables).OrderBy(sv => sv.Name).ToArray();

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
                    OnDisplayMemberList(req, authoringScope);
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
        // List Members. Display the list of identifiers that are valid in this context.
        // DONE: Display all system variables.
        // TODO: Add built-in functions to that list.
        // TODO: Add user variables to that list.
        // TODO: When the context is a member function of one of the system variables,
        // display the member functions.
        private void OnDisplayMemberList(ParseRequest req, AutoScaleAuthoringScope authoringScope)
        {
            foreach (var declaration in AllSystemVariables)
            {
                authoringScope.AddDeclaration(declaration);
            }
        }

        // The user typed a member select operator. Provide the list of members of the
        // identifier preceding the member select operator.
        private void OnMemberSelect(ParseRequest req, AutoScaleAuthoringScope authoringScope)
        {
            var tokens = TokenizeFile(req);
            string identifier = FindPrecedingIdentifier(req, tokens);

            if (IsSamplingSystemVariable(identifier))
            {
                foreach (var declaration in SystemVariableMembers)
                {
                    authoringScope.AddDeclaration(declaration);
                }
            }
        }

        private bool IsSamplingSystemVariable(string identifier)
        {
            return SamplingSystemVariables.Any(sv => sv.Name == identifier);
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
