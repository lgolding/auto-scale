// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System.Collections.Generic;
using Lakewood.AutoScale.Syntax;

namespace Lakewood.AutoScale.Diagnostics
{
    public interface IDiagnosticRule: ISyntaxNodeVisitor
    {
        IReadOnlyCollection<Diagnostic> Diagnostics { get; }
    }
}
