// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lakewood.AutoScale.Syntax
{
    public sealed class FormulaNode : SyntaxNode, IEquatable<FormulaNode>
    {
        public FormulaNode(params AssignmentNode[] assignments)
            : base(GetStartIndex(assignments), GetEndIndex(assignments), assignments)
        {
            Assignments = Array.AsReadOnly(assignments);
        }

        public IReadOnlyCollection<AssignmentNode> Assignments { get; }

        public override void Accept(ISyntaxNodeVisitor visitor)
        {
            foreach (var assignment in Assignments)
            {
                assignment.Accept(visitor);
            }

            visitor.Visit(this);
        }

        #region Object

        public override bool Equals(object other)
        {
            return Equals(other as FormulaNode);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)(
                    Assignments.Aggregate(0U, (s, a) => s + (uint)a.GetHashCode()) +
                    (uint)base.GetHashCode());
            }
        }

        public override string ToString()
        {
            return $"{nameof(FormulaNode)}({string.Join(";", Assignments)})";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(FormulaNode other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.Assignments.Count != Assignments.Count)
            {
                return false;
            }

            AssignmentNode[] assignments = Assignments.ToArray();
            AssignmentNode[] otherAssignments = other.Assignments.ToArray();

            for (int i = 0; i < assignments.Length; ++i)
            {
                if (!assignments[i].Equals(otherAssignments[i]))
                {
                    return false;
                }
            }

            return Equals(other as SyntaxNode);
        }

        #endregion IEquatable<T>

        private static int GetEndIndex(AssignmentNode[] assignments)
        {
            return assignments.Any() ? assignments.First().StartIndex : 0;
        }

        private static int GetStartIndex(AssignmentNode[] assignments)
        {
            return assignments.Any() ? assignments.Last().EndIndex:0;
        }
    }
}
