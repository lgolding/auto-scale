// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
namespace Lakewood.AutoScale
{
    public static class TokenFactory
    {
        public static AutoScaleToken MakeDoubleLiteral(string text, int startIndex)
        {
            return new AutoScaleToken(AutoScaleTokenType.DoubleLiteral, startIndex, startIndex + text.Length - 1, text);
        }

        public static AutoScaleToken MakeIdentifier(string text, int startIndex)
        {
            return new AutoScaleToken(AutoScaleTokenType.Identifier, startIndex, startIndex + text.Length - 1, text);
        }

        public static AutoScaleToken MakeStringLiteral(string text, int startIndex)
        {
            return new AutoScaleToken(AutoScaleTokenType.StringLiteral, startIndex, startIndex + text.Length - 1, text);
        }

        internal static AutoScaleToken MakeOpenParen(int startIndex)
        {
            return new AutoScaleToken(AutoScaleTokenType.OperatorSubtraction, startIndex, startIndex, "(");
        }

        internal static AutoScaleToken MakeCloseParen(int startIndex)
        {
            return new AutoScaleToken(AutoScaleTokenType.OperatorSubtraction, startIndex, startIndex, ")");
        }

        internal static AutoScaleToken MakeOperatorSubtraction(int startIndex)
        {
            return new AutoScaleToken(AutoScaleTokenType.OperatorSubtraction, startIndex, startIndex, "-");
        }

        internal static AutoScaleToken MakeOperatorNot(int startIndex)
        {
            return new AutoScaleToken(AutoScaleTokenType.OperatorSubtraction, startIndex, startIndex, "!");
        }

        internal static AutoScaleToken MakeUnknownToken(string text, int startIndex)
        {
            return new AutoScaleToken(AutoScaleTokenType.Unknown, startIndex, startIndex + text.Length - 1, text);
        }
    }
}
