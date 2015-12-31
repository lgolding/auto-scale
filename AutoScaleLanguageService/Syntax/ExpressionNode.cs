using System;

namespace Lakewood.AutoScale.Syntax
{
    public class ExpressionNode : SyntaxNode, IEquatable<ExpressionNode>
    {
        protected ExpressionNode(params SyntaxNode[] children) : base(children)
        {
        }

        #region IEquatable<T>

        public bool Equals(ExpressionNode other)
        {
            if (this is DoubleLiteralNode && other is DoubleLiteralNode)
            {
                return ((DoubleLiteralNode)this).Equals((DoubleLiteralNode)other);
            }
            else if (this is StringLiteralNode && other is StringLiteralNode)
            {
                return ((StringLiteralNode)this).Equals((StringLiteralNode)other);
            }
            else if (this is IdentifierNode && other is IdentifierNode)
            {
                return ((IdentifierNode)this).Equals((IdentifierNode)other);
            }
            else
            {
                return false;
            }
        }

        #endregion IEquatable<T>
    }
}
