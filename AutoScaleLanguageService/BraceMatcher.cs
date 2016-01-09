// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;
using System.Collections.Generic;
using Lakewood.AutoScale.Diagnostics;
using Lakewood.AutoScale.Syntax;

namespace Lakewood.AutoScale
{
    internal class BraceMatcher
    {
        internal void FindMatches(FormulaNode formula)
        {
            var sink = new BraceMatchSink();

            formula.Accept(new BraceMatchVisitor(sink));

            Matches = sink.Matches;
        }

        internal int? FindMatchForBrace(int indexOfCaret)
        {
            foreach (var match in Matches)
            {
                if (indexOfCaret == match.Left + 1)
                {
                    return match.Right;
                }
                else if (indexOfCaret == match.Right + 1)
                {
                    return match.Left;
                }
            }

            return null;
        }

        internal IReadOnlyCollection<BraceMatch> Matches
        {
            get; private set;
        }

        #region Helper types

        private interface IBraceMatchSink
        {
            void AddBraceMatch(BraceMatch match);
        }

        private class BraceMatchSink : IBraceMatchSink
        {
            private readonly List<BraceMatch> _matches = new List<BraceMatch>();

            public void AddBraceMatch(BraceMatch match)
            {
                _matches.Add(match);
            }

            internal IReadOnlyCollection<BraceMatch> Matches => Array.AsReadOnly(_matches.ToArray());
        }

        private class BraceMatchVisitor : DiagnosticVisitor
        {
            internal BraceMatchVisitor(IBraceMatchSink sink)
                : base(new[] { new BraceMatchRule(sink) })
            {
            }
        }

        private class BraceMatchRule : DiagnosticRuleBase
        {
            private readonly IBraceMatchSink _sink;

            internal BraceMatchRule(IBraceMatchSink sink)
            {
                _sink = sink;
            }

            public override void Visit(ParenthesizedExpressionNode parenthesizedExpression)
            {
                _sink.AddBraceMatch(
                    new BraceMatch(
                        parenthesizedExpression.OpenParen.StartIndex,
                        parenthesizedExpression.CloseParen.StartIndex));
            }

            public override void Visit(FunctionCallNode functionCall)
            {
                _sink.AddBraceMatch(
                    new BraceMatch(
                        functionCall.OpenParen.StartIndex,
                        functionCall.CloseParen.StartIndex));
            }

            public override void Visit(MethodInvocationNode methodInvocation)
            {
                _sink.AddBraceMatch(
                    new BraceMatch(
                        methodInvocation.OpenParen.StartIndex,
                        methodInvocation.CloseParen.StartIndex));
            }

            #endregion Helper types
        }
    }
}
