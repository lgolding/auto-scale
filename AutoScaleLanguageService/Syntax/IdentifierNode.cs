using System;

namespace Lakewood.AutoScale.Syntax
{
    public class IdentifierNode: ExpressionNode, IEquatable<IdentifierNode>
    {
        private readonly string _name;

        public IdentifierNode(string name) : base()
        {
            _name = name;
        }

        public string Name => _name;

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
            return $"{typeof(IdentifierNode).Name}({_name})";
        }

        #endregion

        #region IEquatable<T>

        public bool Equals(IdentifierNode other)
        {
            if (other == null)
            {
                return false;
            }

            return _name == other._name;
        }

        #endregion IEquatable<T>
    }
}
