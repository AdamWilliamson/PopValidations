namespace ApiValidations.Helpers;

public static class GenericNameHelper
{
    //public static string GetNameWithoutGenericArity(this Type? t)
    //{
    //    if (t == null) return string.Empty;

    //    string name = t.Name;
    //    int index = name.IndexOf('`');
    //    name = index == -1 ? name : name.Substring(0, index);

    //    if (t.IsGenericType)
    //    {
    //        name += $"<{string.Join(',', t.GenericTypeArguments.ToList().Select(x => GetNameWithoutGenericArity(x)).ToList())}>";
    //    }

    //    return name;
    //}


    public static string GetNameWithoutGenericArity(this Type? t)
    {
        if (t == null) return string.Empty;

        string name = t.Name;

        // Handle Nullable<T>
        if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            // Get the underlying type of Nullable<T>
            return "Nullable<" + GetNameWithoutGenericArity(t.GetGenericArguments()[0]) + ">";
        }
        // Handle arrays
        else if (t.IsArray)
        {
            // Get the element type of the array
            name = GetNameWithoutGenericArity(t.GetElementType()) + "[]";
        }
        else
        {
            int index = name.IndexOf('`');
            name = index == -1 ? name : name.Substring(0, index);
        }

        // Handle generics
        if (t.IsGenericType)
        {
            name += $"<{string.Join(',', t.GenericTypeArguments.Select(x => GetNameWithoutGenericArity(x)))}>";
        }

        return name;
    }
}