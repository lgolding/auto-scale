// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;

namespace Lakewood.AutoScale.Syntax
{
    public sealed class StringLiteralNode : SyntaxNode, IEquatable<StringLiteralNode>
    {
        public StringLiteralNode(AutoScaleToken token)
            : base(token.StartIndex, token.EndIndex)
        {
            Text = token.Text;
        }

        public string Text { get; }

        public override void Accept(ISyntaxNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        #region Object

        public override bool Equals(object other)
        {
            return Equals(other as StringLiteralNode);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)(
                    (uint)Text.GetHashCode() +
                    (uint)base.GetHashCode());
            }
        }

        public override string ToString()
        {
            return $"{nameof(StringLiteralNode)}({Text})";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(StringLiteralNode other)
        {
            if (other == null)
            {
                return false;
            }

            return Text == other.Text
                && Equals(other as SyntaxNode);
        }

        #endregion IEquatable<T>
    }
}