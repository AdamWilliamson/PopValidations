using ApiValidations;
using PopValidations;

namespace ApiValidations_Tests.ReturnTests.ScopeValidationTests;

public class Level1Validator : ApiValidator<Level1>
{
    public Level1Validator()
    {
        DescribeFunc(x => x.Check()).Return.IsEqualTo(true);

        Scope(
            "Database Value",
            (x) => DataRetriever.GetValue(x),
            (retrievedData) =>
            {
                DescribeFunc(x => x.Check()).Return.IsEqualTo(retrievedData);

                Scope(
                    "Another DB Value",
                    (x) => DataRetriever.GetMoreValue(x),
                    (moreData) =>
                    {
                        DescribeFunc(x => x.Check()).Return.IsEqualTo(moreData);
                    }
                );

                DescribeEnumerable(x => x.Child)
                    .ForEach(x => x.SetValidator(new Level2Validator()));
            }
        );
    }
}

public class Level2Validator : ApiSubValidator<Level2>
{
    public Level2Validator()
    {
        DescribeFunc(x => x.Check()).Return.IsEqualTo(true);

        Scope(
            "Database Value",
            (x) => DataRetriever.GetValue(x),
            (retrievedData) =>
            {
                DescribeFunc(x => x.Check()).Return.IsEqualTo(retrievedData);

                Scope(
                    "Another DB Value",
                    (x) => DataRetriever.GetMoreValue(x),
                    (moreData) =>
                    {
                        DescribeFunc(x => x.Check()).Return.IsEqualTo(moreData);
                    }
                );

                DescribeEnumerable(x => x.Child)
                    .ForEach(x => x.SetValidator(new Level3Validator()));
            }
        );
    }
}

public class Level3Validator : ApiSubValidator<Level3>
{
    public Level3Validator()
    {
        DescribeFunc(x => x.Check()).Return.IsEqualTo(true);

        Scope(
            "Database Value",
            (x) => DataRetriever.GetValue(x),
            (retrievedData) =>
            {
                DescribeFunc(x => x.Check()).Return.IsEqualTo(retrievedData);

                Scope(
                    "Another DB Value",
                    (x) => DataRetriever.GetMoreValue(x),
                    (moreData) =>
                    {
                        DescribeFunc(x => x.Check()).Return.IsEqualTo(moreData);
                    }
                );

                DescribeEnumerable(x => x.Child)
                    .ForEach(x => x.SetValidator(new Level4Validator()));
            }
        );
    }
}

public class Level4Validator : ApiSubValidator<Level4>
{
    public Level4Validator()
    {
        DescribeFunc(x => x.Check()).Return.IsEqualTo(true);

        Scope(
            "Database Value",
            (x) => DataRetriever.GetValue(x),
            (retrievedData) =>
            {
                DescribeFunc(x => x.Check()).Return.IsEqualTo(retrievedData);

                Scope(
                    "Another DB Value",
                    (x) => DataRetriever.GetMoreValue(x),
                    (moreData) =>
                    {
                        DescribeFunc(x => x.Check()).Return.IsEqualTo(moreData);
                    }
                );

                DescribeEnumerable(x => x.Child)
                    .ForEach(x => x.SetValidator(new Level5Validator()));
            }
        );
    }
}

public class Level5Validator : ApiSubValidator<Level5>
{
    public Level5Validator()
    {
        DescribeFunc(x => x.Check()).Return.IsEqualTo(true);

        Scope(
            "Another DB Value",
            (x) => DataRetriever.GetMoreValue(x),
            (moreData) =>
            {
                DescribeFunc(x => x.Check()).Return.IsEqualTo(moreData);
            }
        );
    }
}
