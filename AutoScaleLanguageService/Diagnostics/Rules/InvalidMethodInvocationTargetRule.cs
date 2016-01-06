// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using Lakewood.AutoScale.Syntax;
using Microsoft.VisualStudio.Package;

namespace Lakewood.AutoScale.Diagnostics.Rules
{
    [Export(typeof(IDiagnosticRule))]
    public class InvalidMethodInvocationTargetRule : DiagnosticRuleBase
    {
        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor("ASF0006", Severity.Error);

        public override void Visit(MethodInvocationNode methodInvocation)
        {
            if (!SamplingVariableName.All.Contains(methodInvocation.Object.Name))
            {
                AddDiagnostic(
                    new Diagnostic(
                        Descriptor,
                        FormatMessage(methodInvocation.Object.Name),
                        methodInvocation.Object.StartIndex,
                        methodInvocation.Object.EndIndex));
            }
        }

        internal static string FormatMessage(string objectName)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                Resources.DiagnosticInvalidMethodInvocationTarget,
                objectName);
        }
    }
}
