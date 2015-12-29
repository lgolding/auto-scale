using Lakewood.AutoScale.Syntax;

namespace Lakewood.AutoScale
{
    internal class Parser
    {
        private readonly Lexer _lexer;

        internal Parser(string input)
        {
            _lexer = new Lexer(input);
        }

        internal SyntaxNode Parse()
        {
            SyntaxNode node = null;

            switch (_lexer.Peek().Type)
            {
                case AutoScaleTokenType.DoubleLiteral:
                    node = DoubleLiteral();
                    break;

                case AutoScaleTokenType.StringLiteral:
                    node = StringLiteral();
                    break;

                case AutoScaleTokenType.Identifier:
                    node = Identifier();
                    break;
            }

            return node;
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
