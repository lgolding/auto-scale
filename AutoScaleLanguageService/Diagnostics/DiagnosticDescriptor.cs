// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;
using Microsoft.VisualStudio.Package;

namespace Lakewood.AutoScale.Diagnostics
{
    public class DiagnosticDescriptor: IEquatable<DiagnosticDescriptor>
    {
        public DiagnosticDescriptor(string diagnosticId, Severity severity)
        {
            DiagnosticId = diagnosticId;
            Severity = severity;
        }

        public string DiagnosticId { get; }
        public Severity Severity { get; }

        #region Object

        public override bool Equals(object other)
        {
            return Equals(other as DiagnosticDescriptor);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)(
                    (uint)DiagnosticId.GetHashCode() +
                    (uint)Severity.GetHashCode());
            }
        }

        public override string ToString()
        {
            return DiagnosticId;
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(DiagnosticDescriptor other)
        {
            if (other == null)
            {
                return false;
            }

            return DiagnosticId.Equals(other.DiagnosticId)
                && Severity.Equals(other.Severity);
        }

        #endregion IEquatable<T>
    }
}
