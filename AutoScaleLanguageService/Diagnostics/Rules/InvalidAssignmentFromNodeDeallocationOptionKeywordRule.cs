using System.ComponentModel.Composition;
using System.Globalization;
using Lakewood.AutoScale.Syntax;
using Microsoft.VisualStudio.Package;

namespace Lakewood.AutoScale.Diagnostics.Rules
{
    [Export(typeof(IDiagnosticRule))]
    public class InvalidAssignmentFromNodeDeallocationOptionKeywordRule: DiagnosticRuleBase
    {
        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor("ASF0003", Severity.Error);

        internal static string FormatMessage(string keywordName, string identifierName)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                Resources.DiagnosticInvalidAssignmentFromNodeDeallocationOptionKeyword,
                keywordName,
                identifierName,
                VariableName.NodeDeallocationOption);
        }

        public override void Visit(AssignmentNode assignment)
        {
            var keywordNode = assignment.Expression as KeywordNode;
            if (keywordNode != null)
            {
                string keywordName = keywordNode.Name;
                if (Lexer.IsNodeDeallocationOptionKeyword(keywordName))
                {
                    var identifierName = assignment.Identifier.Name;
                    if (identifierName != VariableName.NodeDeallocationOption)
                    {
                        AddDiagnostic(
                            new Diagnostic(
                                Descriptor,
                                FormatMessage(keywordNode.Name, identifierName),
                                keywordNode.StartIndex,
                                keywordNode.EndIndex));
                    }
                }
            }
        }
    }
}
