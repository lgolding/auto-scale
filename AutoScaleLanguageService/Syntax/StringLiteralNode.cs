using System;

namespace Lakewood.AutoScale.Syntax
{
    public class StringLiteralNode : SyntaxNode, IEquatable<StringLiteralNode>
    {
        private readonly string _text;

        public StringLiteralNode(AutoScaleToken token)
            : base(token.StartIndex, token.EndIndex)
        {
            _text = token.Text;
        }

        public string Text => _text;

        public override void Accept(ISyntaxNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

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

            return _text == other._text
                && Equals(other as SyntaxNode);
        }

        #endregion IEquatable<T>
    }
}