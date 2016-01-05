using System;
using Lakewood.AutoScale.Diagnostics;

namespace Lakewood.AutoScale
{
    public class ParseException: Exception
    {
        private readonly DiagnosticDescriptor _descriptor;
        private readonly int _startIndex;
        private readonly int _endIndex;

        public ParseException() : base()
        {
        }

        public ParseException(string message) : base(message)
        {
        }

        public ParseException(string message, Exception inner) : base(message, inner)
        {
        }

        public ParseException(
            DiagnosticDescriptor descriptor,
            int startIndex,
            int endIndex,
            string message) : base(message)
        {
            _descriptor = descriptor;
            _startIndex = startIndex;
            _endIndex = endIndex;
        }

        public ParseException(
            DiagnosticDescriptor descriptor,
            AutoScaleTokenType expectedTokenType,
            AutoScaleToken actualToken)
            : this(
                  descriptor,
                  actualToken.StartIndex,
                  actualToken.EndIndex,
                  ParserErrorMessage.UnexpectedToken(actualToken, expectedTokenType))
        {
        }

        public DiagnosticDescriptor Descriptor => _descriptor;
        public int StartIndex => _startIndex;
        public int EndIndex => _endIndex;
    }
}
