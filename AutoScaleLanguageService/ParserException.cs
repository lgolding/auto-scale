using System;

namespace Lakewood.AutoScale
{
    public class ParserException: Exception
    {
        private readonly int _startIndex;
        private readonly int _endIndex;

        public ParserException() : base()
        {
        }

        public ParserException(string message) : base(message)
        {
        }

        public ParserException(string message, Exception inner) : base(message, inner)
        {
        }

        public ParserException(
            int startIndex,
            int endIndex,
            string message) : base(message)
        {
            _startIndex = startIndex;
            _endIndex = endIndex;
        }

        public ParserException(
            AutoScaleTokenType expectedTokenType,
            AutoScaleToken actualToken)
            : this(
                  actualToken.StartIndex,
                  actualToken.EndIndex,
                  ParserError.UnexpectedTokenMessage(actualToken, expectedTokenType))
        {
        }

        public int StartIndex => _startIndex;
        public int EndIndex => _endIndex;
    }
}
