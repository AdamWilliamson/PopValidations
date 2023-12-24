using PopValidations.FieldDescriptors.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PopValidations_Tests.PublicAccessTests
{
    public static class TypeHelper
    {
        public static List<string> GetPubliclyAccessibleValuesFromType(Type type)
        {
            return type
                .GetMembers().Select(p => p.Name)
                .Union(
                    type.GetMethods().Select(p => p.Name)
                )
                .Union(
                    type.GetFields().Select(p => p.Name)
                )
                .ToList();
        }
    }
}
