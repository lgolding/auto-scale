// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
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
