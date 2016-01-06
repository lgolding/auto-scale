// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lakewood.AutoScale.Syntax
{
    public sealed class FunctionCallNode : SyntaxNode, IEquatable<FunctionCallNode>
    {
        private readonly IdentifierNode _function;
        private readonly AutoScaleToken _openParen;
        private readonly IReadOnlyCollection<SyntaxNode> _arguments;
        private readonly AutoScaleToken _closeParen;

        public FunctionCallNode(
            IdentifierNode identifier,
            AutoScaleToken openParen,
            IEnumerable<SyntaxNode> arguments,
            AutoScaleToken closeParen)
            : base(identifier.StartIndex, closeParen.EndIndex, identifier, arguments)
        {
            _function = identifier;
            _openParen = openParen;
            _arguments = Array.AsReadOnly(arguments.ToArray());
            _closeParen = closeParen;
        }

        public IdentifierNode Function => _function;
        public AutoScaleToken OpenParen => _openParen; 
        public IReadOnlyCollection<SyntaxNode> Arguments => _arguments;
        public AutoScaleToken CloseParen => _closeParen;

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
                uint sum =
                    (uint)_function.GetHashCode() +
                    (uint)_openParen.GetHashCode() +
                    (uint)_closeParen.GetHashCode();

                sum = _arguments.Aggregate(sum, (s, arg) => { return s += (uint)arg.GetHashCode(); });

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
                && _openParen.Equals(other._openParen)
                && _arguments.HasSameElementsAs(other._arguments)
                && _closeParen.Equals(other._closeParen)
                && Equals(other as SyntaxNode);
        }

        #endregion IEquatable<T>

        private string FormatArguments()
        {
            return string.Join(", ", _arguments);
        }
    }
}
