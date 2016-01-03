namespace Lakewood.AutoScale.Syntax
{
    public sealed class BinaryOperationNode : SyntaxNode
    {
        private BinaryOperator _operator;
        private SyntaxNode _left;
        private SyntaxNode _right;

        public BinaryOperationNode(BinaryOperator logicalOr, SyntaxNode left, SyntaxNode right)
            : base(left.StartIndex, right.EndIndex, left, right)
        {
            _operator = logicalOr;
            _left = left;
            _right = right;
        }

        public BinaryOperator Operator => _operator;
        public SyntaxNode Left => _left;
        public SyntaxNode Right => _right;

        public override void Accept(ISyntaxNodeVisitor visitor)
        {
            _left.Accept(visitor);
            _right.Accept(visitor);

            visitor.Visit(this);
        }

        #region Object

        public override bool Equals(object other)
        {
            return Equals(other as BinaryOperationNode);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)(
                    (uint)_operator.GetHashCode() +
                    (uint)_left.GetHashCode() +
                    (uint)_right.GetHashCode());
            }
        }

        public override string ToString()
        {
            return $"{typeof(BinaryOperationNode).Name}({_left} {_operator} {_right})";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(BinaryOperationNode other)
        {
            if (other == null)
            {
                return false;
            }

            return _operator.Equals(other._operator)
                && _left.Equals(other._left)
                && _right.Equals(other._right)
                && Equals(other as SyntaxNode);
        }

        #endregion IEquatable<T>

    }
}