﻿// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using Lakewood.AutoScale.Syntax;
using Microsoft.VisualStudio.Package;

namespace Lakewood.AutoScale.Diagnostics.Rules
{
    [Export(typeof(IDiagnosticRule))]
    public class UnknownMethodNameRule : DiagnosticRuleBase
    {
        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor("ASF0002", Severity.Error);
         
        public override void Visit(MethodInvocationNode methodInvocation)
        {
            if (!SamplingVariableMethod.Names.Contains(methodInvocation.Method.Name))
            {
                string message = FormatMessage(methodInvocation.Method.Name);

                AddDiagnostic(
                    new Diagnostic(
                        Descriptor,
                        message,
                        methodInvocation.Method.StartIndex,
                        methodInvocation.Method.EndIndex));
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
