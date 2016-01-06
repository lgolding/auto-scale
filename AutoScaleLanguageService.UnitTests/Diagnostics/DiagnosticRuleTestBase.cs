// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System.Linq;
using FluentAssertions;
using Lakewood.AutoScale.Diagnostics;

namespace Lakewood.AutoScale.UnitTests.Diagnostics
{
    public abstract class DiagnosticRuleTestBase
    {
        protected void RunTestCase(string testName, string input, Diagnostic[] expectedDiagnostics)
        {
            var parser = new Parser(input);
            var formulaNode = parser.Parse();

            var analyzer = new Analyzer();
            analyzer.Analyze(formulaNode);

            var allDiagnostics = parser.Diagnostics.Union(analyzer.Diagnostics);

            allDiagnostics.Count().Should().Be(expectedDiagnostics.Length);
            allDiagnostics.Should().ContainInOrder(expectedDiagnostics);
        }
    }
}
