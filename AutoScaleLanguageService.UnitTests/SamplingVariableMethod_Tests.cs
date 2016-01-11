// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using Xunit;

namespace Lakewood.AutoScale.UnitTests
{
    public class SamplingVariableMethod_Tests : SignatureDictionaryTestBase
    {
        /// <summary>
        /// Ensure that, for every MethodSignatureInfo object contained in the dictionary of
        /// sampling variable methods, its Name property matches the key under which it is stored.
        /// </summary>
        [Fact]
        public void Dictionary_key_matches_method_info()
        {
            VerifyDictionary(SamplingVariableMethod.Signatures);
        }
    }
}
