using System;
using System.Collections.Generic;

namespace Lakewood.AutoScale.Syntax
{
    public abstract class SyntaxNode
    {
        private readonly IReadOnlyCollection<SyntaxNode> _children;

        protected SyntaxNode(params SyntaxNode[] children)
        {
            _children = Array.AsReadOnly(children);
        }

        public IReadOnlyCollection<SyntaxNode> Children => _children;
    }
}
