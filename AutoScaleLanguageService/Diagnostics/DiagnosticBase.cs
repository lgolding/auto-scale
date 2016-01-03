using System;
using System.Collections.Generic;
using Lakewood.AutoScale.Syntax;

namespace Lakewood.AutoScale.Diagnostics
{
    public abstract class DiagnosticBase : ISyntaxNodeVisitor
   {
        private readonly List<Diagnostic> _errors = new List<Diagnostic>();

        public IReadOnlyCollection<Diagnostic> Errors => Array.AsReadOnly(_errors.ToArray());

        protected void AddError(Diagnostic error)
        {
            _errors.Add(error);
        }

        #region ISyntaxNodeVisitor

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

        #endregion ISyntaxNodeVisitor
    }
}
