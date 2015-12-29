namespace Lakewood.AutoScale.Syntax
{
    internal class StringLiteralNode : SyntaxNode
    {
        private readonly string _text;

        internal StringLiteralNode(string text)
        {
            _text = text;
        }

        public override bool Equals(object other)
        {
            return Equals(other as StringLiteralNode);
        }

        public override int GetHashCode()
        {
            return _text.GetHashCode();
        }

        public override string ToString()
        {
            return $"{typeof(StringLiteralNode).Name}({_text})";
        }

        public bool Equals(StringLiteralNode other)
        {
            if (other == null)
            {
                return false;
            }

            return _text == other._text;
        }
    }
}
