// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;

namespace Lakewood.AutoScale
{
    public class BraceMatch : IEquatable<BraceMatch>
    {
        private readonly int _leftIndex;
        private readonly int _rightIndex;

        public BraceMatch(int leftIndex, int rightIndex)
        {
            _leftIndex = leftIndex;
            _rightIndex = rightIndex;
        }

        public int Left => _leftIndex;
        public int Right => _rightIndex;

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
                    (uint)_leftIndex.GetHashCode() +
                    (uint)_rightIndex.GetHashCode());
            }
        }

        public override string ToString()
        {
            return $"({_leftIndex}, {_rightIndex})";
        }

        #endregion

        #region IEquatable<T>

        public bool Equals(BraceMatch other)
        {
            return _leftIndex == other._leftIndex
                && _rightIndex == other._rightIndex;
        }

        #endregion IEquatable<T>
    }
}