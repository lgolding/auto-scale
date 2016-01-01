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
                    AutoScaleToken nextToken = _lexer.Peek();
                    if (nextToken.Type == AutoScaleTokenType.Semicolon)
                    {
                        _lexer.Skip();
                    }
                    else
                    {
                        throw new ParseException(AutoScaleTokenType.Semicolon, nextToken);
                    }

                    _lexer.SkipWhite();
                }
            }

            return new FormulaNode(assignments.ToArray());
        }

        internal AssignmentNode Assignment()
        {
            IdentifierNode identifier = Identifier();

            _lexer.SkipWhite();
            _lexer.Consume(AutoScaleTokenType.OperatorAssign);

            _lexer.SkipWhite();
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

                _lexer.SkipWhite();
                AutoScaleToken token = _lexer.GetNextToken();
                if (token.Type == AutoScaleTokenType.OperatorTernaryColon)
                {
                    var falseValue = PrimaryExpression();

                    return new TernaryOperatorNode(condition, trueValue, falseValue);
                }
                else
                {
                    throw new ParseException(AutoScaleTokenType.OperatorTernaryColon, token);
                }
            }
            else
            {
                return condition;
            }
        }

        private SyntaxNode PrimaryExpression()
        {
            SyntaxNode expression = null;
            switch (_lexer.Peek().Type)
            {
                case AutoScaleTokenType.DoubleLiteral:
                    expression = DoubleLiteral();
                    break;

                case AutoScaleTokenType.StringLiteral:
                    expression = StringLiteral();
                    break;

                case AutoScaleTokenType.Identifier:
                    expression = Identifier();
                    break;

                default:
                    throw new ParseException();
            }

            return expression;
        }

        internal IdentifierNode Identifier()
        {
            AutoScaleToken token = _lexer.GetNextToken();
            if (token.Type == AutoScaleTokenType.Identifier)
            {
                return new IdentifierNode(token.Text);
            }
            else
            {
                throw new ParseException(AutoScaleTokenType.Identifier, token);
            }
        }

        internal DoubleLiteralNode DoubleLiteral()
        {
            AutoScaleToken token = _lexer.GetNextToken();
            if (token.Type == AutoScaleTokenType.DoubleLiteral)
            {
                return new DoubleLiteralNode(double.Parse(token.Text));
            }
            else
            {
                throw new ParseException(AutoScaleTokenType.DoubleLiteral, token);
            }
        }

        internal StringLiteralNode StringLiteral()
        {
            AutoScaleToken token = _lexer.GetNextToken();
            if (token.Type == AutoScaleTokenType.StringLiteral)
            {
                return new StringLiteralNode(token.Text);
            }
            else
            {
                throw new ParseException(AutoScaleTokenType.StringLiteral, token);
            }
        }
    }
}
