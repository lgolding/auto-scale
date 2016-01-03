using System;

namespace Lakewood.AutoScale.Syntax
{
    public sealed class DoubleLiteralNode : SyntaxNode, IEquatable<DoubleLiteralNode>
    {
        private readonly double _number;

        public DoubleLiteralNode(AutoScaleToken token)
            : base(token.StartIndex, token.EndIndex)
        {
            _number = double.Parse(token.Text);
        }

        public double Number => _number;

        public override void Accept(ISyntaxNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        #region Object

        public override bool Equals(object other)
        {
            return Equals(other as DoubleLiteralNode);
        }

        public override int GetHashCode()
        {
            return _number.GetHashCode();
        }

        public override string ToString()
        {
            return $"{typeof(DoubleLiteralNode).Name}({_number})";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(DoubleLiteralNode other)
        {
            if (other == null)
            {
                return false;
            }

            return _number == other._number
                && Equals(other as SyntaxNode);
        }

        #endregion IEquatable<T>
    }
}
