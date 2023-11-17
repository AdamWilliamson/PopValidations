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

        public static List<string> GetAllPublicItems(Type type)
        {
            var temp = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                .Where(x => !x.IsPrivate);
            var allPublicFunctions =  temp.Select(m => m.Name).ToList();

            var allPublicProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                .Select(p => p.Name).ToList();
            var allPublicFields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy).Select(p => p.Name).ToList();
            return allPublicFunctions.Concat(allPublicProperties).Concat(allPublicFields).ToList();
        }

        public static List<string> GetAllNonPublicItems(Type type)
        {
            var allProtectedFunctions = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                .Where(x => !x.IsPrivate).Select(m => m.Name).ToList();
            var allProtectedProperties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                .Where(x => !x.GetMethod.IsPrivate).Select(p => p.Name).ToList();
            var allProtectedFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                .Where(x => !x.IsPrivate).Select(p => p.Name).ToList();
            return allProtectedFunctions.Concat(allProtectedProperties).Concat(allProtectedFields).ToList();
        }

        public static List<string> GetAllStaticItems(Type type)
        {
            var staticFunctions = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(x => !x.IsPrivate).Select(m => m.Name).ToList();
            var staticProperties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(x => !x.GetMethod.IsPrivate).Select(m => m.Name).ToList();
            var staticFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(x => !x.IsPrivate).Select(p => p.Name).ToList();
            return staticFunctions.Concat(staticProperties).Concat(staticFields).ToList();
        }

        //// https://stackoverflow.com/questions/17853671/how-to-know-if-a-memberinfo-is-an-explicit-implementation-of-a-property
        //public static bool IsExplicitInterfaceImplementation(MethodInfo method)
        //{
        //    // Check all interfaces implemented in the type that declares
        //    // the method we want to check, with this we'll exclude all methods
        //    // that don't implement an interface method
        //    var declaringType = method.DeclaringType;
        //    foreach (var implementedInterface in declaringType.GetInterfaces())
        //    {
        //        var mapping = declaringType.GetInterfaceMap(implementedInterface);

        //        // If interface isn't implemented in the type that owns
        //        // this method then we can ignore it (for sure it's not
        //        // an explicit implementation)
        //        if (mapping.TargetType != declaringType)
        //            continue;

        //        // Is this method the implementation of this interface?
        //        int methodIndex = Array.IndexOf(mapping.TargetMethods, method);
        //        if (methodIndex == -1)
        //            continue;

        //        // Is it true for any language? Can we just skip this check?
        //        if (!method.IsFinal || !method.IsVirtual)
        //            return false;

        //        // It's not required in all languages to implement every method
        //        // in the interface (if the type is abstract)
        //        string methodName = "";
        //        if (mapping.InterfaceMethods[methodIndex] != null)
        //            methodName = mapping.InterfaceMethods[methodIndex].Name;

        //        // If names don't match then it's explicit
        //        if (!method.Name.Equals(methodName, StringComparison.Ordinal))
        //            return true;
        //    }

        //    return false;
        //}

        //// https://stackoverflow.com/questions/17853671/how-to-know-if-a-memberinfo-is-an-explicit-implementation-of-a-property
        //public static bool IsExplicitInterfaceImplementation(PropertyInfo property)
        //{
        //    // At least one accessor must exists, I arbitrary check first for
        //    // "get" one. Note that in Managed C++ (not C++ CLI) these methods
        //    // are logically separated so they may follow different rules (one of them
        //    // is explicit and the other one is not). It's a pretty corner case
        //    // so we may just ignore it.
        //    if (property.GetMethod != null)
        //        return IsExplicitInterfaceImplementation(property.GetMethod);

        //    return IsExplicitInterfaceImplementation(property.SetMethod);
        //}
    }
}
