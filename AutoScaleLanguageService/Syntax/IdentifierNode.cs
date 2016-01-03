using System;

namespace Lakewood.AutoScale.Syntax
{
    public sealed class IdentifierNode: SyntaxNode, IEquatable<IdentifierNode>
    {
        private readonly string _name;

        public IdentifierNode(AutoScaleToken token)
            : base(token.StartIndex, token.EndIndex)
        {
            _name = token.Text;
        }

        public string Name => _name;

        public override void Accept(ISyntaxNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        #region Object

        public override bool Equals(object other)
        {
            return Equals(other as IdentifierNode);
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }

        public override string ToString()
        {
            return $"{nameof(IdentifierNode)}({_name})";
        }

        #endregion

        #region IEquatable<T>

        public bool Equals(IdentifierNode other)
        {
            if (other == null)
            {
                return false;
            }

            return _name == other._name
                && Equals(other as SyntaxNode);
        }

        #endregion IEquatable<T>
    }
}
