using System.Globalization;
using System.Linq;
using Lakewood.AutoScale.Syntax;
using Microsoft.VisualStudio.Package;

namespace Lakewood.AutoScale.Diagnostics.Rules
{
    public class UnknownFunctionNameRule : DiagnosticRuleBase
    {
        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor("ASF0005", Severity.Error);

        public override void Visit(FunctionCallNode functionCall)
        {
            if (!BuiltInFunctionName.All.Contains(functionCall.Function.Name))
            {
                string message = FormatMessage(functionCall.Function.Name);

                AddDiagnostic(
                    new Diagnostic(
                        Descriptor,
                        message,
                        functionCall.Function.StartIndex,
                        functionCall.Function.EndIndex));
            }
        }

        internal static string FormatMessage(string functionName)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                Resources.DiagnosticUnknownFunctionName,
                functionName);
        }
    }
}
