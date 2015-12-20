﻿using System.Linq;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Lakewood.AutoScaleFormulaLanguageService
{
    internal class AutoScaleFormulaScanner : IScanner
    {
        private static readonly char[] s_delimiters = "();,".ToCharArray();
        private static readonly char[] s_singleCharacterOperators = "+-/*?:".ToCharArray();
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
            else if (s_delimiters.Contains(ch))
            {
                tokenInfo.Type = TokenType.Delimiter;
                tokenInfo.Color = TokenColor.Text;
            }
            else if (s_singleCharacterOperators.Contains(ch))
            {
                tokenInfo.Type = TokenType.Operator;
                tokenInfo.Color = TokenColor.Number;
            }
            else if (s_operatorsWithOptionalEquals.Contains(ch))
            {
                tokenInfo.Type = TokenType.Operator;
                tokenInfo.Color = TokenColor.Number;

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
}
