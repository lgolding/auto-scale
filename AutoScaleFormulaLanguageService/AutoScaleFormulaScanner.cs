using System;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Lakewood.AutoScaleFormulaLanguageService
{
    internal class AutoScaleFormulaScanner : IScanner
    {
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

            if (ch == '(' || ch == ')' || ch == ';')
            {
                tokenInfo.Type = TokenType.Delimiter;
                tokenInfo.Color = TokenColor.Text;
            }
            else if (ch == '+' || ch == '-' || ch == '/' || ch == '*' || ch == '?' || ch == ':')
            {
                tokenInfo.Type = TokenType.Operator;
                tokenInfo.Color = TokenColor.Number;
            }
            else if (ch == '<' || ch == '>' || ch == '!')
            {
                tokenInfo.Type = TokenType.Operator;
                tokenInfo.Color = TokenColor.Number;

                if (Peek() == '=')
                {
                    ++_index;
                }
            }
            else if (ch == '=')
            {
                if (Peek() == '=')
                {
                    tokenInfo.Type = TokenType.Operator;
                    tokenInfo.Color = TokenColor.Number;
                    ++_index;
                }
                else
                {
                    tokenInfo.Type = TokenType.Unknown;
                    tokenInfo.Color = TokenColor.Text;
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
