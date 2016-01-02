using System;
using System.Collections.Generic;
using System.Linq;

namespace Lakewood.AutoScale.Syntax
{
    public class MethodInvocationNode : SyntaxNode, IEquatable<MethodInvocationNode>
    {
        private readonly string _objectName;
        private readonly string _methodName;
        private readonly IReadOnlyCollection<SyntaxNode> _arguments;

        public MethodInvocationNode(IdentifierNode @object, IdentifierNode method, IEnumerable<SyntaxNode> arguments)
            : base(@object, method, arguments)
        {
            _objectName = @object.Name;
            _methodName = method.Name;
            _arguments = Array.AsReadOnly(arguments.ToArray());
        }

        public string ObjectName => _objectName;
        public string MethodName => _methodName;
        public IReadOnlyCollection<SyntaxNode> Arguments => _arguments;

        #region Object

        public override bool Equals(object other)
        {
            return Equals(other as MethodInvocationNode);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                uint sum = (uint)_objectName.GetHashCode();
                sum += (uint)_methodName.GetHashCode();

                foreach (var arg in _arguments)
                {
                    sum += (uint)arg.GetHashCode();
                }

                return (int)sum;
            }
        }

        public override string ToString()
        {
            return $"{typeof(MethodInvocationNode).Name}({_objectName}.{_methodName}({FormatArguments()}))";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(MethodInvocationNode other)
        {
            return _objectName.Equals(other._objectName)
                && _methodName.Equals(other._methodName)
                && _arguments.HasSameElementsAs(other._arguments);
        }

        #endregion IEquatable<T>

        private string FormatArguments()
        {
            return string.Join(", ", _arguments);
        }
    }
}
