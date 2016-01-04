using System;
using System.Collections.Generic;

namespace Lakewood.AutoScale
{
    internal static class BuiltInFunctionName
    {
        public const string Average = "avg";
        public const string Length = "len";
        public const string Log2 = "lg";
        public const string NaturalLog = "ln";
        public const string Log10 = "log";
        public const string Maximum = "max";
        public const string Minimum = "min";
        public const string EuclideanNorm = "norm";
        public const string Percentile = "percentile";
        public const string Random = "rand";
        public const string Range = "range";
        public const string StandardDeviation = "std";
        public const string Stop = "stop";
        public const string Sum = "sum";
        public const string Time = "time";
        public const string Value = "val";

        public static IReadOnlyCollection<string> All = Array.AsReadOnly(
            new[] {
                Average,
                Length,
                Log2,
                NaturalLog,
                Log10,
                Maximum,
                Minimum,
                EuclideanNorm,
                Percentile,
                Random,
                Range,
                StandardDeviation,
                Stop,
                Sum,
                Time,
                Value
            });
    }
}
