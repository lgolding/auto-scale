namespace Lakewood.AutoScale.Diagnostics
{
    public class DiagnosticDescriptor
    {
        private readonly string _diagnosticId;

        public DiagnosticDescriptor(string diagnosticId)
        {
            _diagnosticId = diagnosticId;
        }

        public string DiagnosticId => _diagnosticId;
    }
}
