﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Lakewood.AutoScale {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Lakewood.AutoScale.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The number of tasks that are in an active state..
        /// </summary>
        internal static string ActiveTasksVariableDescription {
            get {
                return ResourceManager.GetString("ActiveTasksVariableDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The average value for all values in a doubleVecList..
        /// </summary>
        internal static string AverageFunctionDescription {
            get {
                return ResourceManager.GetString("AverageFunctionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Returns the total number of samples in the metric history..
        /// </summary>
        internal static string CountMethodDescription {
            get {
                return ResourceManager.GetString("CountMethodDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The average percentage of CPU usage..
        /// </summary>
        internal static string CPUPercentVariableDescription {
            get {
                return ResourceManager.GetString("CPUPercentVariableDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The current number of dedicated compute nodes..
        /// </summary>
        internal static string CurrentDedicatedVariableDescription {
            get {
                return ResourceManager.GetString("CurrentDedicatedVariableDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value &quot;{0}&quot; cannot be assigned to the variable &quot;{1}&quot;. It can be assigned only to the built-in variable $NodeDeallocationOptions..
        /// </summary>
        internal static string DiagnosticInvalidAssignmentFromNodeDeallocationOptionKeyword {
            get {
                return ResourceManager.GetString("DiagnosticInvalidAssignmentFromNodeDeallocationOptionKeyword", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown method name: {0}..
        /// </summary>
        internal static string DiagnosticUnknownMethodName {
            get {
                return ResourceManager.GetString("DiagnosticUnknownMethodName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The average number of gigabytes used on the local disks..
        /// </summary>
        internal static string DiskBytesVariableDescription {
            get {
                return ResourceManager.GetString("DiskBytesVariableDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The number of bytes read..
        /// </summary>
        internal static string DiskReadBytesVariableDescription {
            get {
                return ResourceManager.GetString("DiskReadBytesVariableDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The count of read disk operations performed..
        /// </summary>
        internal static string DiskReadOpsVariableDescription {
            get {
                return ResourceManager.GetString("DiskReadOpsVariableDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The number of bytes written..
        /// </summary>
        internal static string DiskWriteBytesVariableDescription {
            get {
                return ResourceManager.GetString("DiskWriteBytesVariableDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The count of write disk operations performed..
        /// </summary>
        internal static string DiskWriteOpsVariableDescription {
            get {
                return ResourceManager.GetString("DiskWriteOpsVariableDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expected a token of type {0}, but got token &quot;{1}&quot; of type {2}..
        /// </summary>
        internal static string ErrorUnexpectedToken {
            get {
                return ResourceManager.GetString("ErrorUnexpectedToken", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expected a token of one of the types {0}, but got token &quot;{1}&quot; of type {2}..
        /// </summary>
        internal static string ErrorUnexpectedTokenWithChoices {
            get {
                return ResourceManager.GetString("ErrorUnexpectedTokenWithChoices", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The number of tasks that failed..
        /// </summary>
        internal static string FailedTasksVariableDescription {
            get {
                return ResourceManager.GetString("FailedTasksVariableDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Returns a vector of data samples..
        /// </summary>
        internal static string GetSampleMethodDescription {
            get {
                return ResourceManager.GetString("GetSampleMethodDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Returns the percent of samples a history currently has for a given time interval..
        /// </summary>
        internal static string GetSamplePercentMethodDescription {
            get {
                return ResourceManager.GetString("GetSamplePercentMethodDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Returns the period of the samples taken in a historical sample data set..
        /// </summary>
        internal static string GetSamplePeriodMethodDescription {
            get {
                return ResourceManager.GetString("GetSamplePeriodMethodDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Returns the timestamp of the oldest available data sample for the metric..
        /// </summary>
        internal static string HistoryBeginTimeMethodDescription {
            get {
                return ResourceManager.GetString("HistoryBeginTimeMethodDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The length of the vector created from a doubleVecList..
        /// </summary>
        internal static string LengthFunctionDescription {
            get {
                return ResourceManager.GetString("LengthFunctionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Log base 10..
        /// </summary>
        internal static string Log10FunctionDescription {
            get {
                return ResourceManager.GetString("Log10FunctionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Log base 2..
        /// </summary>
        internal static string Log2FunctionDescription {
            get {
                return ResourceManager.GetString("Log2FunctionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The maximium value in a doubleVecList..
        /// </summary>
        internal static string MaximumFunctionDescription {
            get {
                return ResourceManager.GetString("MaximumFunctionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The average number of megabytes used..
        /// </summary>
        internal static string MemoryBytesVariableDescription {
            get {
                return ResourceManager.GetString("MemoryBytesVariableDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The minimum value in a doubleVecList..
        /// </summary>
        internal static string MinimumFunctionDescription {
            get {
                return ResourceManager.GetString("MinimumFunctionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Natural log..
        /// </summary>
        internal static string NaturalLogFunctionDescription {
            get {
                return ResourceManager.GetString("NaturalLogFunctionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The number of inbound bytes..
        /// </summary>
        internal static string NetworkInBytesVariableDescription {
            get {
                return ResourceManager.GetString("NetworkInBytesVariableDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The number of outbound bytes..
        /// </summary>
        internal static string NetworkOutBytesVariableDescription {
            get {
                return ResourceManager.GetString("NetworkOutBytesVariableDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The two-norm of the vector created from a doubleVecList..
        /// </summary>
        internal static string NormFunctionDescription {
            get {
                return ResourceManager.GetString("NormFunctionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified percentile element of the specified vector..
        /// </summary>
        internal static string PercentileFunctionDescription {
            get {
                return ResourceManager.GetString("PercentileFunctionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A random value between 0.0 and 1.0..
        /// </summary>
        internal static string RandomFunctionDescription {
            get {
                return ResourceManager.GetString("RandomFunctionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The diference between the max and min values in a doubleVecList..
        /// </summary>
        internal static string RangeFunctionDescription {
            get {
                return ResourceManager.GetString("RangeFunctionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The number of tasks in a running state..
        /// </summary>
        internal static string RunningTasksVariableDescription {
            get {
                return ResourceManager.GetString("RunningTasksVariableDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The count of compute nodes..
        /// </summary>
        internal static string SampleNodeCountVariableDescription {
            get {
                return ResourceManager.GetString("SampleNodeCountVariableDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The sample standard deviation in a doubleVecList..
        /// </summary>
        internal static string StandardDeviationFunctionDescription {
            get {
                return ResourceManager.GetString("StandardDeviationFunctionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Stop auto-scaling expression evaluation..
        /// </summary>
        internal static string StopFunctionDescription {
            get {
                return ResourceManager.GetString("StopFunctionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The number of tasks that finished successfully..
        /// </summary>
        internal static string SucceededTasksVariableDescription {
            get {
                return ResourceManager.GetString("SucceededTasksVariableDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The sum of all components of a doubleVecList..
        /// </summary>
        internal static string SumFunctionDescription {
            get {
                return ResourceManager.GetString("SumFunctionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The timestamp of the current time if no parameters passed, or the timestamp of the dateTime string if passed. Supported dateTime formats are W3CDTF and RFC1123..
        /// </summary>
        internal static string TimeFunctionDescription {
            get {
                return ResourceManager.GetString("TimeFunctionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A time interval of length 100 nanoseconds..
        /// </summary>
        internal static string TimeInterval100NanosecondsDescription {
            get {
                return ResourceManager.GetString("TimeInterval100NanosecondsDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A time interval of length 1 day..
        /// </summary>
        internal static string TimeIntervalDayDescription {
            get {
                return ResourceManager.GetString("TimeIntervalDayDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A time interval of length 1 hour..
        /// </summary>
        internal static string TimeIntervalHourDescription {
            get {
                return ResourceManager.GetString("TimeIntervalHourDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A time interval of length 1 microsecond..
        /// </summary>
        internal static string TimeIntervalMicrosecondDescription {
            get {
                return ResourceManager.GetString("TimeIntervalMicrosecondDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A time interval of length 1 millisecond..
        /// </summary>
        internal static string TimeIntervalMillisecondDescription {
            get {
                return ResourceManager.GetString("TimeIntervalMillisecondDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A time interval of length 1 minute..
        /// </summary>
        internal static string TimeIntervalMinuteDescription {
            get {
                return ResourceManager.GetString("TimeIntervalMinuteDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A time interval of length 1 second..
        /// </summary>
        internal static string TimeIntervalSecondDescription {
            get {
                return ResourceManager.GetString("TimeIntervalSecondDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A time interval of length 1 week..
        /// </summary>
        internal static string TimeIntervalWeekDescription {
            get {
                return ResourceManager.GetString("TimeIntervalWeekDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A time interval of length 1 year..
        /// </summary>
        internal static string TimeIntervalYearDescription {
            get {
                return ResourceManager.GetString("TimeIntervalYearDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A time interval of length 0..
        /// </summary>
        internal static string TimeIntervalZeroDescription {
            get {
                return ResourceManager.GetString("TimeIntervalZeroDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User-defined variable..
        /// </summary>
        internal static string UserDefinedVariableDescription {
            get {
                return ResourceManager.GetString("UserDefinedVariableDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value of the element at the specified location the specified vector, with a starting index of zero..
        /// </summary>
        internal static string ValueFunctionDescription {
            get {
                return ResourceManager.GetString("ValueFunctionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The number of seconds consumed..
        /// </summary>
        internal static string WallClockSecondsVariableDescription {
            get {
                return ResourceManager.GetString("WallClockSecondsVariableDescription", resourceCulture);
            }
        }
    }
}
