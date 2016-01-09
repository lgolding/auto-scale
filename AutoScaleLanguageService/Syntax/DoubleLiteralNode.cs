// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;

namespace Lakewood.AutoScale.Syntax
{
    public sealed class DoubleLiteralNode : SyntaxNode, IEquatable<DoubleLiteralNode>
    {
        public DoubleLiteralNode(AutoScaleToken token)
            : base(token.StartIndex, token.EndIndex)
        {
            Number = double.Parse(token.Text);
        }

        public double Number { get; }

        public override void Accept(ISyntaxNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        #region Object

        public override bool Equals(object other)
        {
            return Equals(other as DoubleLiteralNode);
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }

        public override string ToString()
        {
            return $"{nameof(DoubleLiteralNode)}({Number})";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(DoubleLiteralNode other)
        {
            if (other == null)
            {
                return false;
            }

            return Number == other.Number
                && Equals(other as SyntaxNode);
        }

        #endregion IEquatable<T>
    }
}
