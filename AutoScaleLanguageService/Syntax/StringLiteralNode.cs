using System;

namespace Lakewood.AutoScale.Syntax
{
    public class StringLiteralNode : PrimaryExpressionNode, IEquatable<StringLiteralNode>
    {
        private readonly string _text;

        public StringLiteralNode(string text) : base()
        {
            _text = text;
        }

        public string Text => _text;

        #region Object

        public override bool Equals(object other)
        {
            return Equals(other as StringLiteralNode);
        }

        public override int GetHashCode()
        {
            return _text.GetHashCode();
        }

        public override string ToString()
        {
            return $"{typeof(StringLiteralNode).Name}({_text})";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(StringLiteralNode other)
        {
            if (other == null)
            {
                return false;
            }

            return _text == other._text;
        }

        #endregion IEquatable<T>
    }
}