using System;

namespace Lakewood.AutoScale.Diagnostics
{
    public class DiagnosticDescriptor: IEquatable<DiagnosticDescriptor>
    {
        private readonly string _diagnosticId;

        public DiagnosticDescriptor(string diagnosticId)
        {
            _diagnosticId = diagnosticId;
        }

        public string DiagnosticId => _diagnosticId;

        #region Object

        public override bool Equals(object other)
        {
            return Equals(other as DiagnosticDescriptor);
        }

        public override int GetHashCode()
        {
            return _diagnosticId.GetHashCode();
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

            return _diagnosticId.Equals(other._diagnosticId);
        }

        #endregion IEquatable<T>
    }
}
