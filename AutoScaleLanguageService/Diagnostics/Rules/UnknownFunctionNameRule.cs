// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using Lakewood.AutoScale.Syntax;
using Microsoft.VisualStudio.Package;

namespace Lakewood.AutoScale.Diagnostics.Rules
{
    [Export(typeof(IDiagnosticRule))]
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
