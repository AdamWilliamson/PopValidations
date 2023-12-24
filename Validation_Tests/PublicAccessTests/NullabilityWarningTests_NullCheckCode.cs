#nullable enable
using PopValidations;
using System.Collections.Generic;

public record TestClass(
    int IntField,
    int? NullableIntField,
    string StringField,
    string? NullableStringField,
    List<int> ListOfInts,
    List<int?> ListOfNullableInts,
    List<int>? NullableListOfInts,
    List<int?>? NullableListOfNullableInts
);

public class TestClassValidator : AbstractValidator<TestClass>
{
    public TestClassValidator()
    {
        Describe(x => x.IntField);
        Describe(x => x.NullableIntField);
        Describe(x => x.StringField);
        Describe(x => x.NullableStringField);
        Describe(x => x.ListOfInts);
        Describe(x => x.ListOfNullableInts);
        Describe(x => x.NullableListOfInts);
        Describe(x => x.NullableListOfNullableInts);

        DescribeEnumerable(x => x.ListOfInts);
        DescribeEnumerable(x => x.ListOfNullableInts);
        DescribeEnumerable(x => x.NullableListOfInts);
        DescribeEnumerable(x => x.NullableListOfNullableInts);
    }
}