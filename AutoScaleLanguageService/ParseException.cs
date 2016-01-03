using System;
using System.Globalization;
using Lakewood.AutoScale.Diagnostics;

namespace Lakewood.AutoScale
{
    public class ParseException: Exception
    {
        private readonly DiagnosticDescriptor _descriptor;

        public ParseException() : base()
        {
        }

        public ParseException(string message) : base(message)
        {
        }

        public ParseException(string message, Exception inner) : base(message, inner)
        {
        }

        public ParseException(DiagnosticDescriptor descriptor, string message) : base(message)
        {
            _descriptor = descriptor;
        }

        public ParseException(DiagnosticDescriptor diagnosticId, AutoScaleTokenType expectedTokenType, AutoScaleToken actualToken)
            : this(FormatMessage(expectedTokenType, actualToken))
        {
            _descriptor = diagnosticId;
        }

        public DiagnosticDescriptor Descriptor => _descriptor;

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
