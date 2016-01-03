using System;

namespace Lakewood.AutoScale.Diagnostics
{
    public class Diagnostic: IEquatable<Diagnostic>
    {
        private readonly DiagnosticDescriptor _descriptor;
        private readonly string _message;
        private readonly int _startIndex;
        private readonly int _endIndex;

        public Diagnostic(DiagnosticDescriptor descriptor, string message, int startIndex, int endIndex)
        {
            _descriptor = descriptor;
            _message = message;
            _startIndex = startIndex;
            _endIndex = endIndex;
        }

        public DiagnosticDescriptor Descriptor => _descriptor;
        public string Message => _message;
        public int StartIndex => _startIndex;
        public int EndIndex => _endIndex;

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
                    (uint)_message.GetHashCode() +
                    (uint)_startIndex.GetHashCode() +
                    (uint)_endIndex.GetHashCode());
            }
        }

        public override string ToString()
        {
            return $"{_descriptor.DiagnosticId} ({_startIndex}-{_endIndex}): {_message}";
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
                && _message.Equals(other._message)
                && _startIndex == other._startIndex
                && _endIndex == other._endIndex;
        }

        #endregion IEquatable<T>
    }
}
