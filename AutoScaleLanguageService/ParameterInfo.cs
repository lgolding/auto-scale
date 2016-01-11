// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
namespace Lakewood.AutoScale
{
    /// <summary>
    /// Describes a parameter to a method.
    /// </summary>
    internal class ParameterInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterInfo"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the parameter.
        /// </param>
        /// <param name="type">
        /// The data type of the parameter.
        /// </param>
        /// <param name="description">
        /// The description of the parameter.
        /// </param>
        public ParameterInfo(
            string name,
            string type,
            string description)
        {
            Name = name;
            Display = $"{type} {name}";
            Description = description;
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the parameter name and type, formatted for display.
        /// </summary>
        public string Display { get; }

        /// <summary>
        /// Gets the description of the parameter.
        /// </summary>
        public string Description { get; }
    }
}