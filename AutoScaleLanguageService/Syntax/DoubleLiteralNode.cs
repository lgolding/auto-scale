using System;

namespace Lakewood.AutoScale.Syntax
{
    internal class DoubleLiteralNode : SyntaxNode, IEquatable<DoubleLiteralNode>
    {
        private readonly double _number;

        internal DoubleLiteralNode(double number)
        {
            _number = number;
        }

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

        public bool Equals(DoubleLiteralNode other)
        {
            if (other == null)
            {
                return false;
            }

            return _number == other._number;
        }

        public static bool operator ==(DoubleLiteralNode left, DoubleLiteralNode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DoubleLiteralNode left, DoubleLiteralNode right)
        {
            return !(left == right);
        }
    }
}
