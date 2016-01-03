using System;
using System.Collections.Generic;
using System.Linq;

namespace Lakewood.AutoScale.Syntax
{
    public abstract class SyntaxNode: IEquatable<SyntaxNode>
    {
        private readonly IReadOnlyCollection<SyntaxNode> _children;
        private readonly int _startIndex;
        private readonly int _endIndex;

        protected SyntaxNode(int startIndex, int endIndex, params SyntaxNode[] children)
        {
            _startIndex = startIndex;
            _endIndex = endIndex;
            _children = Array.AsReadOnly(children);
        }

        public SyntaxNode(int startIndex, int endIndex, SyntaxNode child1, IEnumerable<SyntaxNode> otherChildren) 
            : this(startIndex, endIndex, new[] { child1 }.Concat(otherChildren).ToArray())
        {
        }

        public SyntaxNode(int startIndex, int endIndex, SyntaxNode child1, SyntaxNode child2, IEnumerable<SyntaxNode> otherChildren)
            : this(startIndex, endIndex, new[] { child1, child2 }.Concat(otherChildren).ToArray())
        {
        }

        public IReadOnlyCollection<SyntaxNode> Children => _children;
        public int StartIndex => _startIndex;
        public int EndIndex => _endIndex;

        public abstract void Accept(ISyntaxNodeVisitor visitor);

        #region IEquatable<T>

        public bool Equals(SyntaxNode other)
        {
            return _startIndex == other._startIndex
                && _endIndex == other._endIndex;
        }

        #endregion
    }
}
