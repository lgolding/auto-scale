using Microsoft.VisualStudio.Package;

namespace Lakewood.AutoScaleFormulaLanguageService
{
    internal class Token
    {
        private readonly int _lineNumber;
        private readonly int _startIndex;
        private readonly int _endIndex;
        private readonly TokenType _type;

        public Token(int lineNumber, int startIndex, int endIndex, TokenType type)
        {
            _lineNumber = lineNumber;
            _startIndex = startIndex;
            _endIndex = endIndex;
            _type = type;
        }

        public int LineNumber => _lineNumber;
        public int StartIndex => _startIndex;
        public int EndIndex => _endIndex;
        public TokenType Type => _type;
    }
}
