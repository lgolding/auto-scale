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

            SkipWhite();
            while (_lexer.More())
            {
                assignments.Add(Assignment());
                SkipWhite();

                if (_lexer.More())
                {
                    AutoScaleToken nextToken = _lexer.Peek();
                    if (nextToken.Type == AutoScaleTokenType.Semicolon)
                    {
                        _lexer.Skip();
                        SkipWhite();
                    }
                    else
                    {
                        throw new ParseException(AutoScaleTokenType.Semicolon, nextToken);
                    }
                }
            }

            return new FormulaNode(assignments.ToArray());
        }

        internal AssignmentNode Assignment()
        {
            IdentifierNode identifier = Identifier();

            SkipWhite();
            AutoScaleToken nextToken = _lexer.Peek();

            if (nextToken.Type == AutoScaleTokenType.OperatorAssign)
            {
                _lexer.Skip();
            }
            else
            {
                throw new ParseException(AutoScaleTokenType.OperatorAssign, nextToken);
            }

            SkipWhite();
            PrimaryExpressionNode expression = Expression();

            return new AssignmentNode(identifier, expression);
        }

        private PrimaryExpressionNode Expression()
        {
            PrimaryExpressionNode expression = null;
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

        private void SkipWhite()
        {
            while (_lexer.Peek().Type == AutoScaleTokenType.WhiteSpace)
            {
                _lexer.Skip();
            }
        }
    }
}
