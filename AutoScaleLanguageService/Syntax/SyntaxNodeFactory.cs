namespace Lakewood.AutoScale.Syntax
{
    public static class SyntaxNodeFactory
    {
        public static DoubleLiteralNode MakeDoubleLiteral(string text, int startIndex)
        {
            return new DoubleLiteralNode(TokenFactory.MakeDoubleLiteral(text, startIndex));
        }

        public static IdentifierNode MakeIdentifier(string text, int startIndex)
        {
            return new IdentifierNode(TokenFactory.MakeIdentifier(text, startIndex));
        }

        public static StringLiteralNode MakeStringLiteral(string text, int startIndex)
        {
            return new StringLiteralNode(TokenFactory.MakeStringLiteral(text, startIndex));
        }
    }
}
