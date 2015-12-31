using System;
using System.Collections.Generic;
using System.Linq;

namespace Lakewood.AutoScale.Syntax
{
    public class FormulaNode : SyntaxNode, IEquatable<FormulaNode>
    {
        private readonly IReadOnlyCollection<AssignmentNode> _assignments;

        public FormulaNode(params AssignmentNode[] assignments) : base(assignments)
        {
            _assignments = Array.AsReadOnly(assignments);
        }

        public IReadOnlyCollection<AssignmentNode> Assignments => _assignments;

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
            return $"{typeof(FormulaNode).Name}({string.Join(";", _assignments)})";
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

            return true;
        }

        #endregion IEquatable<T>
    }
}
