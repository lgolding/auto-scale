using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Linq;
using Lakewood.AutoScale.Diagnostics;
using Lakewood.AutoScale.Syntax;

namespace Lakewood.AutoScale
{
    /// <summary>
    /// Apply a set of diagnostic rules to a parse tree to produce a set of diagnostics.
    /// </summary>
    public class Analyzer
    {
        // MEF will fill the list of rules, so suppress the compiler warning that _importedRules
        // "is never assigned to, and will always have its default value of null".
        #pragma warning disable 0649
        [ImportMany]
        private IEnumerable<Lazy<IDiagnosticRule>> _importedRules;
        #pragma warning restore 0649

        private IEnumerable<IDiagnosticRule> DiagnosticRules => _importedRules.Select(ir => ir.Value);

        private IReadOnlyCollection<Diagnostic> _diagnostics;
        private readonly Stopwatch _watch = new Stopwatch();
        private TimeSpan _analysisTime;

        public Analyzer()
        {
            ImportRules();
        }

        public void Analyze(FormulaNode formula)
        {
            _watch.Reset();
            _watch.Start();
            formula.Accept(new DiagnosticVisitor(DiagnosticRules));
            _watch.Stop();

            _analysisTime = _watch.Elapsed;
            _diagnostics = Array.AsReadOnly(DiagnosticRules.SelectMany(r => r.Diagnostics).ToArray());
        }

        public IReadOnlyCollection<Diagnostic> Diagnostics => _diagnostics;
        public TimeSpan AnalysisTime => _analysisTime;

        private void ImportRules()
        {
            var catalog = new AssemblyCatalog(GetType().Assembly);
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }
    }
}
