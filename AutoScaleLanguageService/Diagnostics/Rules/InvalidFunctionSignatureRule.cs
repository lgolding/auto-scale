// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using Lakewood.AutoScale.Syntax;
using Microsoft.VisualStudio.Package;

namespace Lakewood.AutoScale.Diagnostics.Rules
{
    /// <summary>
    /// Check that every function call has a valid number of arguments.
    /// </summary>
    [Export(typeof(IDiagnosticRule))]
    public class InvalidFunctionSignatureRule : DiagnosticRuleBase
    {
        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor("ASF0007", Severity.Error);

        public override void Visit(FunctionCallNode functionCall)
        {
            MethodSignatureInfo[] methods;
            if (!BuiltInFunction.Signatures.TryGetValue(functionCall.Function.Name, out methods))
            {
                // No signature information is available, either because this is not a known
                // function, or because this function can accept any number of arguments (these
                // are the functions that are documented to accept a doubleVecList). In either
                // case, there's no diagnostic to report.
                return;
            }

            if (!methods.Select(m => m.Parameters.Length).Contains(functionCall.Arguments.Count))
            {
                // None of the valid signatures have the observed number of parameters.
                AddDiagnostic(
                    new Diagnostic(
                        Descriptor,
                        FormatMessage(functionCall.Function.Name, functionCall.Arguments.Count),
                        functionCall.OpenParen.StartIndex,
                        functionCall.CloseParen.EndIndex));
            }
        }

        internal static string FormatMessage(string functionName, int argumentCount)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                Resources.DiagnosticInvalidFunctionSignature,
                functionName,
                argumentCount);
        }
    }
}
