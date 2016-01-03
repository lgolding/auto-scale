using System;
using Microsoft.VisualStudio.Package;

namespace Lakewood.AutoScale.Diagnostics
{
    public class DiagnosticDescriptor: IEquatable<DiagnosticDescriptor>
    {
        private readonly string _diagnosticId;
        private readonly Severity _severity;

        public DiagnosticDescriptor(string diagnosticId, Severity severity)
        {
            _diagnosticId = diagnosticId;
            _severity = severity;
        }

        public string DiagnosticId => _diagnosticId;
        public Severity Severity => _severity;

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
                    (uint)_diagnosticId.GetHashCode() +
                    (uint)_severity.GetHashCode());
            }
        }

        public override string ToString()
        {
            return _diagnosticId;
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(DiagnosticDescriptor other)
        {
            if (other == null)
            {
                return false;
            }

            return _diagnosticId.Equals(other._diagnosticId)
                && _severity.Equals(other._severity);
        }

        #endregion IEquatable<T>
    }
}
