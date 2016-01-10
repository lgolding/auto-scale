// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Package;

namespace Lakewood.AutoScale
{
    /// <summary>
    /// Represents a collection of method signatures obtained from a parsing operation
    /// in the AutoScale language service.
    /// </summary>
    internal class AutoScaleMethods : Methods
    {
        private readonly MethodSignatureInfo[] _methods;

        internal AutoScaleMethods(IEnumerable<MethodSignatureInfo> methods)
        {
            _methods = methods.ToArray();
        }

        #region Methods

        /// <summary>
        /// Gets the number of overloaded method signatures represented in this collection.
        /// </summary>
        /// <returns>
        /// The number of overloaded method signatures represented in this collection.
        /// </returns>
        public override int GetCount()
        {
            return _methods.Length;
        }

        /// <summary>
        /// Gets the description of the specified method signature.
        /// </summary>
        /// <param name="index">
        /// The index of the desired method signature within the internal list maintained
        /// by this object.
        /// </param>
        /// <returns>
        /// The description of the specified method signature, or null if the method
        /// signature does not exist.
        /// </returns>
        public override string GetDescription(int index)
        {
            return ValidIndex(index) ? _methods[index].Description : null;
        }

        /// <summary>
        /// Gets the name of the specified method.
        /// </summary>
        /// <param name="index">
        /// The index of the desired method signature within the internal list maintained
        /// by this object.
        /// </param>
        /// <returns>
        /// The name of the specified method, or null if the method signature does not
        /// exist.
        /// </returns>
        public override string GetName(int index)
        {
            return ValidIndex(index) ? _methods[index].Name : null;
        }


        /// <summary>
        /// Gets the number of parameters on the specified method signature.
        /// </summary>
        /// <param name="index">
        /// The index of the desired method signature within the internal list maintained
        /// by this object.
        /// </param>
        /// <returns>
        /// The number of parameters on the specified method signature, or -1 if the
        /// method signature does not exist.
        /// </returns>
        public override int GetParameterCount(int index)
        {
            return ValidIndex(index) ? _methods[index].Parameters.Length : -1;
        }

        /// <summary>
        /// Gets information about the specified parameter on the specified method
        /// signature.
        /// </summary>
        /// <param name="index">
        /// The index of the desired method signature within the internal list maintained
        /// by this object.
        /// </param>
        /// <param name="parameter">
        /// The index into the parameter list of the specified method signature.
        /// </param>
        /// <param name="name">
        /// Returns the name of the parameter, or null if the specified method signature
        /// or parameter does not exist.
        /// </param>
        /// <param name="display">
        /// Returns the parameter name and type formatter for display, or null if the
        /// specified method signature or parameter does not exist.
        /// </param>
        /// <param name="description">
        /// Returns the description of the parameter, or null if the specified method
        /// signature or parameter does not exist.
        /// </param>
        public override void GetParameterInfo(int index, int parameter, out string name, out string display, out string description)
        {

            if (!ValidParameterIndex(index, parameter))
            {
                name = null;
                display = null;
                description = null;

                return;
            }

            ParameterInfo parameterInfo = _methods[index].Parameters[parameter];
            name = parameterInfo.Name;
            display = parameterInfo.Display;
            description = parameterInfo.Description;
        }

        /// <summary>
        /// Gets the return type of the specified method signature.
        /// </summary>
        /// <param name="index">
        /// The index of the desired method signature within the internal list maintained
        /// by this object.
        /// </param>
        /// <returns>
        /// The return type of the specified method signature, or null if the specified
        /// method signature does not exist.
        /// </returns>
        public override string GetType(int index)
        {
            return ValidIndex(index) ? _methods[index].Type : null;
        }

        #endregion Methods

        private bool ValidIndex(int index)
        {
            return index >= 0 && index < _methods.Length;
        }

        private bool ValidParameterIndex(int index, int parameter)
        {
            return ValidIndex(index) && parameter >= 0 && parameter < _methods[index].Parameters.Length;
        }
    }
}
