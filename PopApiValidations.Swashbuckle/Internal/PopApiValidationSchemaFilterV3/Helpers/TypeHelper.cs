using ApiValidations.Execution;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Reflection.Metadata;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.Helpers;

public static class TypeHelper
{
    public static Type GetUnderlyingType(Type type)
    {
        // If it's a nullable type (e.g., Nullable<int>), return the underlying type
        Type? underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType != null)
        {
            return GetUnderlyingType(underlyingType);
        }

        // Handle Task<T> (e.g., Task<int>, Task<Nullable<DateTime>>, Task<ActionResult<int>>)
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            Type taskType = type.GetGenericArguments()[0]; // Get the generic argument of Task<T>
            return GetUnderlyingType(taskType); // Recurse to handle nested nullable or task types
        }

        // Handle ActionResult<T> and IActionResult<T> (e.g., ActionResult<int>, IActionResult<string>)
        if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(ActionResult<>)))
        {
            Type actionResultType = type.GetGenericArguments()[0]; // Get the generic argument of ActionResult<T> or IActionResult<T>
            return GetUnderlyingType(actionResultType); // Recurse to handle nested nullable or task types
        }

        // If none of the above, return the type itself (non-generic or other)
        return type;
    }

    public static bool IsSimpleType(Type type)
    {
        // If it's a nullable type, get the underlying type (e.g., int? -> int)
        Type underlyingType = GetUnderlyingType(type) ?? type;

        // Simple types are typically primitive types or basic types like int, string, DateTime, etc.
        return underlyingType.IsPrimitive || underlyingType == typeof(string) || underlyingType == typeof(decimal)
               || underlyingType == typeof(DateTime) || underlyingType == typeof(Guid)
               || (underlyingType == typeof(Task) && !underlyingType.IsGenericType);
    }

    public static bool IsArrayType(Type type)
    {
        type = GetUnderlyingType(type) ?? type;

        if (type == typeof(string))
        {
            return false;
        }

        // Check if the type is an array
        if (type.IsArray && type.IsGenericType)
        {
            return true;
        }

        var isEnumerable = type.GetInterfaces()
               .Append(type) // ensure this type is also checked
               .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        // Check if the type is a generic collection like List<T>, LinkedList<T>, etc.
        if (isEnumerable)
        {
            return true;
        }

        return false;
    }

    public static bool IsDictionaryType(Type type)
    {
        type = GetUnderlyingType(type) ?? type;

        return type.GetInterfaces()
               .Append(type) // ensure this type is also checked
               .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>));
    }

    public static bool IsUnTypedArrayType(Type type)
    {
        if (!IsArrayType(type))
        {
            var isEnumerable = type.GetInterfaces()
               .Append(type) // ensure this type is also checked
               .Any(x => !x.IsGenericType && x == typeof(IEnumerable));

            return isEnumerable;
        }

        return false;
    }

    public static bool IsComplexOrEnumerable(Type type)
    {
        Type underlyingType = GetUnderlyingType(type) ?? type;
        // Check if it's a complex type (class) or a collection type (array, List<T>, Dictionary<TKey, TValue>)
        return 
            (
                underlyingType.IsClass 
                && !PopApi.Configuation.TypesToTreatAsSimple.Contains(underlyingType)
            )
            || TypeHelper.IsArrayType(underlyingType) 
            || TypeHelper.IsDictionaryType(underlyingType)
            || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
            || type == typeof(Task)
        ;
    }

    public static Type GetElementType(Type type)
    {
        type = GetUnderlyingType(type) ?? type;

        if (type.IsArray)
        {
            return GetUnderlyingType(type.GetElementType());  // Get the element type for arrays
        }

        if (type.IsGenericType)
        {
            if (TypeHelper.IsDictionaryType(type))
            {
                // Return KeyValuePair<TKey, TValue> for Dictionary<TKey, TValue>
                return typeof(KeyValuePair<,>).MakeGenericType(type.GetGenericArguments());
            }

            if (TypeHelper.IsArrayType(type))
            {
                return GetUnderlyingType(type.GetGenericArguments()[0]);  // Get the generic argument type for List<T>
            }
        }

        return type;
    }

    public static Type GetDictionaryKeyType(Type type)
    {
        type = GetUnderlyingType(type) ?? type;

        if (type.IsGenericType)
        {

            if (TypeHelper.IsDictionaryType(type))
            {
                // Return KeyValuePair<TKey, TValue> for Dictionary<TKey, TValue>
                return type.GetGenericArguments()[0];
            }
        }

        return type;
    }

    public static Type GetDictionaryValueType(Type type)
    {
        type = GetUnderlyingType(type) ?? type;

        if (type.IsGenericType)
        {

            if (TypeHelper.IsDictionaryType(type))
            {
                // Return KeyValuePair<TKey, TValue> for Dictionary<TKey, TValue>
                return type.GetGenericArguments()[1];
            }
        }

        return type;
    }
}
