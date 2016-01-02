using System;
using System.Globalization;

namespace Lakewood.AutoScale
{
    public class ParseException: Exception
    {
        private readonly string _diagnosticId;

        public ParseException() : base()
        {
        }

        public ParseException(string message) : base(message)
        {
        }

        public ParseException(string message, Exception inner) : base(message, inner)
        {
        }

        public ParseException(string diagnosticId, string message) : base(message)
        {
            _diagnosticId = diagnosticId;
        }

        public ParseException(string diagnosticId, AutoScaleTokenType expectedTokenType, AutoScaleToken actualToken)
            : this(FormatMessage(expectedTokenType, actualToken))
        {
            _diagnosticId = diagnosticId;
        }

        public string DiagnosticId => _diagnosticId;

        private static string FormatMessage(AutoScaleTokenType expectedTokenType, AutoScaleToken actualToken)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                Resources.ErrorUnexpectedToken,
                expectedTokenType,
                actualToken.Text,
                actualToken.Type);
        }
    }
}
