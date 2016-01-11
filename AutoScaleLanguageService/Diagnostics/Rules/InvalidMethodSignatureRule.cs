// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using Lakewood.AutoScale.Syntax;
using Microsoft.VisualStudio.Package;

namespace Lakewood.AutoScale.Diagnostics.Rules
{
    /// <summary>
    /// Check that every method call has a valid number of arguments.
    /// </summary>
    [Export(typeof(IDiagnosticRule))]
    public class InvalidMethodSignatureRule : DiagnosticRuleBase
    {
        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor("ASF0008", Severity.Error);

        public override void Visit(MethodInvocationNode methodInvocation)
        {
            MethodSignatureInfo[] methods;
            if (!SamplingVariableMethod.Signatures.TryGetValue(methodInvocation.Method.Name, out methods))
            {
                // This is not a known method, so it doesn't matter what the signature is.
                // ASF0002 UnknownMethodName will fire.
                return;
            }

            if (!methods.Select(m => m.Parameters.Length).Contains(methodInvocation.Arguments.Count))
            {
                AddDiagnostic(
                    new Diagnostic(
                        Descriptor,
                        FormatMessage(methodInvocation.Method.Name, methodInvocation.Arguments.Count),
                        methodInvocation.OpenParen.StartIndex,
                        methodInvocation.CloseParen.EndIndex));
            }
        }

        internal static string FormatMessage(string methodName, int argumentCount)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                Resources.DiagnosticInvalidMethodSignature,
                methodName,
                argumentCount);
        }
    }
}
