// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;

namespace Lakewood.AutoScale.Syntax
{
    public sealed class AssignmentNode : SyntaxNode, IEquatable<AssignmentNode>
    {
        private readonly IdentifierNode _identifier;
        private readonly SyntaxNode _expression;

        public AssignmentNode(IdentifierNode identifier, SyntaxNode expression)
            : base(identifier.StartIndex, expression.EndIndex, identifier, expression)
        {
            _identifier = identifier;
            _expression = expression;
        }

        public IdentifierNode Identifier => _identifier;
        public SyntaxNode Expression => _expression;

        public override void Accept(ISyntaxNodeVisitor visitor)
        {
            _identifier.Accept(visitor);
            _expression.Accept(visitor);

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
                (int)((uint)_identifier.GetHashCode() + (uint)_expression.GetHashCode()));
        }

        public override string ToString()
        {
            return $"{nameof(AssignmentNode)}({_identifier.Name}={_expression})";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(AssignmentNode other)
        {
            if (other == null)
            {
                return false;
            }

            return _identifier.Equals(other._identifier)
                && _expression.Equals(other._expression)
                && Equals(other as SyntaxNode);
        }

        #endregion IEquatable<T>
    }
}
