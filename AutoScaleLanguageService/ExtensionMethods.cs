// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System.Collections.Generic;
using System.Linq;

namespace Lakewood.AutoScale
{
    public static class ExtensionMethods
    {
        public static bool HasSameElementsAs<T>(this IEnumerable<T> left, IEnumerable<T> right)
        {
            if (left.Count() != right.Count())
            {
                return false;
            }

            var leftArray = left.ToArray();
            var rightArray = right.ToArray();
            for (int i = 0; i < leftArray.Length; ++i)
            {
                if (!leftArray[i].Equals(rightArray[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
