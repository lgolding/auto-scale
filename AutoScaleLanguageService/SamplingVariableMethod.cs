// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;
using System.Collections.Generic;

namespace Lakewood.AutoScale
{
    internal static class SamplingVariableMethod
    {
        internal const string Count = "Count";
        internal const string GetSample = "GetSample";
        internal const string GetSamplePercent = "GetSamplePercent";
        internal const string GetSamplePeriod = "GetSamplePeriod";
        internal const string HistoryBeginTime = "HistoryBeginTime";

        public static readonly Dictionary<string, MethodSignatureInfo[]> Signatures = new Dictionary<string, MethodSignatureInfo[]>
        {
            [Count] = new MethodSignatureInfo[]
            {
                new MethodSignatureInfo(
                    Count,
                    Resources.CountMethodDescription,
                    new ParameterInfo[0],
                    TypeName.Double)
            },

            [GetSample] = new MethodSignatureInfo[]
            {
                new MethodSignatureInfo(
                    GetSample,
                    Resources.GetSampleMethodDescriptionOneArg,
                    new ParameterInfo[]
                    {
                        new ParameterInfo(
                            "count",
                            TypeName.Double,
                            Resources.GetSampleCountParameterDescription)
                    },
                    TypeName.DoubleVec),

                new MethodSignatureInfo(
                    GetSample,
                    Resources.GetSampleMethodDescriptionTwoArgs,
                    new ParameterInfo[]
                    {
                        new ParameterInfo(
                            "startTime",
                            TypeName.Timestamp,
                            Resources.GetSampleStartTimeParameterDescription),

                        new ParameterInfo(
                            "samplePercent",
                            TypeName.Double,
                            Resources.GetSampleSamplePercentParameterDescription)
                    },
                    TypeName.DoubleVec),

                new MethodSignatureInfo(
                    GetSample,
                    Resources.GetSampleMethodDescriptionThreeArgs,
                    new ParameterInfo[]
                    {
                        new ParameterInfo(
                            "startTime",
                            TypeName.Timestamp,
                            Resources.GetSampleStartTimeParameterDescription),

                        new ParameterInfo(
                            "endTime",
                            TypeName.Timestamp,
                            Resources.GetSampleEndTimeParameterDescription),

                        new ParameterInfo(
                            "samplePercent",
                            TypeName.Double,
                            Resources.GetSampleSamplePercentParameterDescription)
                    },
                    TypeName.DoubleVec),
            },

            [GetSamplePercent] = new MethodSignatureInfo[]
            {
                new MethodSignatureInfo(
                    GetSamplePercent,
                    Resources.GetSamplePercentMethodDescriptionOneArg,
                    new ParameterInfo[]
                    {
                        new ParameterInfo(
                            "startTime",
                            TypeName.Timestamp,
                            Resources.GetSamplePercentStartTimeParameterDescription)
                    },
                    TypeName.Double),

                new MethodSignatureInfo(
                    GetSamplePercent,
                    Resources.GetSamplePercentMethodDescriptionTwoArgs,
                    new ParameterInfo[]
                    {
                        new ParameterInfo(
                            "startTime",
                            TypeName.Timestamp,
                            Resources.GetSamplePercentStartTimeParameterDescription),

                        new ParameterInfo(
                            "endTime",
                            TypeName.Timestamp,
                            Resources.GetSamplePercentEndTimeParameterDescription)

                    },
                    TypeName.Double)
            },

            [GetSamplePeriod] = new MethodSignatureInfo[]
            {
                new MethodSignatureInfo(
                    GetSamplePeriod,
                    Resources.GetSamplePeriodMethodDescription,
                    new ParameterInfo[0],
                    TypeName.Timestamp)
            },

            [HistoryBeginTime] = new MethodSignatureInfo[]
            {
                new MethodSignatureInfo(
                    HistoryBeginTime,
                    Resources.HistoryBeginTimeMethodDescription,
                    new ParameterInfo[0],
                    TypeName.Timestamp)
            }
        };

        internal static readonly IReadOnlyCollection<string> Names = Array.AsReadOnly(
            new[] {
                Count,
                GetSample,
                GetSamplePercent,
                GetSamplePeriod,
                HistoryBeginTime
            });
    }
}
