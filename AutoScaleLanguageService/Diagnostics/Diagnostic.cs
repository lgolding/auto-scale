using System;

namespace Lakewood.AutoScale.Diagnostics
{
    public class Diagnostic: IEquatable<Diagnostic>
    {
        private readonly DiagnosticDescriptor _descriptor;
        private readonly string _message;

        public Diagnostic(DiagnosticDescriptor descriptor, string message)
        {
            _descriptor = descriptor;
            _message = message;
        }

        public DiagnosticDescriptor Descriptor => _descriptor;
        public string Message => _message;

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
                    (uint)_descriptor.GetHashCode() +
                    (uint)_message.GetHashCode());
            }
        }

        public override string ToString()
        {
            return $"{_descriptor}: {_message}";
        }

        #endregion Object

        #region IEquatable<T>

        public bool Equals(Diagnostic other)
        {
            if (other == null)
            {
                return false;
            }

            return _descriptor.Equals(other._descriptor)
                && _message.Equals(other._message);
        }

        #endregion IEquatable<T>
    }
}
