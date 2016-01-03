using System;

namespace Lakewood.AutoScale.Syntax
{
    public class TernaryOperationNode : SyntaxNode, IEquatable<TernaryOperationNode>
    {
        private readonly SyntaxNode _condition;
        private readonly SyntaxNode _trueValue;
        private readonly SyntaxNode _falseValue;

        public TernaryOperationNode(SyntaxNode condition, SyntaxNode trueValue, SyntaxNode falseValue):
            base(condition.StartIndex, falseValue.EndIndex, condition, trueValue, falseValue)
        {
            _condition = condition;
            _trueValue = trueValue;
            _falseValue = falseValue;
        }

        public override void Accept(ISyntaxNodeVisitor visitor)
        {
            _condition.Accept(visitor);
            _trueValue.Accept(visitor);
            _falseValue.Accept(visitor);

            visitor.Visit(this);
        }

        #region Object

        public override bool Equals(object other)
        {
            return Equals(other as TernaryOperationNode);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)(
                    (uint)_condition.GetHashCode() +
                    (uint)_trueValue.GetHashCode() +
                    (uint)_falseValue.GetHashCode());
            }
        }

        public override string ToString()
        {
            return $"{typeof(TernaryOperationNode).Name}({_condition} ? {_trueValue} : {_falseValue})";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(TernaryOperationNode other)
        {
            if (other == null)
            {
                return false;
            }

            return _condition.Equals(other._condition)
                && _trueValue.Equals(other._trueValue)
                && _falseValue.Equals(other._falseValue)
                && Equals(other as SyntaxNode);
        }

        #endregion IEquatable<T>
    }
}
