using System;

namespace Lakewood.AutoScale.Syntax
{
    public sealed class UnaryOperationNode: SyntaxNode, IEquatable<UnaryOperationNode>
    {
        private readonly UnaryOperator _operator;
        private readonly SyntaxNode _operand;

        public UnaryOperationNode(AutoScaleToken unaryOperatorToken, UnaryOperator @operator, SyntaxNode operand)
            : base(unaryOperatorToken.StartIndex, operand.EndIndex, operand)
        {
            _operator = @operator;
            _operand = operand;
        }

        public UnaryOperator Operator => _operator;
        public SyntaxNode Operand => _operand;

        public override void Accept(ISyntaxNodeVisitor visitor)
        {
            _operand.Accept(visitor);

            visitor.Visit(this);
        }

        #region Object

        public override bool Equals(object other)
        {
            return Equals(other as UnaryOperationNode);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)(
                    (uint)_operator.GetHashCode() +
                    (uint)_operand.GetHashCode());
            }
        }

        public override string ToString()
        {
            return $"{typeof(UnaryOperationNode).Name}({_operator} {_operand})";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(UnaryOperationNode other)
        {
            if (other == null)
            {
                return false;
            }

            return _operator.Equals(other._operator)
                && _operand.Equals(other._operand)
                && Equals(other as SyntaxNode);
        }

        #endregion
    }
}
