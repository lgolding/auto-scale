using System;
using Lakewood.AutoScale.Diagnostics;

namespace Lakewood.AutoScale
{
    public class ParserException: Exception
    {
        private readonly DiagnosticDescriptor _descriptor;
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
            DiagnosticDescriptor descriptor,
            int startIndex,
            int endIndex,
            string message) : base(message)
        {
            _descriptor = descriptor;
            _startIndex = startIndex;
            _endIndex = endIndex;
        }

        public ParserException(
            DiagnosticDescriptor descriptor,
            AutoScaleTokenType expectedTokenType,
            AutoScaleToken actualToken)
            : this(
                  descriptor,
                  actualToken.StartIndex,
                  actualToken.EndIndex,
                  ParserError.UnexpectedTokenMessage(actualToken, expectedTokenType))
        {
        }

        public DiagnosticDescriptor Descriptor => _descriptor;
        public int StartIndex => _startIndex;
        public int EndIndex => _endIndex;
    }
}
