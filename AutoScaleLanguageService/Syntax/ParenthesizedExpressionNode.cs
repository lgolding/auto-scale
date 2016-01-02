using System;

namespace Lakewood.AutoScale.Syntax
{
    public class ParenthesizedExpressionNode : SyntaxNode, IEquatable<ParenthesizedExpressionNode>
    {
        private readonly SyntaxNode _innerExpression;

        public ParenthesizedExpressionNode(SyntaxNode innerExpression)
            : base(innerExpression)
        {
            _innerExpression = innerExpression;
        }

        public SyntaxNode InnerExpression => _innerExpression;

        #region Object

        public override bool Equals(object other)
        {
            return Equals(other as ParenthesizedExpressionNode);
        }

        public override int GetHashCode()
        {
            return _innerExpression.GetHashCode();
        }

        public override string ToString()
        {
            return $"{typeof(ParenthesizedExpressionNode).Name}({_innerExpression})";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(ParenthesizedExpressionNode other)
        {
            return _innerExpression.Equals(other._innerExpression);
        }

        #endregion IEquatable<T>
    }
}