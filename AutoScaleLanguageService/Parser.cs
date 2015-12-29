using Lakewood.AutoScale.Syntax;

namespace Lakewood.AutoScale
{
    internal class Parser
    {
        internal SyntaxNode Parse(string input)
        {
            return new DoubleLiteralNode(1.0);
        }
    }
}
