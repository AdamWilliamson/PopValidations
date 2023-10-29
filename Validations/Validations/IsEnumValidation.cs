using System;
using System.Linq;
using PopValidations.Validations.Base;

namespace PopValidations.Validations;

public class IsEnumValidation<TFieldType> : ValidationComponentBase
{
    private readonly Type enumType;

    public override string DescriptionTemplate { get; protected set; } = "Must be one of '{{enumNames}}' or '{{enumValues}}'.";
    public override string ErrorTemplate { get; protected set; } = "'{{value}}' Is not a valid value.";

    public IsEnumValidation(Type enumType)
    {
        if (enumType == null || !enumType.IsEnum)
        {
            throw new ArgumentException("Type is not of an Enum");
        }

        this.enumType = enumType;
    }

    public override ValidationActionResult Validate(object? value)
    {
        if (
            value != null
            && Enum.TryParse(enumType, value?.ToString(), true, out var result)
            && result != null
            && Enum.IsDefined(enumType, result)
        )
        {
            return CreateValidationSuccessful();
        }
        else
        {
            var valueAsString = value?.ToString() ?? "null";

            return CreateValidationError(
                ("value", valueAsString),
                ("enumNames", string.Join(',', Enum.GetNames(enumType))),
                (
                    "enumValues",
                    string.Join(
                        ',',
                        Enum.GetValues(enumType).Cast<Enum>().Select(x => x.ToString("d")).ToArray()
                    )
                ),
                ("fieldType", GetType())
            );
        }
    }

    private string GetType()
    {
        var type = typeof(TFieldType);
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            type = type.GetGenericArguments()[0];
        }

        return type switch
        {
            Type t when t.IsEnum => "enum",
            Type t when type == typeof(string) => "string",
            Type t when IsNumericType(t) => "numeric",
            _ => "unknown"
        };
    }

    public override DescribeActionResult Describe()
    {
        return CreateDescription(
            ("enumNames", string.Join(',', Enum.GetNames(enumType))),
            (
                "enumValues",
                string.Join(
                    ',',
                    Enum.GetValues(enumType).Cast<Enum>().Select(x => x.ToString("d")).ToArray()
                )
            ),
            ("fieldType", GetType())
        );
    }

    public static bool IsNumericType(Type type)
    {
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Single:
                return true;
            default:
                return false;
        }
    }
}
