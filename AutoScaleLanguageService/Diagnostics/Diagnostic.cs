// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;

namespace Lakewood.AutoScale.Diagnostics
{
    public class Diagnostic: IEquatable<Diagnostic>
    {
        public Diagnostic(DiagnosticDescriptor descriptor, string message, int startIndex, int endIndex)
        {
            Descriptor = descriptor;
            Message = message;
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        public DiagnosticDescriptor Descriptor { get; }
        public string Message { get; }
        public int StartIndex { get; }
        public int EndIndex { get; }

        #region Object

        public override bool Equals(object other)
        {
            return Equals(other as Diagnostic);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)(
                    (uint)Descriptor.GetHashCode() +
                    (uint)Message.GetHashCode() +
                    (uint)StartIndex.GetHashCode() +
                    (uint)EndIndex.GetHashCode());
            }
        }

        public override string ToString()
        {
            return $"{Descriptor.DiagnosticId} ({StartIndex}-{EndIndex}): {Message}";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(Diagnostic other)
        {
            if (other == null)
            {
                return false;
            }

            return Descriptor.Equals(other.Descriptor)
                && Message.Equals(other.Message)
                && StartIndex == other.StartIndex
                && EndIndex == other.EndIndex;
        }

        #endregion IEquatable<T>
    }
}
