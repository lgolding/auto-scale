// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System.Collections.Generic;
using System.Text;
using FluentAssertions;

namespace Lakewood.AutoScale.UnitTests
{
    public abstract class SignatureDictionaryTestBase
    {
        /// <summary>
        /// Ensure that, for every MethodSignatureInfo object contained in a dictionary of
        /// method names to method signatures, the Name property of each method signature matches
        /// the key under which it is stored.
        /// </summary>
        protected void VerifyDictionary(IDictionary<string, MethodSignatureInfo[]> dictionary)
        {
            var sb = new StringBuilder();

            foreach (var pair in dictionary)
            {
                MethodSignatureInfo[] signatures = pair.Value;
                for (int i = 0; i < signatures.Length; ++i)
                {
                    if (signatures[i].Name != pair.Key)
                    {
                        sb.AppendLine($"Mismatch: key = {pair.Key}, overload index = {i}, name = {signatures[i].Name}");
                    }
                }
            }

            sb.ToString().Should().BeEmpty();
        }
    }
}
