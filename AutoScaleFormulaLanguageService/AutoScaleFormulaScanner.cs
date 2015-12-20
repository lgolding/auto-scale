using System;
using System.Linq;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Lakewood.AutoScaleFormulaLanguageService
{
    internal class AutoScaleFormulaScanner : IScanner
    {
        private static readonly char[] s_delimiters = "();,".ToCharArray();
        private static readonly char[] s_singleCharacterOperators = "+-*?:.".ToCharArray();
        private static readonly char[] s_operatorsWithOptionalEquals = "<>!=".ToCharArray();

        private readonly IVsTextLines _buffer;
        private string _source;
        private int _index;

        public AutoScaleFormulaScanner(IVsTextLines buffer)
        {
            _buffer = buffer;
        }

        bool IScanner.ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
        {
            if (_index >= _source.Length)
            {
                return false;
            }

            char ch = _source[_index];
            tokenInfo.StartIndex = _index;

            if (char.IsWhiteSpace(ch))
            {
                tokenInfo.Type = TokenType.WhiteSpace;
                tokenInfo.Color = TokenColor.Text;

                char? chPeek;
                while ((chPeek = Peek()).HasValue && char.IsWhiteSpace(chPeek.Value))
                {
                    ++_index;
                }
            }
            else if (ch == '/')
            {
                // Disambiguate division operator from comment.
                if (Peek() == '/')
                {
                    tokenInfo.Type = TokenType.Comment;
                    tokenInfo.Color = TokenColor.Comment;
                    _index = _source.Length - 1;
                }
                else
                {
                    tokenInfo.Type = TokenType.Operator;
                    tokenInfo.Color = TokenColor.Number;
                }
            }
            else if (ch.IsLeadingIdentifierCharacter())
            {
                tokenInfo.Type = TokenType.Identifier;
                tokenInfo.Color = TokenColor.Identifier;

                char? chPeek;
                while ((chPeek = Peek()).HasValue && chPeek.Value.IsIdentifierCharacter())
                {
                    ++_index;
                }
            }
            else if (char.IsDigit(ch))
            {
                tokenInfo.Type = TokenType.Literal;
                tokenInfo.Color = TokenColor.String;
                ParseNumber();
            }
            else if (s_delimiters.Contains(ch))
            {
                tokenInfo.Type = TokenType.Delimiter;
                tokenInfo.Color = TokenColor.Text;
            }
            else if (s_singleCharacterOperators.Contains(ch))
            {
                tokenInfo.Type = TokenType.Operator;
                tokenInfo.Color = TokenColor.Text;
            }
            else if (s_operatorsWithOptionalEquals.Contains(ch))
            {
                tokenInfo.Type = TokenType.Operator;
                tokenInfo.Color = TokenColor.Text;

                if (Peek() == '=')
                {
                    ++_index;
                }
            }
            else
            {
                tokenInfo.Type = TokenType.Unknown;
                tokenInfo.Color = TokenColor.Text;
            }

            tokenInfo.EndIndex = _index++;
            return true;
        }

        private void ParseNumber()
        {
            char? chPeek;
            while ((chPeek = Peek()).HasValue && char.IsDigit(chPeek.Value))
            {
                ++_index;
            }

            if (Peek() == '.')
            {
                ++_index;
            }

            while ((chPeek = Peek()).HasValue && char.IsDigit(chPeek.Value))
            {
                ++_index;
            }
        }

        private char? Peek()
        {
            return _index < _source.Length - 1 ? (char?)_source[_index + 1] : null;
        }

        void IScanner.SetSource(string source, int offset)
        {
            _source = source.Substring(offset);
            _index = 0;
        }
    }

    internal static class CharExtensions
    {
        internal static bool IsLeadingIdentifierCharacter(this char ch)
        {
            return char.IsLetter(ch) || ch == '_' || ch == '$';
        }

        internal static bool IsIdentifierCharacter(this char ch)
        {
            return char.IsLetterOrDigit(ch) || ch == '_';
        }
    }
}
