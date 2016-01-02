using System.Globalization;
using System.Linq;
using Lakewood.AutoScale.Syntax;

namespace Lakewood.AutoScale.Diagnostics
{
    public class UnknownMethodName : DiagnosticBase
    {
        public override void Visit(MethodInvocationNode methodInvocation)
        {
            if (!AutoScaleLanguageService.SamplingSystemVariableMembers.Select(m => m.Name).Contains(methodInvocation.MethodName))
            {
#if BLEAH
                AddError(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.DiagnosticUnknownMethodName,
                        methodInvocation.MethodName));
#else
                AddError("ASF0002");
#endif
            }
        }
    }
}
