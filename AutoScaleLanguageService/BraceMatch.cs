// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;

namespace Lakewood.AutoScale
{
    public class BraceMatch : IEquatable<BraceMatch>
    {
        public BraceMatch(int left, int right)
        {
            Left = left;
            Right = right;
        }

        public int Left { get; }
        public int Right { get; }

        #region Object

        public override bool Equals(object other)
        {
            return Equals(other as BraceMatch);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)(
                    (uint)Left.GetHashCode() +
                    (uint)Right.GetHashCode());
            }
        }

        public override string ToString()
        {
            return $"({Left}, {Right})";
        }

        #endregion

        #region IEquatable<T>

        public bool Equals(BraceMatch other)
        {
            return Left == other.Left
                && Right == other.Right;
        }

        #endregion IEquatable<T>
    }
}