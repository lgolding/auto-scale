// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;

namespace Lakewood.AutoScale.Syntax
{
    public sealed class UnaryOperationNode: SyntaxNode, IEquatable<UnaryOperationNode>
    {
        public UnaryOperationNode(AutoScaleToken unaryOperatorToken, UnaryOperator @operator, SyntaxNode operand)
            : base(unaryOperatorToken.StartIndex, operand.EndIndex, operand)
        {
            Operator = @operator;
            Operand = operand;
        }

        public UnaryOperator Operator { get; }
        public SyntaxNode Operand { get; }

        public override void Accept(ISyntaxNodeVisitor visitor)
        {
            Operand.Accept(visitor);

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
                    (uint)Operator.GetHashCode() +
                    (uint)Operand.GetHashCode() +
                    (uint)base.GetHashCode());
            }
        }

        public override string ToString()
        {
            return $"{nameof(UnaryOperationNode)}({Operator} {Operand})";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(UnaryOperationNode other)
        {
            if (other == null)
            {
                return false;
            }

            return Operator.Equals(other.Operator)
                && Operand.Equals(other.Operand)
                && Equals(other as SyntaxNode);
        }

        #endregion
    }
}
