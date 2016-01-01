using System;

namespace Lakewood.AutoScale.Syntax
{
    public class DoubleLiteralNode : SyntaxNode, IEquatable<DoubleLiteralNode>
    {
        private readonly double _number;

        public DoubleLiteralNode(double number) : base()
        {
            _number = number;
        }

        public double Number => _number;

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

            return _number == other._number;
        }

        #endregion IEquatable<T>
    }
}
