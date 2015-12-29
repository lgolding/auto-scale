namespace Lakewood.AutoScale.Syntax
{
    internal class IdentifierNode: SyntaxNode
    {
        private readonly string _name;

        internal IdentifierNode(string name)
        {
            _name = name;
        }

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

        public bool Equals(IdentifierNode other)
        {
            if (other == null)
            {
                return false;
            }

            return _name == other._name;
        }
    }
}
