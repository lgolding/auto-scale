// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System.Collections.Generic;
using System.Linq;
using Lakewood.AutoScale.Diagnostics;
using Lakewood.AutoScale.Syntax;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Lakewood.AutoScale
{
    public class AutoScaleLanguageService : LanguageService
    {
        public const string LanguageName = "AutoScaleFormula";

        private LanguagePreferences _preferences;
        private IScanner _scanner;
        private readonly Parser _parser = new Parser();
        private FormulaNode _formulaNode;
        private Source _source;

        // NOTE: $TargetDedicated and $NodeDeallocationOption are also system variables,
        // but they do not (AFAIK) allow you to sample them, so  we don't include them in
        // the list of variables which will get the Intellisense member completion list if
        // you type a "." after them.
        internal static readonly AutoScaleDeclaration[] SamplingVariables = new AutoScaleDeclaration[]
        {
            new AutoScaleDeclaration(SamplingVariableName.CPUPercent, Resources.CPUPercentVariableDescription, IconImageIndex.Class),
            new AutoScaleDeclaration(SamplingVariableName.WallClockSeconds, Resources.WallClockSecondsVariableDescription, IconImageIndex.Class),
            new AutoScaleDeclaration(SamplingVariableName.MemoryBytes, Resources.MemoryBytesVariableDescription, IconImageIndex.Class),

            new AutoScaleDeclaration(SamplingVariableName.DiskBytes, Resources.DiskBytesVariableDescription, IconImageIndex.Class),
            new AutoScaleDeclaration(SamplingVariableName.DiskReadBytes, Resources.DiskReadBytesVariableDescription, IconImageIndex.Class),
            new AutoScaleDeclaration(SamplingVariableName.DiskWriteBytes, Resources.DiskWriteBytesVariableDescription, IconImageIndex.Class),
            new AutoScaleDeclaration(SamplingVariableName.DiskReadOps, Resources.DiskReadOpsVariableDescription, IconImageIndex.Class),
            new AutoScaleDeclaration(SamplingVariableName.DiskWriteOps, Resources.DiskWriteOpsVariableDescription, IconImageIndex.Class),

            new AutoScaleDeclaration(SamplingVariableName.NetworkInBytes, Resources.NetworkInBytesVariableDescription, IconImageIndex.Class),
            new AutoScaleDeclaration(SamplingVariableName.NetworkOutBytes, Resources.NetworkOutBytesVariableDescription, IconImageIndex.Class),

            new AutoScaleDeclaration(SamplingVariableName.SampleNodeCount, Resources.SampleNodeCountVariableDescription, IconImageIndex.Class),

            new AutoScaleDeclaration(SamplingVariableName.ActiveTasks, Resources.ActiveTasksVariableDescription, IconImageIndex.Class),
            new AutoScaleDeclaration(SamplingVariableName.RunningTasks, Resources.RunningTasksVariableDescription, IconImageIndex.Class),
            new AutoScaleDeclaration(SamplingVariableName.SucceededTasks, Resources.SucceededTasksVariableDescription, IconImageIndex.Class),
            new AutoScaleDeclaration(SamplingVariableName.FailedTasks, Resources.FailedTasksVariableDescription, IconImageIndex.Class),

            new AutoScaleDeclaration(SamplingVariableName.CurrentDedicated, Resources.CurrentDedicatedVariableDescription, IconImageIndex.Class),
        };

        internal static readonly AutoScaleDeclaration[] SamplingVariableMethods = new[]
        {
            new AutoScaleDeclaration(SamplingVariableMethodName.Count, Resources.CountMethodDescription, IconImageIndex.Method),
            new AutoScaleDeclaration(SamplingVariableMethodName.GetSample, Resources.GetSampleMethodDescription, IconImageIndex.Method),
            new AutoScaleDeclaration(SamplingVariableMethodName.GetSamplePercent, Resources.GetSamplePercentMethodDescription, IconImageIndex.Method),
            new AutoScaleDeclaration(SamplingVariableMethodName.GetSamplePeriod, Resources.GetSamplePeriodMethodDescription, IconImageIndex.Method),
            new AutoScaleDeclaration(SamplingVariableMethodName.HistoryBeginTime, Resources.HistoryBeginTimeMethodDescription, IconImageIndex.Method),
        };

        internal static readonly AutoScaleDeclaration[] AssignableSystemVariables = new[]
        {
            new AutoScaleDeclaration("$TargetDedicated", "The target number of dedicated compute nodes for the pool. The value can be changed based upon actual usage for tasks.", IconImageIndex.Variable),
            new AutoScaleDeclaration(VariableName.NodeDeallocationOption, "The action that occurs when compute nodes are removed from a pool.", IconImageIndex.Variable)
        };

        internal static readonly AutoScaleDeclaration[] AllSystemVariables = 
            SamplingVariables.Union(AssignableSystemVariables).OrderBy(decl => decl.Name).ToArray();

        internal static readonly AutoScaleDeclaration[] BuiltInFunctions = new[]
        {
            new AutoScaleDeclaration(BuiltInFunctionName.Average, Resources.AverageFunctionDescription, IconImageIndex.Intrinsic),
            new AutoScaleDeclaration(BuiltInFunctionName.Length, Resources.LengthFunctionDescription, IconImageIndex.Intrinsic),
            new AutoScaleDeclaration(BuiltInFunctionName.Log2, Resources.Log2FunctionDescription, IconImageIndex.Intrinsic),
            new AutoScaleDeclaration(BuiltInFunctionName.NaturalLog, Resources.NaturalLogFunctionDescription, IconImageIndex.Intrinsic),
            new AutoScaleDeclaration(BuiltInFunctionName.Log10, Resources.Log10FunctionDescription, IconImageIndex.Intrinsic),
            new AutoScaleDeclaration(BuiltInFunctionName.Maximum, Resources.MaximumFunctionDescription, IconImageIndex.Intrinsic),
            new AutoScaleDeclaration(BuiltInFunctionName.Minimum, Resources.MinimumFunctionDescription, IconImageIndex.Intrinsic),
            new AutoScaleDeclaration(BuiltInFunctionName.EuclideanNorm, Resources.EuclideanNormFunctionDescription, IconImageIndex.Intrinsic),
            new AutoScaleDeclaration(BuiltInFunctionName.Percentile, Resources.PercentileFunctionDescription, IconImageIndex.Intrinsic),
            new AutoScaleDeclaration(BuiltInFunctionName.Random, Resources.RandomFunctionDescription, IconImageIndex.Intrinsic),
            new AutoScaleDeclaration(BuiltInFunctionName.Range, Resources.RangeFunctionDescription, IconImageIndex.Intrinsic),
            new AutoScaleDeclaration(BuiltInFunctionName.StandardDeviation, Resources.StandardDeviationFunctionDescription, IconImageIndex.Intrinsic),
            new AutoScaleDeclaration(BuiltInFunctionName.Stop, Resources.StopFunctionDescription, IconImageIndex.Intrinsic),
            new AutoScaleDeclaration(BuiltInFunctionName.Sum, Resources.SumFunctionDescription, IconImageIndex.Intrinsic),
            new AutoScaleDeclaration(BuiltInFunctionName.Time, Resources.TimeFunctionDescription, IconImageIndex.Intrinsic),
            new AutoScaleDeclaration(BuiltInFunctionName.Value, Resources.ValueFunctionDescription, IconImageIndex.Intrinsic),
        };

        internal static readonly AutoScaleDeclaration[] TimeIntervals = new[]
        {
            new AutoScaleDeclaration("TimeInterval_Zero", Resources.TimeIntervalZeroDescription, IconImageIndex.Constant),
            new AutoScaleDeclaration("TimeInterval_100ns", Resources.TimeInterval100NanosecondsDescription, IconImageIndex.Constant),
            new AutoScaleDeclaration("TimeInterval_Microsecond", Resources.TimeIntervalMicrosecondDescription, IconImageIndex.Constant),
            new AutoScaleDeclaration("TimeInterval_Millisecond", Resources.TimeIntervalMillisecondDescription, IconImageIndex.Constant),
            new AutoScaleDeclaration("TimeInterval_Second", Resources.TimeIntervalSecondDescription, IconImageIndex.Constant),
            new AutoScaleDeclaration("TimeInterval_Minute", Resources.TimeIntervalMinuteDescription, IconImageIndex.Constant),
            new AutoScaleDeclaration("TimeInterval_Hour", Resources.TimeIntervalHourDescription, IconImageIndex.Constant),
            new AutoScaleDeclaration("TimeInterval_Day", Resources.TimeIntervalDayDescription, IconImageIndex.Constant),
            new AutoScaleDeclaration("TimeInterval_Week", Resources.TimeIntervalWeekDescription, IconImageIndex.Constant),
            new AutoScaleDeclaration("TimeInterval_Year", Resources.TimeIntervalYearDescription, IconImageIndex.Constant),
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
                    Name)
                {
                    ParameterInformation = true
                };

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
                case ParseReason.Check:
                    OnCheck(req, authoringScope);
                    break;

                case ParseReason.DisplayMemberList:
                    OnDisplayMemberList(req, authoringScope);
                    break;

                case ParseReason.HighlightBraces:
                    OnHighlightBraces(req);
                    break;

                case ParseReason.MemberSelect:
                    OnMemberSelect(req, authoringScope);
                    break;

                case ParseReason.MethodTip:
                    break;
            }

            return authoringScope;
        }

        public override void OnIdle(bool periodic)
        {
            // This appears to be necessary to get a parse request with ParseReason.Check.
            // See:
            // https://code.google.com/p/rubydotnetcompiler/source/browse/src/VisualStudioPackage/Language/Invariant/LanguageService.cs?r=4318149251e27807a3dbb7df910fb5ea476f5044
            // http://stackoverflow.com/questions/9727460/language-service-parsereason-check-never-called-after-migrating-to-vs2010
            Source src = GetSource(LastActiveTextView);
            if (src != null && src.LastParseTime >= int.MaxValue >> 12)
            {
                src.LastParseTime = 0;
            }

            base.OnIdle(periodic);
        }

        #endregion LanguageService Members

        #region Parse Handlers

        private void OnCheck(ParseRequest req, AutoScaleAuthoringScope authoringScope)
        {
            _formulaNode = _parser.Parse(req.Text);

            var analyzer = new Analyzer();
            analyzer.Analyze(_formulaNode);

            var allDiagnostics = _parser.Diagnostics.Union(analyzer.Diagnostics);

            ReportDiagnostics(allDiagnostics, req.FileName, req.Sink);
        }

        private void ReportDiagnostics(IEnumerable<Diagnostic> diagnostics, string fileName, AuthoringSink sink)
        {
            foreach (var diagnostic in diagnostics)
            {
                int startLine, startCol;
                _source.GetLineIndexOfPosition(diagnostic.StartIndex, out startLine, out startCol);

                int endLine, endCol;
                _source.GetLineIndexOfPosition(diagnostic.EndIndex, out endLine, out endCol);

                var context = new TextSpan
                {
                    iStartLine = startLine,
                    iStartIndex = startCol,
                    iEndLine = endLine,
                    iEndIndex = endCol + 1
                };

                sink.AddError(fileName, diagnostic.Message, context, diagnostic.Descriptor.Severity);
            }
        }

        // The user placed the cursor on an identifier and selected Edit, Intellisense,
        // List Members. Display the list of identifiers that are valid in this context.
        private void OnDisplayMemberList(ParseRequest req, AutoScaleAuthoringScope authoringScope)
        {
            var tokens = TokenizeFile(req);
            string identifier = FindPrecedingIdentifier(req, tokens);

            IEnumerable<AutoScaleDeclaration> declarationsToDisplay = null;

            if (IsSamplingSystemVariableMember(identifier))
            {
                declarationsToDisplay = SamplingVariableMethods;
            }
            else
            {
                IEnumerable<AutoScaleDeclaration> identifierDeclarations =
                    FindIdentifiers(tokens, req.Text)
                        .Where(id => !AllBuiltInIdentifiers.Select(decl => decl.Name).Contains(id))
                        .Where(id => !SamplingVariableMethods.Select(decl => decl.Name).Contains(id))
                        .Distinct()
                        .Select(id => new AutoScaleDeclaration(id, Resources.UserDefinedVariableDescription, (int)IconImageIndex.Variable + (int)IconImageIndex.AccessPrivate));

                declarationsToDisplay = AllBuiltInIdentifiers
                    .Union(identifierDeclarations)
                    .OrderBy(decl => decl.Name);

            }

            foreach (var declaration in declarationsToDisplay)
            {
                authoringScope.AddDeclaration(declaration);
            }
        }

        // The user typed a closing brace. Highlight the matching opening brace.
        private void OnHighlightBraces(ParseRequest req)
        {
            if (_formulaNode == null)
            {
                _formulaNode = _parser.Parse(req.Text);
            }

            var braceMatcher = new BraceMatcher();
            braceMatcher.FindMatches(_formulaNode);

            int indexOfCaret = GetPositionOfLineIndex(req.Line, req.Col);
            int? matchIndex = braceMatcher.FindMatchForBrace(indexOfCaret);

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

        // The user typed a member select operator. Provide the list of members of the
        // identifier preceding the member select operator.
        private void OnMemberSelect(ParseRequest req, AutoScaleAuthoringScope authoringScope)
        {
            var tokens = TokenizeFile(req);
            string identifier = FindPrecedingIdentifier(req, tokens);

            if (IsSamplingSystemVariable(identifier))
            {
                foreach (var declaration in SamplingVariableMethods)
                {
                    authoringScope.AddDeclaration(declaration);
                }
            }
        }

        #endregion Parse Handlers

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

        private IEnumerable<string> FindIdentifiers(IEnumerable<TokenInfo> tokens, string input)
        {
            return tokens
                .Where(t => t.Type == TokenType.Identifier)
                .Select(t => GetTokenText(t, input));
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

        private bool IsSamplingSystemVariable(string identifier)
        {
            return SamplingVariables.Any(sv => sv.Name == identifier);
        }

        private bool IsSamplingSystemVariableMember(string identifier)
        {
            return SamplingVariableMethods.Any(sv => sv.Name == identifier);
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
    }
}
