﻿using System;

namespace Lakewood.AutoScale.Syntax
{
    public sealed class KeywordNode: SyntaxNode, IEquatable<KeywordNode>
    {
        private readonly string _name;

        public KeywordNode(AutoScaleToken token)
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
            return Equals(other as KeywordNode);
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }

        public override string ToString()
        {
            return $"{typeof(KeywordNode).Name}({_name})";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(KeywordNode other)
        {
            if (other == null)
            {
                return false;
            }

            return _name == other._name;
        }

        #endregion
    }
}