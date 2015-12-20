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
        private static readonly char[] s_logicalOperators = "&|".ToCharArray();

        private static readonly string[] s_keywords = new[]
        {
            "requeue",
            "retaindata",
            "taskcompletion",
            "terminate"
        };

        private readonly IVsTextLines _buffer;
        private string _source;
        private int _index;

        public AutoScaleFormulaScanner(IVsTextLines buffer)
        {
            _buffer = buffer;
        }

        #region IScanner Methods

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

                while (NextCharSatisfies(char.IsWhiteSpace))
                {
                    ++_index;
                }
            }
            else if (ch == '/')
            {
                // Disambiguate division operator from comment.
                if (NextCharIs('/'))
                {
                    tokenInfo.Type = TokenType.Comment;
                    tokenInfo.Color = TokenColor.Comment;
                    _index = _source.Length - 1;
                }
                else
                {
                    tokenInfo.Type = TokenType.Operator;
                    tokenInfo.Color = TokenColor.Text;
                }
            }
            else if (IsLeadingIdentifierCharacter(ch))
            {
                tokenInfo.Type = TokenType.Identifier;

                while (NextCharSatisfies(IsIdentifierCharacter))
                {
                    ++_index;
                }

                string identifier = GetTokenText(tokenInfo);
                tokenInfo.Color = s_keywords.Contains(identifier) ? TokenColor.Keyword : TokenColor.Identifier;
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
            else if (s_logicalOperators.Contains(ch))
            {
                if (NextCharIs(ch))
                {
                    tokenInfo.Type = TokenType.Operator;
                    tokenInfo.Color = TokenColor.Text;
                    ++_index;
                }
                else
                {
                    tokenInfo.Type = TokenType.Unknown;
                    tokenInfo.Color = TokenColor.Text;
                }
            }
            else if (s_operatorsWithOptionalEquals.Contains(ch))
            {
                tokenInfo.Type = TokenType.Operator;
                tokenInfo.Color = TokenColor.Text;

                if (NextCharIs('='))
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

        void IScanner.SetSource(string source, int offset)
        {
            _source = source.Substring(offset);
            _index = 0;
        }

        #endregion IScanner Methods

        private void ParseNumber()
        {
            while (NextCharSatisfies(char.IsDigit))
            {
                ++_index;
            }

            if (NextCharIs('.'))
            {
                ++_index;
                while (NextCharSatisfies(char.IsDigit))
                {
                    ++_index;
                }
            }
        }

        private string GetTokenText(TokenInfo tokenInfo)
        {
            return _source.Substring(tokenInfo.StartIndex, _index - tokenInfo.StartIndex + 1);
        }

        private bool NextCharSatisfies(Func<char, bool> predicate)
        {
            return _index < _source.Length - 1 && predicate(_source[_index + 1]);
        }

        private bool NextCharIs(char ch)
        {
            return NextCharSatisfies(c => c == ch);
        }

        private static bool IsLeadingIdentifierCharacter(char ch)
        {
            return char.IsLetter(ch) || ch == '_' || ch == '$';
        }

        private static bool IsIdentifierCharacter(char ch)
        {
            return char.IsLetterOrDigit(ch) || ch == '_';
        }
    }
}
