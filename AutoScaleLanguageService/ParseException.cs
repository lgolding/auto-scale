using System;
using System.Globalization;

namespace Lakewood.AutoScale
{
    public class ParseException: Exception
    {
        private readonly AutoScaleTokenType _expectedTokenType = AutoScaleTokenType.Unknown;
        private readonly AutoScaleToken _actualToken = null;

        public ParseException() : base()
        {
        }

        public ParseException(string message) : base(message)
        {
        }

        public ParseException(string message, Exception inner) : base(message, inner)
        {
        }

        public ParseException(AutoScaleTokenType expectedTokenType, AutoScaleToken actualToken)
            : this(FormatMessage(expectedTokenType, actualToken))
        {
            _expectedTokenType = expectedTokenType;
            _actualToken = actualToken;
        }

        public AutoScaleTokenType ExpectedTokenType => _expectedTokenType;
        public AutoScaleToken ActualToken => _actualToken;

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
