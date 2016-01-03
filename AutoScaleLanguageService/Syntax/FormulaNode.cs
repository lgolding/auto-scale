using System;
using System.Collections.Generic;
using System.Linq;

namespace Lakewood.AutoScale.Syntax
{
    public sealed class FormulaNode : SyntaxNode, IEquatable<FormulaNode>
    {
        private readonly IReadOnlyCollection<AssignmentNode> _assignments;

        public FormulaNode(params AssignmentNode[] assignments)
            : base(GetStartIndex(assignments), GetEndIndex(assignments), assignments)
        {
            _assignments = Array.AsReadOnly(assignments);
        }

        public IReadOnlyCollection<AssignmentNode> Assignments => _assignments;

        public override void Accept(ISyntaxNodeVisitor visitor)
        {
            foreach (var assignment in _assignments)
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
                return (int)_assignments.Aggregate(0U, (s, a) => s + (uint)a.GetHashCode());
            }
        }

        public override string ToString()
        {
            return $"{nameof(FormulaNode)}({string.Join(";", _assignments)})";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(FormulaNode other)
        {
            if (other == null)
            {
                return false;
            }

            if (other._assignments.Count != _assignments.Count)
            {
                return false;
            }

            AssignmentNode[] assignments = _assignments.ToArray();
            AssignmentNode[] otherAssignments = other._assignments.ToArray();

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
