using PopValidations.Validations.Base;
using System;

namespace PopValidations_Tests.DepthTests;

public enum FieldObjectEnum
{
    Value1
}

internal record FieldObject(
    int Is,
    string IsEmail,
    string IsEmpty,
    string IsEnum,
    int IsEqualTo,
    int IsGreaterThan,
    int IsGreaterThanOrEqualTo,
    string IsLengthExclusivelyBetween,
    string IsLengthInclusivelyBetween,
    int IsLessThan,
    int IsLessThanOrEqualTo,
    string IsNotEmpty,
    string? IsNotNull,
    string? IsNull
);

internal static class FieldObjectBuilder
{
    private static int NameToInt(string name)
    {
        var intValue = 0;
        foreach (char c in name)
        {
            intValue += Convert.ToInt32(c);
        }
        return intValue;
    }

    public static FieldObject CreateValidTestObject(string name)
    {
        var intValue = NameToInt(name);

        return new FieldObject(
            Is: 5,
            IsEmail: "Test@Email.com",
            IsEmpty: "",
            IsEnum: nameof(FieldObjectEnum.Value1),
            IsEqualTo: intValue,
            IsGreaterThan: 101,
            IsGreaterThanOrEqualTo: 100,
            IsLengthExclusivelyBetween: name,
            IsLengthInclusivelyBetween: name,
            IsLessThan: 99,
            IsLessThanOrEqualTo: 100,
            IsNotEmpty: "Not Empty",
            IsNotNull: "Not Null",
            IsNull: null
        );
    }

    public static FieldObject CreateValidObject(string name)
    {
        var intValue = NameToInt(name);
        
        return new FieldObject(
            Is: 5,
            IsEmail: "Test@Email.com",
            IsEmpty: "",
            IsEnum: nameof(FieldObjectEnum.Value1),
            IsEqualTo: intValue,
            IsGreaterThan: 100,
            IsGreaterThanOrEqualTo: 100,
            IsLengthExclusivelyBetween: name,
            IsLengthInclusivelyBetween: name,
            IsLessThan: 100,
            IsLessThanOrEqualTo: 100,
            IsNotEmpty: "Not Empty",
            IsNotNull: "Not Null",
            IsNull: null
        );
    }

    public static FieldObject CreateFailingObject(string name)
    {
        var intValue = NameToInt(name);

        return new FieldObject(
            Is: 10,
            IsEmail: "NotAnEmail",
            IsEmpty: "NotEmpty",
            IsEnum: "Not an enum value",
            IsEqualTo: intValue + 1,
            IsGreaterThan: 99,
            IsGreaterThanOrEqualTo: 90,
            IsLengthExclusivelyBetween: name+"Fail",
            IsLengthInclusivelyBetween: name + "Fail",
            IsLessThan: 200,
            IsLessThanOrEqualTo: 200,
            IsNotEmpty: "",
            IsNotNull: null,
            IsNull: "Not a null value"
        );
    }

    public static ScopedData<FieldObject> CreateValidScopeObject(string name)
    {
        return new ScopedData<FieldObject>(CreateValidObject(name));
    }

    public static ScopedData<FieldObject> CreateFailingScopeObject(string name)
    {
        return new ScopedData<FieldObject>(CreateFailingObject(name));
    }
}