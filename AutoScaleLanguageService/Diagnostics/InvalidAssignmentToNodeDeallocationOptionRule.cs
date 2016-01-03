using System.Globalization;
using Lakewood.AutoScale.Syntax;
using Microsoft.VisualStudio.Package;

namespace Lakewood.AutoScale.Diagnostics
{
    public class InvalidAssignmentToNodeDeallocationOptionRule: DiagnosticRuleBase
    {
        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor("ASF0004", Severity.Error);

        public override void Visit(AssignmentNode assignment)
        {
            string identifierName = assignment.Identifier.Name;
            if (identifierName == "$NodeDeallocationOption")
            {
                var keywordNode = assignment.Expression as KeywordNode;
                if (keywordNode == null)
                {
                    AddDiagnostic(
                        new Diagnostic(
                            Descriptor,
                            FormatMessage(),
                            assignment.Expression.StartIndex,
                            assignment.Expression.EndIndex));
                }
            }
        }

        internal static string FormatMessage()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                Resources.DiagnosticInvalidAssignmentNodeDeallocationOption,
                string.Join(", ", Lexer.NodeDeallocationOptionKeywords));
        }
    }
}
