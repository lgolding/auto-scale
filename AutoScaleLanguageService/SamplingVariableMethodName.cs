// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;
using System.Collections.Generic;

namespace Lakewood.AutoScale
{
    internal static class SamplingVariableMethodName
    {
        internal const string Count = "Count";
        internal const string GetSample = "GetSample";
        internal const string GetSamplePercent = "GetSamplePercent";
        internal const string GetSamplePeriod = "GetSamplePeriod";
        internal const string HistoryBeginTime = "HistoryBeginTime";

        internal static readonly IReadOnlyCollection<string> All = Array.AsReadOnly(
            new[] {
                Count,
                GetSample,
                GetSamplePercent,
                GetSamplePeriod,
                HistoryBeginTime
            });
    }
}
