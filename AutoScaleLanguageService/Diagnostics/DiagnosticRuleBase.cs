using System;
using System.Collections.Generic;
using Lakewood.AutoScale.Syntax;

namespace Lakewood.AutoScale.Diagnostics
{
    public abstract class DiagnosticRuleBase : IDiagnosticRule
    {
        private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

        protected void AddDiagnostic(Diagnostic diagnostic)
        {
            _diagnostics.Add(diagnostic);
        }

        #region IDiagnosticRule

        public IReadOnlyCollection<Diagnostic> Diagnostics => Array.AsReadOnly(_diagnostics.ToArray());

        public virtual void Visit(TernaryOperationNode ternaryOperation)
        {
        }

        public virtual void Visit(UnaryOperationNode unaryOperation)
        {
        }

        public virtual void Visit(MethodInvocationNode methodInvocation)
        {
        }

        public virtual void Visit(StringLiteralNode stringLiteral)
        {
        }

        public virtual void Visit(ParenthesizedExpressionNode parenthesizedExpression)
        {
        }

        public virtual void Visit(IdentifierNode identifier)
        {
        }

        public virtual void Visit(DoubleLiteralNode doubleLiteral)
        {
        }

        public void Visit(KeywordNode keywordNode)
        {
        }

        public virtual void Visit(FunctionCallNode functionCall)
        {
        }

        public virtual void Visit(BinaryOperationNode binaryOperation)
        {
        }

        public virtual void Visit(AssignmentNode assignment)
        {
        }

        public virtual void Visit(FormulaNode formula)
        {
        }

        #endregion IDiagnosticRule
    }
}
