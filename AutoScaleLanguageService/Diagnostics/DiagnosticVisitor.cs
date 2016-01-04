using System.Collections.Generic;
using System.Linq;
using Lakewood.AutoScale.Syntax;

namespace Lakewood.AutoScale.Diagnostics
{
    internal class DiagnosticVisitor : ISyntaxNodeVisitor
    {
        private readonly List<IDiagnosticRule> _diagnosticRules;

        internal DiagnosticVisitor(IEnumerable<IDiagnosticRule> diagnosticRules)
        {
            _diagnosticRules = diagnosticRules.ToList();
        }

        #region ISyntaxNodeVisitor

        public void Visit(TernaryOperationNode ternaryOperation)
        {
            foreach (var rule in _diagnosticRules)
            {
                rule.Visit(ternaryOperation);
            }
        }

        public void Visit(UnaryOperationNode unaryOperation)
        {
            foreach (var rule in _diagnosticRules)
            {
                rule.Visit(unaryOperation);
            }
        }

        public void Visit(MethodInvocationNode methodInvocation)
        {
            foreach (var rule in _diagnosticRules)
            {
                rule.Visit(methodInvocation);
            }
        }

        public void Visit(StringLiteralNode stringLiteral)
        {
            foreach (var rule in _diagnosticRules)
            {
                rule.Visit(stringLiteral);
            }
        }

        public void Visit(ParenthesizedExpressionNode parenthesizedExpression)
        {
            foreach (var rule in _diagnosticRules)
            {
                rule.Visit(parenthesizedExpression);
            }
        }

        public void Visit(IdentifierNode identifier)
        {
            foreach (var rule in _diagnosticRules)
            {
                rule.Visit(identifier);
            }
        }

        public void Visit(DoubleLiteralNode doubleLiteral)
        {
            foreach (var rule in _diagnosticRules)
            {
                rule.Visit(doubleLiteral);
            }
        }

        public void Visit(KeywordNode keyword)
        {
            foreach (var rule in _diagnosticRules)
            {
                rule.Visit(keyword);
            }
        }

        public void Visit(FunctionCallNode functionCall)
        {
            foreach (var rule in _diagnosticRules)
            {
                rule.Visit(functionCall);
            }
        }

        public void Visit(BinaryOperationNode binaryOperation)
        {
            foreach (var rule in _diagnosticRules)
            {
                rule.Visit(binaryOperation);
            }
        }

        public void Visit(AssignmentNode assignment)
        {
            foreach (var rule in _diagnosticRules)
            {
                rule.Visit(assignment);
            }
        }

        public void Visit(FormulaNode formula)
        {
            foreach (var rule in _diagnosticRules)
            {
                rule.Visit(formula);
            }
        }

        #endregion ISyntaxNodeVisitor
    }
}