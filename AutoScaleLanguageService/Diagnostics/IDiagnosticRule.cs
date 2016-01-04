using System.Collections.Generic;
using Lakewood.AutoScale.Syntax;

namespace Lakewood.AutoScale.Diagnostics
{
    public interface IDiagnosticRule: ISyntaxNodeVisitor
    {
        IReadOnlyCollection<Diagnostic> Diagnostics { get; }
    }
}
