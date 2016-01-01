using System;

namespace Lakewood.AutoScale.Syntax
{
    public class TernaryOperatorNode : SyntaxNode, IEquatable<TernaryOperatorNode>
    {
        private readonly SyntaxNode _condition;
        private readonly SyntaxNode _trueValue;
        private readonly SyntaxNode _falseValue;

        public TernaryOperatorNode(SyntaxNode condition, SyntaxNode trueValue, SyntaxNode falseValue):
            base(condition, trueValue, falseValue)
        {
            _condition = condition;
            _trueValue = trueValue;
            _falseValue = falseValue;
        }

        #region Object

        public override bool Equals(object other)
        {
            return Equals(other as TernaryOperatorNode);
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
            return $"{typeof(TernaryOperatorNode).Name}({_condition} ? {_trueValue} : {_falseValue})";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(TernaryOperatorNode other)
        {
            if (other == null)
            {
                return false;
            }

            return _condition.Equals(other._condition)
                && _trueValue.Equals(other._trueValue)
                && _falseValue.Equals(other._falseValue);
        }

        #endregion IEquatable<T>
    }
}
