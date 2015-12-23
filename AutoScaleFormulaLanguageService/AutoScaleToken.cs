using System;

namespace Lakewood.AutoScaleFormulaLanguageService
{
    public sealed class AutoScaleToken: IEquatable<AutoScaleToken>
    {
        private readonly AutoScaleTokenType _type;
        private readonly int _line; // 0-based
        private readonly int _col;  // 0-based
        private readonly int _startIndex;
        private readonly int _endIndex;
        private readonly string _text;

        public AutoScaleToken(AutoScaleTokenType type, int line, int col, int startIndex, int endIndex, string text)
        {
            _type = type;
            _line = line;
            _col = col;
            _startIndex = startIndex;
            _endIndex = endIndex;
            _text = text;
        }

        public AutoScaleTokenType Type => _type;
        public int Line => _line;
        public int Col => _col;
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
            return $"({_line}, {_col}): ({_startIndex}-{_endIndex}): {_type}: \"{_text}\"";
        }

        public bool Equals(AutoScaleToken other)
        {
            if (other == null)
            {
                return false;
            }

            return Type == other.Type
                && Line == other.Line
                && Col == other.Col
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
