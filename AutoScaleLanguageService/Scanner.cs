using System.Collections.Generic;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Lakewood.AutoScale
{
    internal class Scanner : IScanner
    {
        private class TokenInfoProperties
        {
            public TokenInfoProperties(TokenType type, TokenColor color)
            {
                Type = type;
                Color = color;
            }

            public TokenType Type;
            public TokenColor Color;
        }

        private static readonly Dictionary<AutoScaleTokenType, TokenInfoProperties> s_tokenInfoPropertiesDictionary = new Dictionary<AutoScaleTokenType, TokenInfoProperties>
        {
            [AutoScaleTokenType.Unknown] = new TokenInfoProperties(TokenType.Unknown, TokenColor.Text),
            [AutoScaleTokenType.Comma] = new TokenInfoProperties(TokenType.Delimiter, TokenColor.Text),
            [AutoScaleTokenType.Identifier] = new TokenInfoProperties(TokenType.Identifier, TokenColor.Identifier),
            [AutoScaleTokenType.Keyword] = new TokenInfoProperties(TokenType.Keyword, TokenColor.Keyword),
            [AutoScaleTokenType.LineComment] = new TokenInfoProperties(TokenType.LineComment, TokenColor.Comment),
            [AutoScaleTokenType.DoubleLiteral] = new TokenInfoProperties(TokenType.Literal, TokenColor.String),
            [AutoScaleTokenType.OperatorAddition] = new TokenInfoProperties(TokenType.Operator, TokenColor.Text),
            [AutoScaleTokenType.OperatorAssign] = new TokenInfoProperties(TokenType.Operator, TokenColor.Text),
            [AutoScaleTokenType.OperatorDivision] = new TokenInfoProperties(TokenType.Operator, TokenColor.Text),
            [AutoScaleTokenType.OperatorEquality] = new TokenInfoProperties(TokenType.Operator, TokenColor.Text),
            [AutoScaleTokenType.OperatorGreaterThan] = new TokenInfoProperties(TokenType.Operator, TokenColor.Text),
            [AutoScaleTokenType.OperatorGreaterThanOrEqual] = new TokenInfoProperties(TokenType.Operator, TokenColor.Text),
            [AutoScaleTokenType.OperatorLessThan] = new TokenInfoProperties(TokenType.Operator, TokenColor.Text),
            [AutoScaleTokenType.OperatorLessThanOrEqual] = new TokenInfoProperties(TokenType.Operator, TokenColor.Text),
            [AutoScaleTokenType.OperatorLogicalAnd] = new TokenInfoProperties(TokenType.Operator, TokenColor.Text),
            [AutoScaleTokenType.OperatorLogicalOr] = new TokenInfoProperties(TokenType.Operator, TokenColor.Text),
            [AutoScaleTokenType.OperatorMemberSelect] = new TokenInfoProperties(TokenType.Operator, TokenColor.Text),
            [AutoScaleTokenType.OperatorMultiplication] = new TokenInfoProperties(TokenType.Operator, TokenColor.Text),
            [AutoScaleTokenType.OperatorNot] = new TokenInfoProperties(TokenType.Operator, TokenColor.Text),
            [AutoScaleTokenType.OperatorNotEqual] = new TokenInfoProperties(TokenType.Operator, TokenColor.Text),
            [AutoScaleTokenType.OperatorSubtraction] = new TokenInfoProperties(TokenType.Operator, TokenColor.Text),
            [AutoScaleTokenType.OperatorTernaryColon] = new TokenInfoProperties(TokenType.Operator, TokenColor.Text),
            [AutoScaleTokenType.OperatorTernaryQuestion] = new TokenInfoProperties(TokenType.Operator, TokenColor.Text),
            [AutoScaleTokenType.ParenClose] = new TokenInfoProperties(TokenType.Delimiter, TokenColor.Text),
            [AutoScaleTokenType.ParenOpen] = new TokenInfoProperties(TokenType.Delimiter, TokenColor.Text),
            [AutoScaleTokenType.Semicolon] = new TokenInfoProperties(TokenType.Delimiter, TokenColor.Text),
            [AutoScaleTokenType.StringLiteral] = new TokenInfoProperties(TokenType.Literal, TokenColor.String),
            [AutoScaleTokenType.WhiteSpace] = new TokenInfoProperties(TokenType.WhiteSpace, TokenColor.Text)
        };

        private Lexer _lexer;

        public Scanner(IVsTextLines buffer)
        {
        }

        bool IScanner.ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
        {
            AutoScaleToken token = _lexer.GetNextToken();
            if (token.Type == AutoScaleTokenType.End)
            {
                return false;
            }

            TokenInfoProperties properties = s_tokenInfoPropertiesDictionary[token.Type];

            tokenInfo.StartIndex = token.StartIndex;
            tokenInfo.EndIndex = token.EndIndex;
            tokenInfo.Type = properties.Type;
            tokenInfo.Color = properties.Color;
            tokenInfo.Trigger = TokenTriggers.None;

            if (token.Type == AutoScaleTokenType.OperatorMemberSelect)
            {
                tokenInfo.Trigger |= TokenTriggers.MemberSelect;
            }
            else if (token.Type == AutoScaleTokenType.ParenClose)
            {
                tokenInfo.Trigger |= TokenTriggers.MatchBraces;
            }

            return true;
        }

        void IScanner.SetSource(string source, int offset)
        {
            _lexer = new Lexer(source.Substring(offset));
        }
    }
}
