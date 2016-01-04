using System;
using System.Collections.Generic;

namespace Lakewood.AutoScale
{
    internal static class SamplingVariableName
    {
        internal const string CPUPercent = "$CPUPercent";
        internal const string WallClockSeconds = "$WallClockSeconds";
        internal const string MemoryBytes = "$MemoryBytes";

        internal const string DiskBytes = "$DiskBytes";
        internal const string DiskReadBytes = "$DiskReadBytes";
        internal const string DiskWriteBytes = "$DiskWriteBytes";
        internal const string DiskReadOps = "$DiskReadOps";
        internal const string DiskWriteOps = "$DiskWriteOps";

        internal const string NetworkInBytes = "$NetworkInBytes";
        internal const string NetworkOutBytes = "$NetworkOutBytes";

        internal const string SampleNodeCount = "$SampleNodeCount";

        internal const string ActiveTasks = "$ActiveTasks";
        internal const string RunningTasks = "$RunningTasks";
        internal const string SucceededTasks = "$SucceededTasks";
        internal const string FailedTasks = "$FailedTasks";

        internal const string CurrentDedicated = "$CurrentDedicated";

        internal static readonly IReadOnlyCollection<string> All = Array.AsReadOnly(
            new [] {
                CPUPercent,
                WallClockSeconds,
                MemoryBytes,

                DiskBytes,
                DiskReadBytes,
                DiskWriteBytes,
                DiskReadOps,
                DiskWriteOps,

                NetworkInBytes,
                NetworkOutBytes,

                SampleNodeCount,

                ActiveTasks,
                RunningTasks,
                SucceededTasks,
                FailedTasks,

                CurrentDedicated
            });
    }
}
