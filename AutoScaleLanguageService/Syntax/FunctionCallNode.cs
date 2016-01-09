// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lakewood.AutoScale.Syntax
{
    public sealed class FunctionCallNode : SyntaxNode, IEquatable<FunctionCallNode>
    {
        public FunctionCallNode(
            IdentifierNode identifier,
            AutoScaleToken openParen,
            IEnumerable<SyntaxNode> arguments,
            AutoScaleToken closeParen)
            : base(identifier.StartIndex, closeParen.EndIndex, identifier, arguments)
        {
            Function = identifier;
            OpenParen = openParen;
            Arguments = Array.AsReadOnly(arguments.ToArray());
            CloseParen = closeParen;
        }

        public IdentifierNode Function { get; }
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
            return Equals(other as FunctionCallNode);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)(
                    (uint)Function.GetHashCode() +
                    (uint)OpenParen.GetHashCode() +
                    (uint)CloseParen.GetHashCode() +
                    Arguments.Aggregate(0U, (s, arg) => { return s += (uint)arg.GetHashCode(); }) +
                    (uint)base.GetHashCode());
            }
        }

        public override string ToString()
        {
            return $"{nameof(FunctionCallNode)}({Function}({FormatArguments()}))";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(FunctionCallNode other)
        {
            return Function.Equals(other.Function)
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
