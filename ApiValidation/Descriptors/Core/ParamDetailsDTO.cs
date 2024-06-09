namespace ApiValidations.Descriptors.Core;

public class ParamDetailsDTO
{
    public ParamDetailsDTO(string? name, Type type, int index)
    {
        Name = name;
        ParamType = type;
        Index = index;
        IsNullable = IsNullableCheck(type);
    }

    public bool IsNullable { get; }
    public string? Name { get; }
    public Type ParamType { get; }
    public int Index { get; }

    public bool Matches(string name, Type type)
    {
        return Name == name && ParamType == type;
    }

    public bool MatchesValue<T>(T o)
    {
        if (o is null) { return false; }
        if (!o.GetType().IsGenericType) return false;
        if (o.GetType().GetGenericTypeDefinition() != typeof(ParamDetailsDTO)) return false;
        if (o.GetType().GetGenericArguments()?.Length != 1) return false;
        return ParamType.IsAssignableFrom(o.GetType().GetGenericArguments()[0]);
    }

    static bool IsNullableCheck(Type type)
    {
        if (!type.IsValueType) return true; // ref-type
        if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
        return false;
    }
}
