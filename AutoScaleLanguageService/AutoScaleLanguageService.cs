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
            new AutoScaleDeclaration("$CPUPercent", Resources.CPUPercentVariableDescription),
            new AutoScaleDeclaration("$WallClockSeconds", Resources.WallClockSecondsVariableDescription),
            new AutoScaleDeclaration("$MemoryBytes", Resources.MemoryBytesVariableDescription),

            new AutoScaleDeclaration("$DiskBytes", Resources.DiskBytesVariableDescription),
            new AutoScaleDeclaration("$DiskReadBytes", Resources.DiskReadBytesVariableDescription),
            new AutoScaleDeclaration("$DiskWriteBytes", Resources.DiskWriteBytesVariableDescription),
            new AutoScaleDeclaration("$DiskReadOps", Resources.DiskReadOpsVariableDescription),
            new AutoScaleDeclaration("$DiskWriteOps", Resources.DiskWriteOpsVariableDescription),

            new AutoScaleDeclaration("$NetworkInBytes", Resources.NetworkInBytesVariableDescription),
            new AutoScaleDeclaration("$NetworkOutBytes", Resources.NetworkOutBytesVariableDescription),

            new AutoScaleDeclaration("$SampleNodeCount", Resources.SampleNodeCountVariableDescription),

            new AutoScaleDeclaration("$ActiveTasks", Resources.ActiveTasksVariableDescription),
            new AutoScaleDeclaration("$RunningTasks", Resources.RunningTasksVariableDescription),
            new AutoScaleDeclaration("$SucceededTasks", Resources.SucceededTasksVariableDescription),
            new AutoScaleDeclaration("$FailedTasks", Resources.FailedTasksVariableDescription),

            new AutoScaleDeclaration("$CurrentDedicated", Resources.CurrentDedicatedVariableDescription),
        };

        internal static readonly AutoScaleDeclaration[] SamplingSystemVariableMembers = new[]
        {
            // TODO: Descriptions are localized.
            new AutoScaleDeclaration("Count", Resources.CountMethodDescription),
            new AutoScaleDeclaration("GetSample", Resources.GetSampleMethodDescription),
            new AutoScaleDeclaration("GetSamplePeriod", Resources.GetSamplePeriodMethodDescription),
            new AutoScaleDeclaration("HistoryBeginTime", Resources.HistoryBeginTimeMethodDescription),
            new AutoScaleDeclaration("GetSamplePercent", Resources.GetSamplePercentMethodDescription),
        };

        internal static readonly AutoScaleDeclaration[] AssignableSystemVariables = new[]
        {
            new AutoScaleDeclaration("$TargetDedicated", "The target number of dedicated compute nodes for the pool. The value can be changed based upon actual usage for tasks."),
            new AutoScaleDeclaration("$NodeDeallocationOption", "The action that occurs when compute nodes are removed from a pool.")
        };

        internal static readonly AutoScaleDeclaration[] AllSystemVariables = 
            SamplingSystemVariables.Union(AssignableSystemVariables).OrderBy(decl => decl.Name).ToArray();

        internal static readonly AutoScaleDeclaration[] BuiltInFunctions = new[]
        {
            new AutoScaleDeclaration("avg", Resources.AverageFunctionDescription),
            new AutoScaleDeclaration("len", Resources.LengthFunctionDescription),
            new AutoScaleDeclaration("lg", Resources.Log2FunctionDescription),
            new AutoScaleDeclaration("ln", Resources.NaturalLogFunctionDescription),
            new AutoScaleDeclaration("log", Resources.Log10FunctionDescription),
            new AutoScaleDeclaration("max", Resources.MaximumFunctionDescription),
            new AutoScaleDeclaration("min", Resources.MinimumFunctionDescription),
            new AutoScaleDeclaration("norm", Resources.NormFunctionDescription),
            new AutoScaleDeclaration("percentile", Resources.PercentileFunctionDescription),
            new AutoScaleDeclaration("rand", Resources.RandomFunctionDescription),
            new AutoScaleDeclaration("range", Resources.RangeFunctionDescription),
            new AutoScaleDeclaration("std", Resources.StandardDeviationFunctionDescription),
            new AutoScaleDeclaration("stop", Resources.StopFunctionDescription),
            new AutoScaleDeclaration("sum", Resources.SumFunctionDescription),
            new AutoScaleDeclaration("time", Resources.TimeFunctionDescription),
            new AutoScaleDeclaration("val", Resources.ValueFunctionDescription),
        };

        internal static readonly AutoScaleDeclaration[] TimeIntervals = new[]
        {
            new AutoScaleDeclaration("TimeInterval_Zero", Resources.TimeIntervalZeroDescription),
            new AutoScaleDeclaration("TimeInterval_100ns", Resources.TimeInterval100NanosecondsDescription),
            new AutoScaleDeclaration("TimeInterval_Microsecond", Resources.TimeIntervalMicrosecondDescription),
            new AutoScaleDeclaration("TimeInterval_Millisecond", Resources.TimeIntervalMillisecondDescription),
            new AutoScaleDeclaration("TimeInterval_Second", Resources.TimeIntervalSecondDescription),
            new AutoScaleDeclaration("TimeInterval_Minute", Resources.TimeIntervalMinuteDescription),
            new AutoScaleDeclaration("TimeInterval_Hour", Resources.TimeIntervalHourDescription),
            new AutoScaleDeclaration("TimeInterval_Day", Resources.TimeIntervalDayDescription),
            new AutoScaleDeclaration("TimeInterval_Week", Resources.TimeIntervalWeekDescription),
            new AutoScaleDeclaration("TimeInterval_Year", Resources.TimeIntervalYearDescription),
        };

        internal static readonly AutoScaleDeclaration[] AllBuiltInIdentifiers = AllSystemVariables
            .Union(BuiltInFunctions)
            .Union(TimeIntervals)
            .ToArray();

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
        private void OnDisplayMemberList(ParseRequest req, AutoScaleAuthoringScope authoringScope)
        {
            var tokens = TokenizeFile(req);
            string identifier = FindPrecedingIdentifier(req, tokens);

            IEnumerable<AutoScaleDeclaration> declarationsToDisplay = null;

            if (IsSamplingSystemVariableMember(identifier))
            {
                declarationsToDisplay = SamplingSystemVariableMembers;
            }
            else
            {
                IEnumerable<AutoScaleDeclaration> identifierDeclarations =
                    FindIdentifiers(tokens, req.Text)
                        .Where(id => !AllBuiltInIdentifiers.Select(decl => decl.Name).Contains(id))
                        .Where(id => !SamplingSystemVariableMembers.Select(decl => decl.Name).Contains(id))
                        .Distinct()
                        .Select(id => new AutoScaleDeclaration(id, Resources.UserDefinedVariableDescription));

                declarationsToDisplay = AllBuiltInIdentifiers
                    .Union(identifierDeclarations)
                    .OrderBy(decl => decl.Name);

            }

            foreach (var declaration in declarationsToDisplay)
            {
                authoringScope.AddDeclaration(declaration);
            }
        }

        private IEnumerable<string> FindIdentifiers(IEnumerable<TokenInfo> tokens, string input)
        {
            return tokens
                .Where(t => t.Type == TokenType.Identifier)
                .Select(t => GetTokenText(t, input));
        }

        // The user typed a member select operator. Provide the list of members of the
        // identifier preceding the member select operator.
        private void OnMemberSelect(ParseRequest req, AutoScaleAuthoringScope authoringScope)
        {
            var tokens = TokenizeFile(req);
            string identifier = FindPrecedingIdentifier(req, tokens);

            if (IsSamplingSystemVariable(identifier))
            {
                foreach (var declaration in SamplingSystemVariableMembers)
                {
                    authoringScope.AddDeclaration(declaration);
                }
            }
        }

        private bool IsSamplingSystemVariable(string identifier)
        {
            return SamplingSystemVariables.Any(sv => sv.Name == identifier);
        }

        private bool IsSamplingSystemVariableMember(string identifier)
        {
            return SamplingSystemVariableMembers.Any(sv => sv.Name == identifier);
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
            int indexOfMemberSelectOperator = indexOfCaret - 1;

            TokenInfo precedingIdentifier = null;
            foreach (var token in tokens)
            {
                if (token.StartIndex >= indexOfMemberSelectOperator)
                {
                    break;
                }

                if (token.Type == TokenType.WhiteSpace || token.Type == TokenType.LineComment)
                {
                    continue;
                }

                precedingIdentifier = token.Type == TokenType.Identifier
                    ? token
                    : null;
            }

            return precedingIdentifier != null
                ? GetTokenText(precedingIdentifier, req.Text)
                : null;
        }

        private string GetTokenText(TokenInfo token, string input)
        {
            return input.Substring(
                token.StartIndex,
                token.EndIndex - token.StartIndex + 1);
        }
    }
}
