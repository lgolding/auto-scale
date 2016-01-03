﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Lakewood.AutoScale.Syntax
{
    public class MethodInvocationNode : SyntaxNode, IEquatable<MethodInvocationNode>
    {
        private readonly IdentifierNode _objectName;
        private readonly IdentifierNode _methodName;
        private readonly IReadOnlyCollection<SyntaxNode> _arguments;

        public MethodInvocationNode(
            IdentifierNode objectName,
            IdentifierNode methodName,
            IEnumerable<SyntaxNode> arguments,
            AutoScaleToken closeParen)
            : base(objectName.StartIndex, closeParen.EndIndex, objectName, methodName, arguments)
        {
            _objectName = objectName;
            _methodName = methodName;
            _arguments = Array.AsReadOnly(arguments.ToArray());
        }

        public IdentifierNode ObjectName => _objectName;
        public IdentifierNode MethodName => _methodName;
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
