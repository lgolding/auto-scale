// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lakewood.AutoScale.Syntax;

namespace Lakewood.AutoScale.Diagnostics
{
    internal class DiagnosticVisitor : ISyntaxNodeVisitor
    {
        private readonly List<IDiagnosticRule> _assignmentRules;
        private readonly List<IDiagnosticRule> _binaryOperationRules;
        private readonly List<IDiagnosticRule> _doubleLiteralRules;
        private readonly List<IDiagnosticRule> _formulaRules;
        private readonly List<IDiagnosticRule> _functionCallRules;
        private readonly List<IDiagnosticRule> _identifierRules;
        private readonly List<IDiagnosticRule> _keywordRules;
        private readonly List<IDiagnosticRule> _methodInvocationRules;
        private readonly List<IDiagnosticRule> _parenthesizedExpressionRules;
        private readonly List<IDiagnosticRule> _stringLiteralRules;
        private readonly List<IDiagnosticRule> _ternaryOperationRules;
        private readonly List<IDiagnosticRule> _unaryOperationRules;

        internal DiagnosticVisitor(IEnumerable<IDiagnosticRule> diagnosticRules)
        {
            var ruleList = diagnosticRules.ToList();

            _assignmentRules              = GetRegisteredRules<AssignmentNode>(ruleList);
            _binaryOperationRules         = GetRegisteredRules<BinaryOperationNode>(ruleList);
            _doubleLiteralRules           = GetRegisteredRules<DoubleLiteralNode>(ruleList);
            _formulaRules                 = GetRegisteredRules<FormulaNode>(ruleList);
            _functionCallRules            = GetRegisteredRules<FunctionCallNode>(ruleList);
            _identifierRules              = GetRegisteredRules<IdentifierNode>(ruleList);
            _keywordRules                 = GetRegisteredRules<KeywordNode>(ruleList);
            _methodInvocationRules        = GetRegisteredRules<MethodInvocationNode>(ruleList);
            _parenthesizedExpressionRules = GetRegisteredRules<ParenthesizedExpressionNode>(ruleList);
            _stringLiteralRules           = GetRegisteredRules<StringLiteralNode>(ruleList);
            _ternaryOperationRules        = GetRegisteredRules<TernaryOperationNode>(ruleList);
            _unaryOperationRules          = GetRegisteredRules<UnaryOperationNode>(ruleList);
        }

        private List<IDiagnosticRule> GetRegisteredRules<T>(List<IDiagnosticRule> diagnosticRules) where T : SyntaxNode
        {
            return diagnosticRules.Where(OverridesVisit<T>).ToList();
        }

        private bool OverridesVisit<T>(IDiagnosticRule rule) where T : SyntaxNode
        {
            MethodInfo overriddenMethod =
                rule.GetType().GetMethod(
                    "Visit",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                    null,
                    CallingConventions.HasThis,
                    new[] { typeof(T) },
                    null);

            return overriddenMethod != null;
        }

        #region ISyntaxNodeVisitor

        public void Visit(TernaryOperationNode ternaryOperation)
        {
            foreach (var rule in _ternaryOperationRules)
            {
                rule.Visit(ternaryOperation);
            }
        }

        public void Visit(UnaryOperationNode unaryOperation)
        {
            foreach (var rule in _unaryOperationRules)
            {
                rule.Visit(unaryOperation);
            }
        }

        public void Visit(MethodInvocationNode methodInvocation)
        {
            foreach (var rule in _methodInvocationRules)
            {
                rule.Visit(methodInvocation);
            }
        }

        public void Visit(StringLiteralNode stringLiteral)
        {
            foreach (var rule in _stringLiteralRules)
            {
                rule.Visit(stringLiteral);
            }
        }

        public void Visit(ParenthesizedExpressionNode parenthesizedExpression)
        {
            foreach (var rule in _parenthesizedExpressionRules)
            {
                rule.Visit(parenthesizedExpression);
            }
        }

        public void Visit(IdentifierNode identifier)
        {
            foreach (var rule in _identifierRules)
            {
                rule.Visit(identifier);
            }
        }

        public void Visit(DoubleLiteralNode doubleLiteral)
        {
            foreach (var rule in _doubleLiteralRules)
            {
                rule.Visit(doubleLiteral);
            }
        }

        public void Visit(KeywordNode keyword)
        {
            foreach (var rule in _keywordRules)
            {
                rule.Visit(keyword);
            }
        }

        public void Visit(FunctionCallNode functionCall)
        {
            foreach (var rule in _functionCallRules)
            {
                rule.Visit(functionCall);
            }
        }

        public void Visit(BinaryOperationNode binaryOperation)
        {
            foreach (var rule in _binaryOperationRules)
            {
                rule.Visit(binaryOperation);
            }
        }

        public void Visit(AssignmentNode assignment)
        {
            foreach (var rule in _assignmentRules)
            {
                rule.Visit(assignment);
            }
        }

        public void Visit(FormulaNode formula)
        {
            foreach (var rule in _formulaRules)
            {
                rule.Visit(formula);
            }
        }

        #endregion ISyntaxNodeVisitor
    }
}