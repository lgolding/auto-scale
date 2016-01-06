// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;

namespace Lakewood.AutoScale.Syntax
{
    public sealed class ParenthesizedExpressionNode : SyntaxNode, IEquatable<ParenthesizedExpressionNode>
    {
        private readonly AutoScaleToken _openParen;
        private readonly SyntaxNode _innerExpression;
        private readonly AutoScaleToken _closeParen;

        public ParenthesizedExpressionNode(AutoScaleToken openParen, SyntaxNode innerExpression, AutoScaleToken closeParen)
            : base(openParen.StartIndex, closeParen.EndIndex, innerExpression)
        {
            _openParen = openParen;
            _innerExpression = innerExpression;
            _closeParen = closeParen;
        }

        public AutoScaleToken OpenParen => _openParen;
        public SyntaxNode InnerExpression => _innerExpression;
        public AutoScaleToken CloseParen => _closeParen;

        public override void Accept(ISyntaxNodeVisitor visitor)
        {
            _innerExpression.Accept(visitor);

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
                    (uint)_openParen.GetHashCode() +
                    (uint)_innerExpression.GetHashCode() +
                    (uint)_closeParen.GetHashCode());
            }
        }

        public override string ToString()
        {
            return $"{nameof(ParenthesizedExpressionNode)}({_innerExpression})";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(ParenthesizedExpressionNode other)
        {
            return _openParen.Equals(other._openParen)
                &&_innerExpression.Equals(other._innerExpression)
                && _closeParen.Equals(other._closeParen)
                && Equals(other as SyntaxNode);
        }

        #endregion IEquatable<T>
    }
}