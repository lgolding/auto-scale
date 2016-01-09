// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;

namespace Lakewood.AutoScale.Syntax
{
    public sealed class AssignmentNode : SyntaxNode, IEquatable<AssignmentNode>
    {
        public AssignmentNode(IdentifierNode identifier, SyntaxNode expression)
            : base(identifier.StartIndex, expression.EndIndex, identifier, expression)
        {
            Identifier = identifier;
            Expression = expression;
        }

        public IdentifierNode Identifier { get; }
        public SyntaxNode Expression { get; }

        public override void Accept(ISyntaxNodeVisitor visitor)
        {
            Identifier.Accept(visitor);
            Expression.Accept(visitor);

            visitor.Visit(this);
        }

        #region Object

        public override bool Equals(object other)
        {
            return Equals(other as AssignmentNode);
        }

        public override int GetHashCode()
        {
            return unchecked (
                (int)((uint)Identifier.GetHashCode() + (uint)Expression.GetHashCode()));
        }

        public override string ToString()
        {
            return $"{nameof(AssignmentNode)}({Identifier.Name}={Expression})";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(AssignmentNode other)
        {
            if (other == null)
            {
                return false;
            }

            return Identifier.Equals(other.Identifier)
                && Expression.Equals(other.Expression)
                && Equals(other as SyntaxNode);
        }

        #endregion IEquatable<T>
    }
}
