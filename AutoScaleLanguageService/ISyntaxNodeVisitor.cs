// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using Lakewood.AutoScale.Syntax;

namespace Lakewood.AutoScale
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
