// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System.Collections.Generic;
using System.Linq;

namespace Lakewood.AutoScale
{
    /// <summary>
    /// Describes a method signature.
    /// </summary>
    public class MethodSignatureInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodSignatureInfo"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the method.
        /// </param>
        /// <param name="description">
        /// The description of this method signature.
        /// </param>
        /// <param name="parameters">
        /// Information about each parameter on this method signature.
        /// </param>
        /// <param name="type">
        /// The return type of this method signature.
        /// </param>
        internal MethodSignatureInfo(
            string name,
            string description,
            IEnumerable<ParameterInfo> parameters,
            string type)
        {
            Name = name;
            Description = description;
            Parameters = parameters.ToArray();
            Type = type;
        }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        internal string Name { get; }

        /// <summary>
        /// Gets the description of this method signature.
        /// </summary>
        internal string Description { get; }

        /// <summary>
        /// Gets information about each parameter on this method signature.
        /// </summary>
        internal ParameterInfo[] Parameters { get; }

        /// <summary>
        /// Gets the return type of this method signature.
        /// </summary>
        internal string Type { get; }
    }
}
