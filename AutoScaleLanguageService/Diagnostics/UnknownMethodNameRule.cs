using System.Globalization;
using System.Linq;
using Lakewood.AutoScale.Syntax;

namespace Lakewood.AutoScale.Diagnostics
{
    public class UnknownMethodNameRule : DiagnosticRuleBase
    {
        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor("ASF0002");
         
        public override void Visit(MethodInvocationNode methodInvocation)
        {
            if (!AutoScaleLanguageService.SamplingSystemVariableMembers.Select(m => m.Name).Contains(methodInvocation.MethodName))
            {
                string message = FormatMessage(methodInvocation.MethodName);

                AddDiagnostic(new Diagnostic(Descriptor, message));
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
