using System;
using System.Collections.Generic;

namespace Lakewood.AutoScale
{
    internal static class SamplingVariableMethodName
    {
        public const string Count = "Count";
        public const string GetSample = "GetSample";
        public const string GetSamplePercent = "GetSamplePercent";
        public const string GetSamplePeriod = "GetSamplePeriod";
        public const string HistoryBeginTime = "HistoryBeginTime";

        public static readonly IReadOnlyCollection<string> All = Array.AsReadOnly(
            new[] {
                Count,
                GetSample,
                GetSamplePercent,
                GetSamplePeriod,
                HistoryBeginTime
            });
    }
}
