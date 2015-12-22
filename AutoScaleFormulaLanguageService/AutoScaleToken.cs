using System;

namespace Lakewood.AutoScaleFormulaLanguageService
{
    public sealed class AutoScaleToken: IEquatable<AutoScaleToken>
    {
        private readonly AutoScaleTokenType _type;
        private readonly int _startIndex;
        private readonly int _endIndex;
        private readonly string _text;

        public AutoScaleToken(AutoScaleTokenType type, int startIndex, int endIndex, string text)
        {
            _type = type;
            _startIndex = startIndex;
            _endIndex = endIndex;
            _text = text;
        }

        public AutoScaleTokenType Type => _type;
        public int StartIndex => _startIndex;
        public int EndIndex => _endIndex;
        public string Text => _text;

        public override bool Equals(object other)
        {
            return Equals(other as AutoScaleToken);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{_startIndex}-{_endIndex}: {_type}: \"{_text}\"";
        }

        public bool Equals(AutoScaleToken other)
        {
            if (other == null)
            {
                return false;
            }

            return Type == other.Type
                && StartIndex == other.StartIndex
                && EndIndex == other.EndIndex
                && Text == other.Text;
        }

        public static bool operator ==(AutoScaleToken left, AutoScaleToken right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AutoScaleToken left, AutoScaleToken right)
        {
            return !(left == right);
        }
    }
}
