// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
namespace Lakewood.AutoScale.Syntax
{
    public sealed class BinaryOperationNode : SyntaxNode
    {
        public BinaryOperationNode(BinaryOperator logicalOr, SyntaxNode left, SyntaxNode right)
            : base(left.StartIndex, right.EndIndex, left, right)
        {
            Operator = logicalOr;
            Left = left;
            Right = right;
        }

        public BinaryOperator Operator { get; }
        public SyntaxNode Left { get; }
        public SyntaxNode Right { get; }

        public override void Accept(ISyntaxNodeVisitor visitor)
        {
            Left.Accept(visitor);
            Right.Accept(visitor);

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
                    (uint)Operator.GetHashCode() +
                    (uint)Left.GetHashCode() +
                    (uint)Right.GetHashCode());
            }
        }

        public override string ToString()
        {
            return $"{nameof(BinaryOperationNode)}({Left} {Operator} {Right})";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(BinaryOperationNode other)
        {
            if (other == null)
            {
                return false;
            }

            return Operator.Equals(other.Operator)
                && Left.Equals(other.Left)
                && Right.Equals(other.Right)
                && Equals(other as SyntaxNode);
        }

        #endregion IEquatable<T>

    }
}