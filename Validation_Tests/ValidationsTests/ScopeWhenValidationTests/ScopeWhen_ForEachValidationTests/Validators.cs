using PopValidations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PopValidations_Tests.ValidationsTests.ScopeWhenValidationTests.ScopeWhen_ForEachValidationTests;

public record Base(string? DependantField);
public record Level1(bool Check, string? DependantField, List<Level2>? Child) : Base(DependantField);
public record Level2(bool Check, string? DependantField, List<Level3>? Child) : Base(DependantField);
public record Level3(bool Check, string? DependantField, List<Level4>? Child) : Base(DependantField);
public record Level4(bool Check, string? DependantField, List<Level5>? Child) : Base(DependantField);
public record Level5(bool Check, string? DependantField) : Base(DependantField);

public class Level1Validator : AbstractValidator<Level1>
{
    public Level1Validator()
    {
        Describe(x => x.DependantField).IsEqualTo("Level1");

        ScopeWhen(
            "When Level1.Child is not null",
            x => Task.FromResult(x.Child != null),
            "Database Value",
            (x) => DataRetriever.GetValue(x),
            (retrievedData) =>
            {
                Describe(x => x.DependantField).IsEqualTo(retrievedData);

                ScopeWhen(
                    "When Level1.Check is true",
                    x => Task.FromResult(x.Check == true),
                    "Another DB Value",
                    (x) => DataRetriever.GetMoreValue(x),
                    (moreData) =>
                    {
                        Describe(x => x.DependantField).IsEqualTo(moreData);
                    }
                );

                DescribeEnumerable(x => x.Child)
                    .ForEach(x => x.SetValidator(new Level2Validator()));
            }
        );
    }
}

public class Level2Validator : AbstractSubValidator<Level2>
{
    public Level2Validator()
    {
        Describe(x => x.DependantField).IsEqualTo("Level2");

        ScopeWhen(
            "When Level2.Child is not null",
            x => Task.FromResult(x.Child != null),
            "Database Value",
            (x) => DataRetriever.GetValue(x),
            (retrievedData) =>
            {
                Describe(x => x.DependantField).IsEqualTo(retrievedData);

                ScopeWhen(
                    "When Level2.Check is true",
                    x => Task.FromResult(x.Check == true),
                    "Another DB Value",
                    (x) => DataRetriever.GetMoreValue(x),
                    (moreData) =>
                    {
                        Describe(x => x.DependantField).IsEqualTo(moreData);
                    }
                );

                DescribeEnumerable(x => x.Child)
                    .ForEach(x => x.SetValidator(new Level3Validator()));
            }
        );
    }
}

public class Level3Validator : AbstractSubValidator<Level3>
{
    public Level3Validator()
    {
        Describe(x => x.DependantField).IsEqualTo("Level3");

        ScopeWhen(
            "When Level3.Child is not null",
            x => Task.FromResult(x.Child != null),
            "Database Value",
            (x) => DataRetriever.GetValue(x),
            (retrievedData) =>
            {
                Describe(x => x.DependantField).IsEqualTo(retrievedData);

                ScopeWhen(
                    "When Level3.Check is true",
                    x => Task.FromResult(x.Check == true),
                    "Another DB Value",
                    (x) => DataRetriever.GetMoreValue(x),
                    (moreData) =>
                    {
                        Describe(x => x.DependantField).IsEqualTo(moreData);
                    }
                );

                DescribeEnumerable(x => x.Child)
                    .ForEach(x => x.SetValidator(new Level4Validator()));
            }
        );
    }
}

public class Level4Validator : AbstractSubValidator<Level4>
{
    public Level4Validator()
    {
        Describe(x => x.DependantField).IsEqualTo("Level4");

        ScopeWhen(
            "When Level4.Child is not null",
            x => Task.FromResult(x.Child != null),
            "Database Value",
            (x) => DataRetriever.GetValue(x),
            (retrievedData) =>
            {
                Describe(x => x.DependantField).IsEqualTo(retrievedData);

                ScopeWhen(
                    "When Level4.Check is true",
                    x => Task.FromResult(x.Check == true),
                    "Another DB Value",
                    (x) => DataRetriever.GetMoreValue(x),
                    (moreData) =>
                    {
                        Describe(x => x.DependantField).IsEqualTo(moreData);
                    }
                );

                DescribeEnumerable(x => x.Child)
                    .ForEach(x => x.SetValidator(new Level5Validator()));
            }
        );
    }
}

public class Level5Validator : AbstractSubValidator<Level5>
{
    public Level5Validator()
    {
        Describe(x => x.DependantField).IsEqualTo("Level5");

        ScopeWhen(
            "When Level5.Check is true",
            x => Task.FromResult(x.Check == true),
            "Another DB Value",
            (x) => DataRetriever.GetMoreValue(x),
            (moreData) =>
            {
                Describe(x => x.DependantField).IsEqualTo(moreData);
            }
        );
    }
}
