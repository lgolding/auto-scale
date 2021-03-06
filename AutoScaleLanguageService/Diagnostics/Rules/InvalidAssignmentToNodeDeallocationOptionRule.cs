﻿// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System.ComponentModel.Composition;
using System.Globalization;
using Lakewood.AutoScale.Syntax;
using Microsoft.VisualStudio.Package;

namespace Lakewood.AutoScale.Diagnostics.Rules
{
    [Export(typeof(IDiagnosticRule))]
    public class InvalidAssignmentToNodeDeallocationOptionRule: DiagnosticRuleBase
    {
        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor("ASF0004", Severity.Error);

        public override void Visit(AssignmentNode assignment)
        {
            string identifierName = assignment.Identifier.Name;
            if (identifierName == VariableName.NodeDeallocationOption)
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
                Resources.DiagnosticInvalidAssignmentToNodeDeallocationOption,
                VariableName.NodeDeallocationOption,
                string.Join(", ", Lexer.NodeDeallocationOptionKeywords));
        }
    }
}
