namespace Lakewood.AutoScale.Syntax
{
    public interface ISyntaxNodeVisitor
    {
        void Visit(FormulaNode formula);
        void Visit(AssignmentNode assignment);
        void Visit(TernaryOperationNode ternaryOperation);
        void Visit(BinaryOperationNode binaryOperation);
        void Visit(UnaryOperationNode unaryOperation);
        void Visit(FunctionCallNode functionCall);
        void Visit(MethodInvocationNode methodInvocation);
        void Visit(DoubleLiteralNode doubleLiteral);
        void Visit(KeywordNode keywordNode);
        void Visit(StringLiteralNode stringLiteral);
        void Visit(IdentifierNode identifier);
        void Visit(ParenthesizedExpressionNode parenthesizedExpression);
    }
}
