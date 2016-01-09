// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;

namespace Lakewood.AutoScale.Syntax
{
    public sealed class ParenthesizedExpressionNode : SyntaxNode, IEquatable<ParenthesizedExpressionNode>
    {
        public ParenthesizedExpressionNode(AutoScaleToken openParen, SyntaxNode innerExpression, AutoScaleToken closeParen)
            : base(openParen.StartIndex, closeParen.EndIndex, innerExpression)
        {
            OpenParen = openParen;
            InnerExpression = innerExpression;
            CloseParen = closeParen;
        }

        public AutoScaleToken OpenParen { get; }
        public SyntaxNode InnerExpression { get; }
        public AutoScaleToken CloseParen { get; }

        public override void Accept(ISyntaxNodeVisitor visitor)
        {
            InnerExpression.Accept(visitor);

            visitor.Visit(this);
        }


        #region Object

        public override bool Equals(object other)
        {
            return Equals(other as ParenthesizedExpressionNode);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)(
                    (uint)OpenParen.GetHashCode() +
                    (uint)InnerExpression.GetHashCode() +
                    (uint)CloseParen.GetHashCode() +
                    (uint)base.GetHashCode());
            }
        }

        public override string ToString()
        {
            return $"{nameof(ParenthesizedExpressionNode)}({InnerExpression})";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(ParenthesizedExpressionNode other)
        {
            return OpenParen.Equals(other.OpenParen)
                &&InnerExpression.Equals(other.InnerExpression)
                && CloseParen.Equals(other.CloseParen)
                && Equals(other as SyntaxNode);
        }

        #endregion IEquatable<T>
    }
}