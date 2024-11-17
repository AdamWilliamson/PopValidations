using System.Diagnostics.CodeAnalysis;

namespace PopHelpers;

public static class ThrowIf
{
    public static void IsNull([NotNull]object? obj, string? message = null)
    {
        if (obj == null) throw new InvalidDataException(message ?? "Object is not supposed to be null.");
    }
}
