using System;
using System.Collections.Generic;
using System.Linq;

namespace Lakewood.AutoScale.Syntax
{
    public sealed class FunctionCallNode : SyntaxNode, IEquatable<FunctionCallNode>
    {
        private readonly IdentifierNode _function;
        private readonly IReadOnlyCollection<SyntaxNode> _arguments;

        public FunctionCallNode(
            IdentifierNode identifier,
            IEnumerable<SyntaxNode> arguments,
            AutoScaleToken closeParen)
            : base(identifier.StartIndex, closeParen.EndIndex, identifier, arguments)
        {
            _function = identifier;
            _arguments = Array.AsReadOnly(arguments.ToArray());
        }

        public IdentifierNode Function => _function;
        public IReadOnlyCollection<SyntaxNode> Arguments => _arguments;

        public override void Accept(ISyntaxNodeVisitor visitor)
        {
            foreach (var arg in _arguments)
            {
                arg.Accept(visitor);
            }

            visitor.Visit(this);
        }

        #region Object

        public override bool Equals(object other)
        {
            return Equals(other as FunctionCallNode);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                uint sum = (uint)_function.GetHashCode();
                foreach (var arg in _arguments)
                {
                    sum += (uint)arg.GetHashCode();
                }

                return (int)sum;
            }
        }

        public override string ToString()
        {
            return $"{nameof(FunctionCallNode)}({_function}({FormatArguments()}))";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(FunctionCallNode other)
        {
            return _function.Equals(other._function)
                && _arguments.HasSameElementsAs(other._arguments)
                && Equals(other as SyntaxNode);
        }

        #endregion IEquatable<T>

        private string FormatArguments()
        {
            return string.Join(", ", _arguments);
        }
    }
}
