using System.Globalization;
using System.Linq;
using Lakewood.AutoScale.Syntax;
using Microsoft.VisualStudio.Package;

namespace Lakewood.AutoScale.Diagnostics
{
    public class UnknownMethodNameRule : DiagnosticRuleBase
    {
        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor("ASF0002", Severity.Error);
         
        public override void Visit(MethodInvocationNode methodInvocation)
        {
            if (!AutoScaleLanguageService.SamplingSystemVariableMembers.Select(m => m.Name).Contains(methodInvocation.MethodName.Name))
            {
                string message = FormatMessage(methodInvocation.MethodName.Name);

                AddDiagnostic(
                    new Diagnostic(
                        Descriptor,
                        message,
                        methodInvocation.MethodName.StartIndex,
                        methodInvocation.MethodName.EndIndex));
            }
        }

        internal static string FormatMessage(string methodName)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                Resources.DiagnosticUnknownMethodName,
                methodName);
        }
    }
}
