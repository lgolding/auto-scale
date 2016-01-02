using System;
using System.Collections.Generic;
using System.Linq;

namespace Lakewood.AutoScale.Syntax
{
    public abstract class SyntaxNode
    {
        private readonly IReadOnlyCollection<SyntaxNode> _children;

        protected SyntaxNode(params SyntaxNode[] children)
        {
            _children = Array.AsReadOnly(children);
        }

        public SyntaxNode(SyntaxNode child1, IEnumerable<SyntaxNode> otherChildren) 
            : this(new[] { child1 }.Concat(otherChildren).ToArray())
        {
        }

        public SyntaxNode(SyntaxNode child1, SyntaxNode child2, IEnumerable<SyntaxNode> otherChildren)
            : this(new[] { child1, child2 }.Concat(otherChildren).ToArray())
        {
        }

        public IReadOnlyCollection<SyntaxNode> Children => _children;
    }
}
