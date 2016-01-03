using System;

namespace Lakewood.AutoScale.Diagnostics
{
    public class Diagnostic: IEquatable<Diagnostic>
    {
        private readonly DiagnosticDescriptor _descriptor;

        public Diagnostic(DiagnosticDescriptor descriptor)
        {
            _descriptor = descriptor;
        }

        public DiagnosticDescriptor Descriptor => _descriptor;

        #region Object

        public override bool Equals(object other)
        {
            return Equals(other as Diagnostic);
        }

        public override int GetHashCode()
        {
            return _descriptor.GetHashCode();
        }

        public override string ToString()
        {
            return _descriptor.ToString();
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(Diagnostic other)
        {
            if (other == null)
            {
                return false;
            }

            return _descriptor.Equals(other._descriptor);
        }

        #endregion IEquatable<T>
    }
}
