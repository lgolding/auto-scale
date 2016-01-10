// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;
using System.Collections.Generic;

namespace Lakewood.AutoScale
{
    internal static class BuiltInFunction
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

        // This list includes only functions that accept a fixed number of arguments. It
        // does not include functions that are documented to accept a doubleVecList,
        // because those can accept any number of arguments.
        public static readonly Dictionary<string, MethodSignatureInfo[]> Signatures = new Dictionary<string, MethodSignatureInfo[]>
        {
            [Percentile] = new MethodSignatureInfo[]
            {
                new MethodSignatureInfo(
                    Percentile,
                    Resources.PercentileFunctionDescription,
                    new ParameterInfo[]
                    {
                        new ParameterInfo(
                            "vec",
                            $"{TypeName.DoubleVec} vec",
                            Resources.PercentileVecParameterDescription),
                        new ParameterInfo(
                            "perc",
                            $"{TypeName.Double} perc",
                            Resources.PercentilePercParameterDescription)
                    },
                    TypeName.Double)
            },

            [Random] = new MethodSignatureInfo[]
            {
                new MethodSignatureInfo(
                    Random,
                    Resources.RandomFunctionDescription,
                    new ParameterInfo[0],
                    TypeName.Double)
            },

            [Stop] = new MethodSignatureInfo[]
            {
                new MethodSignatureInfo(
                    Stop,
                    Resources.StopFunctionDescription,
                    new ParameterInfo[0],
                    TypeName.Void)
            },
            
            [Time] = new MethodSignatureInfo[]
            {
                new MethodSignatureInfo(
                    Time,
                    Resources.TimeFunctionDescriptionNoArgs,
                    new ParameterInfo[0],
                    TypeName.Timestamp),

                new MethodSignatureInfo(
                    Time,
                    Resources.TimeFunctionDescriptionOneArg,
                    new ParameterInfo[]
                    {
                        new ParameterInfo(
                            "dateTime",
                            $"{TypeName.Timestamp} dateTime",
                            Resources.TimeDateTimeParameterDescription)
                    },
                    TypeName.Timestamp)
            }
        };

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
