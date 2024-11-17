internal static class TypeHelper
{
    public static bool IsNullableType(this Type type)
    {
        // Check if it's a generic type and if it's a nullable type
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return true;
        }

        // Check if it's a reference type (which is inherently nullable)
        return !type.IsValueType;
    }
}