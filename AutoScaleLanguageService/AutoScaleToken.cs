// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;

namespace Lakewood.AutoScale
{
    public sealed class AutoScaleToken: IEquatable<AutoScaleToken>
    {
        public AutoScaleToken(AutoScaleTokenType type, int startIndex, int endIndex, string text)
        {
            Type = type;
            StartIndex = startIndex;
            EndIndex = endIndex;
            Text = text;
        }

        public AutoScaleTokenType Type { get; }
        public int StartIndex { get; }
        public int EndIndex { get; }
        public string Text { get; }

        public override bool Equals(object other)
        {
            return Equals(other as AutoScaleToken);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)(
                    (uint)Type.GetHashCode() +
                    (uint)StartIndex.GetHashCode() +
                    (uint)EndIndex.GetHashCode() +
                    (uint)Text.GetHashCode());
            }
        }

        public override string ToString()
        {
            return $"{StartIndex}-{EndIndex}: {Type}: \"{Text}\"";
        }

        public bool Equals(AutoScaleToken other)
        {
            if (other == null)
            {
                return false;
            }

            return Type == other.Type
                && StartIndex == other.StartIndex
                && EndIndex == other.EndIndex
                && Text == other.Text;
        }

        public static bool operator ==(AutoScaleToken left, AutoScaleToken right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AutoScaleToken left, AutoScaleToken right)
        {
            return !(left == right);
        }
    }
}
