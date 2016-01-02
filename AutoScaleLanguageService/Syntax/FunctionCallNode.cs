using System;

namespace Lakewood.AutoScale.Syntax
{
    public class FunctionCallNode : SyntaxNode, IEquatable<FunctionCallNode>
    {
        private readonly string _functionName;

        public FunctionCallNode(IdentifierNode identifier)
            : base(identifier)
        {
            _functionName = identifier.Name;
        }

        public string FunctionName => _functionName;

        #region Object

        public override bool Equals(object other)
        {
            return Equals(other as FunctionCallNode);
        }

        public override int GetHashCode()
        {
            return _functionName.GetHashCode();
        }

        public override string ToString()
        {
            return $"{typeof(FunctionCallNode).Name}({_functionName})";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(FunctionCallNode other)
        {
            return _functionName.Equals(other._functionName);
        }

        #endregion IEquatable<T>
    }
}
