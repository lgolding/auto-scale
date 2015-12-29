using System.Collections.Generic;
using System.Linq;

namespace Lakewood.AutoScale.Syntax
{
    public class FormulaNode
    {
        private readonly List<SyntaxNode> _statements = new List<SyntaxNode>();

        public FormulaNode(params SyntaxNode[] statements)
        {
            _statements.AddRange(statements);
        }

        public IReadOnlyList<SyntaxNode> Statements => _statements.AsReadOnly();

        public override bool Equals(object other)
        {
            return Equals(other as FormulaNode);
        }

        public override int GetHashCode()
        {
            return _statements.GetHashCode();
        }

        public override string ToString()
        {
            return $"{typeof(FormulaNode).Name}({FormatStatements()})";
        }

        private string FormatStatements()
        {
            return string.Join(";", _statements);
        }

        public bool Equals(FormulaNode other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.Statements.Count != Statements.Count)
            {
                return false;
            }

            SyntaxNode[] statements = _statements.ToArray();
            SyntaxNode[] otherStatements = other.Statements.ToArray();
            for (int i = 0; i < statements.Length; ++i)
            {
                if (!statements[i].Equals(other.Statements[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
