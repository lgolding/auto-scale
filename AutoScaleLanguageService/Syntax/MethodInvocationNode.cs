// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lakewood.AutoScale.Syntax
{
    public sealed class MethodInvocationNode : SyntaxNode, IEquatable<MethodInvocationNode>
    {
        public MethodInvocationNode(
            IdentifierNode @object,
            IdentifierNode method,
            AutoScaleToken openParen,
            IEnumerable<SyntaxNode> arguments,
            AutoScaleToken closeParen)
            : base(@object.StartIndex, closeParen.EndIndex, @object, method, arguments)
        {
            Object = @object;
            Method = method;
            OpenParen = openParen;
            Arguments = Array.AsReadOnly(arguments.ToArray());
            CloseParen = closeParen;
        }

        public IdentifierNode Object { get; }
        public IdentifierNode Method { get; }
        public AutoScaleToken OpenParen { get; }
        public IReadOnlyCollection<SyntaxNode> Arguments { get; }
        public AutoScaleToken CloseParen { get; }

        public override void Accept(ISyntaxNodeVisitor visitor)
        {
            foreach (var arg in Arguments)
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
                uint sum = 
                    (uint)Object.GetHashCode() +
                    (uint)Method.GetHashCode() +
                    (uint)OpenParen.GetHashCode() +
                    (uint)CloseParen.GetHashCode();

                sum = Arguments.Aggregate(sum, (s, arg) => { return s += (uint)arg.GetHashCode(); });

                return (int)sum;
            }
        }

        public override string ToString()
        {
            return $"{nameof(MethodInvocationNode)}({Object}.{Method}({FormatArguments()}))";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(MethodInvocationNode other)
        {
            return Object.Equals(other.Object)
                && Method.Equals(other.Method)
                && OpenParen.Equals(other.OpenParen)
                && Arguments.HasSameElementsAs(other.Arguments)
                && CloseParen.Equals(other.CloseParen)
                && Equals(other as SyntaxNode);
        }

        #endregion IEquatable<T>

        private string FormatArguments()
        {
            return string.Join(", ", Arguments);
        }
    }
}
