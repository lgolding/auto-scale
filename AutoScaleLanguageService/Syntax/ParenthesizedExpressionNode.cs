﻿// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;

namespace Lakewood.AutoScale.Syntax
{
    public sealed class ParenthesizedExpressionNode : SyntaxNode, IEquatable<ParenthesizedExpressionNode>
    {
        private readonly SyntaxNode _innerExpression;

        public ParenthesizedExpressionNode(AutoScaleToken openParen, SyntaxNode innerExpression, AutoScaleToken closeParen)
            : base(openParen.StartIndex, closeParen.EndIndex, innerExpression)
        {
            _innerExpression = innerExpression;
        }

        public SyntaxNode InnerExpression => _innerExpression;

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
            return _innerExpression.GetHashCode();
        }

        public override string ToString()
        {
            return $"{nameof(ParenthesizedExpressionNode)}({_innerExpression})";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(ParenthesizedExpressionNode other)
        {
            return _innerExpression.Equals(other._innerExpression)
                && Equals(other as SyntaxNode);
        }

        #endregion IEquatable<T>
    }
}