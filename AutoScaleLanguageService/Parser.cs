using System.Collections.Generic;
using Lakewood.AutoScale.Syntax;

namespace Lakewood.AutoScale
{
    internal class Parser
    {
        private readonly Lexer _lexer;
        private List<string> _errors = new List<string>();

        internal Parser(string input)
        {
            _lexer = new Lexer(input);
        }

        internal FormulaNode Parse()
        {
            var assignments = new List<AssignmentNode>();

            _lexer.SkipWhite();
            while (_lexer.More())
            {
                assignments.Add(Assignment());

                _lexer.SkipWhite();
                if (_lexer.More())
                {
                    _lexer.Consume(AutoScaleTokenType.Semicolon);
                    _lexer.SkipWhite();
                }
            }

            return new FormulaNode(assignments.ToArray());
        }

        internal AssignmentNode Assignment()
        {
            IdentifierNode identifier = Identifier();

            _lexer.Consume(AutoScaleTokenType.OperatorAssign);

            SyntaxNode expression = Expression();

            return new AssignmentNode(identifier, expression);
        }

        private SyntaxNode Expression()
        {
            var condition = PrimaryExpression();

            _lexer.SkipWhite();
            if (_lexer.Peek().Type == AutoScaleTokenType.OperatorTernaryQuestion)
            {
                _lexer.Skip();

                var trueValue = PrimaryExpression();

                _lexer.Consume(AutoScaleTokenType.OperatorTernaryColon);

                var falseValue = PrimaryExpression();

                return new TernaryOperatorNode(condition, trueValue, falseValue);
            }
            else
            {
                return condition;
            }
        }

        private SyntaxNode PrimaryExpression()
        {
            _lexer.SkipWhite();

            switch (_lexer.Peek().Type)
            {
                case AutoScaleTokenType.DoubleLiteral:
                    return DoubleLiteral();

                case AutoScaleTokenType.StringLiteral:
                    return StringLiteral();

                case AutoScaleTokenType.Identifier:
                    return Identifier();

                default:
                    throw new ParseException();
            }
        }

        internal IdentifierNode Identifier()
        {
            AutoScaleToken token = _lexer.Consume(AutoScaleTokenType.Identifier);
            return new IdentifierNode(token.Text);
        }

        internal DoubleLiteralNode DoubleLiteral()
        {
            AutoScaleToken token = _lexer.Consume(AutoScaleTokenType.DoubleLiteral);
            return new DoubleLiteralNode(double.Parse(token.Text));
        }

        internal StringLiteralNode StringLiteral()
        {
            AutoScaleToken token = _lexer.Consume(AutoScaleTokenType.StringLiteral);
            return new StringLiteralNode(token.Text);
        }
    }
}
