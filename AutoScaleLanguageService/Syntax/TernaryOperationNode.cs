// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;

namespace Lakewood.AutoScale.Syntax
{
    public sealed class TernaryOperationNode : SyntaxNode, IEquatable<TernaryOperationNode>
    {
        public TernaryOperationNode(SyntaxNode condition, SyntaxNode trueValue, SyntaxNode falseValue):
            base(condition.StartIndex, falseValue.EndIndex, condition, trueValue, falseValue)
        {
            Condition = condition;
            TrueValue = trueValue;
            FalseValue = falseValue;
        }

        public SyntaxNode Condition { get; }
        public SyntaxNode TrueValue { get; }
        public SyntaxNode FalseValue { get; }

        public override void Accept(ISyntaxNodeVisitor visitor)
        {
            Condition.Accept(visitor);
            TrueValue.Accept(visitor);
            FalseValue.Accept(visitor);

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
                    (uint)Condition.GetHashCode() +
                    (uint)TrueValue.GetHashCode() +
                    (uint)FalseValue.GetHashCode() +
                    (uint)base.GetHashCode());
            }
        }

        public override string ToString()
        {
            return $"{nameof(TernaryOperationNode)}({Condition} ? {TrueValue} : {FalseValue})";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(TernaryOperationNode other)
        {
            if (other == null)
            {
                return false;
            }

            return Condition.Equals(other.Condition)
                && TrueValue.Equals(other.TrueValue)
                && FalseValue.Equals(other.FalseValue)
                && Equals(other as SyntaxNode);
        }

        #endregion IEquatable<T>
    }
}
