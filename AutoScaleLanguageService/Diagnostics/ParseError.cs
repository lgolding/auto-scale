﻿using Microsoft.VisualStudio.Package;

namespace Lakewood.AutoScale.Diagnostics
{
    public static class ParseError
    {
        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor("ASF0001", Severity.Error);
    }
}
