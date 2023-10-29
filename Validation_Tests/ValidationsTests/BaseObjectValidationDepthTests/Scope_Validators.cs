using PopValidations;
using System.Threading.Tasks;

namespace PopValidations_Tests.ValidationsTests.BaseObjectValidationDepthTests;

public class Level1Validator : AbstractValidator<Level1>
{
    public Level1Validator()
    {
        Describe(x => x).IsNull();

        Scope(
            "Database Value",
            (x) => DataRetriever.GetValue(x),
            (retrievedData) =>
            {
                Describe(x => x.DependantField).IsEqualTo(retrievedData);

                Scope(
                    "Another DB Value",
                    (x) => DataRetriever.GetMoreValue(x),
                    (moreData) =>
                    {
                        Describe(x => x).IsEqualTo(moreData);
                    }
                );

                Describe(x => x.Child).SetValidator(new Level2Validator());
            }
        );
    }
}

public class Level2Validator : AbstractSubValidator<Level2>
{
    public Level2Validator()
    {
        Describe(x => x).IsNull();

        Scope(
            "Database Value",
            (x) => DataRetriever.GetValue(x),
            (retrievedData) =>
            {
                Describe(x => x).IsEqualTo(retrievedData);

                Scope(
                    "Another DB Value",
                    (x) => DataRetriever.GetMoreValue(x),
                    (moreData) =>
                    {
                        Describe(x => x).IsEqualTo(moreData);
                    }
                );

                Describe(x => x.Child).SetValidator(new Level3Validator());
            }
        );
    }
}

public class Level3Validator : AbstractSubValidator<Level3>
{
    public Level3Validator()
    {
        Describe(x => x).IsNull();

        Scope(
            "Database Value",
            (x) => DataRetriever.GetValue(x),
            (retrievedData) =>
            {
                Describe(x => x).IsEqualTo(retrievedData);

                Scope(
                    "Another DB Value",
                    (x) => DataRetriever.GetMoreValue(x),
                    (moreData) =>
                    {
                        Describe(x => x).IsEqualTo(moreData);
                    }
                );

                Describe(x => x.Child).SetValidator(new Level4Validator());
            }
        );
    }
}

public class Level4Validator : AbstractSubValidator<Level4>
{
    public Level4Validator()
    {
        Describe(x => x).IsNull();

        Scope(
            "Database Value",
            (x) => DataRetriever.GetValue(x),
            (retrievedData) =>
            {
                Describe(x => x).IsEqualTo(retrievedData);

                Scope(
                    "Another DB Value",
                    (x) => DataRetriever.GetMoreValue(x),
                    (moreData) =>
                    {
                        Describe(x => x).IsEqualTo(moreData);
                    }
                );

                Describe(x => x.Child).SetValidator(new Level5Validator());
            }
        );
    }
}

public class Level5Validator : AbstractSubValidator<Level5>
{
    public Level5Validator()
    {
        Describe(x => x).IsNull();

        Scope(
            "Another DB Value",
            (x) => DataRetriever.GetMoreValue(x),
            (moreData) =>
            {
                Describe(x => x).IsEqualTo(moreData);
            }
        );
    }
}
