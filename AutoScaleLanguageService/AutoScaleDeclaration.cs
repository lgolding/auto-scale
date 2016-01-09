// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
namespace Lakewood.AutoScale
{
    public class AutoScaleDeclaration
    {
        public AutoScaleDeclaration(string name, string description, int typeImageIndex)
        {
            Name = name;
            Description = description;
            TypeImageIndex = typeImageIndex;
        }

        public AutoScaleDeclaration(string name, string description, IconImageIndex iconImageIndex = default(IconImageIndex))
            : this(name, description, (int)iconImageIndex)
        {
        }

        public string Name { get; }
        public string Description { get; }
        public int TypeImageIndex { get; }
    }
}
