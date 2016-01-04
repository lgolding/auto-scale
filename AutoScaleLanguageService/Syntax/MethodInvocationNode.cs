using System;
using System.Collections.Generic;
using System.Linq;

namespace Lakewood.AutoScale.Syntax
{
    public sealed class MethodInvocationNode : SyntaxNode, IEquatable<MethodInvocationNode>
    {
        private readonly IdentifierNode _object;
        private readonly IdentifierNode _method;
        private readonly IReadOnlyCollection<SyntaxNode> _arguments;

        public MethodInvocationNode(
            IdentifierNode @object,
            IdentifierNode method,
            IEnumerable<SyntaxNode> arguments,
            AutoScaleToken closeParen)
            : base(@object.StartIndex, closeParen.EndIndex, @object, method, arguments)
        {
            _object = @object;
            _method = method;
            _arguments = Array.AsReadOnly(arguments.ToArray());
        }

        public IdentifierNode Object => _object;
        public IdentifierNode Method => _method;
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
            return Equals(other as MethodInvocationNode);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                uint sum = (uint)_object.GetHashCode();
                sum += (uint)_method.GetHashCode();

                foreach (var arg in _arguments)
                {
                    sum += (uint)arg.GetHashCode();
                }

                return (int)sum;
            }
        }

        public override string ToString()
        {
            return $"{nameof(MethodInvocationNode)}({_object}.{_method}({FormatArguments()}))";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(MethodInvocationNode other)
        {
            return _object.Equals(other._object)
                && _method.Equals(other._method)
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
