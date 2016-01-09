// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lakewood.AutoScale.Syntax
{
    public abstract class SyntaxNode: IEquatable<SyntaxNode>
    {
        protected SyntaxNode(int startIndex, int endIndex, params SyntaxNode[] children)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
            Children = Array.AsReadOnly(children);
        }

        public SyntaxNode(int startIndex, int endIndex, SyntaxNode child1, IEnumerable<SyntaxNode> otherChildren) 
            : this(startIndex, endIndex, new[] { child1 }.Concat(otherChildren).ToArray())
        {
        }

        public SyntaxNode(int startIndex, int endIndex, SyntaxNode child1, SyntaxNode child2, IEnumerable<SyntaxNode> otherChildren)
            : this(startIndex, endIndex, new[] { child1, child2 }.Concat(otherChildren).ToArray())
        {
        }

        public int StartIndex { get; }
        public int EndIndex { get; }
        public IReadOnlyCollection<SyntaxNode> Children { get; }

        public abstract void Accept(ISyntaxNodeVisitor visitor);

        #region Object

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)(
                        (uint)StartIndex.GetHashCode() +
                        (uint)EndIndex.GetHashCode() +
                        Children.Aggregate(0U, (s, c) => s += (uint)c.GetHashCode()));
            }
        }

        #endregion

        #region IEquatable<T>

        public bool Equals(SyntaxNode other)
        {
            return StartIndex == other.StartIndex
                && EndIndex == other.EndIndex;
        }

        #endregion
    }
}
