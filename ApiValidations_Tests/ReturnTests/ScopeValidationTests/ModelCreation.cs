namespace ApiValidations_Tests.ReturnTests.ScopeValidationTests;

public static class ModelCreation
{
    public static Level1 GenerateTestData()
    {
        return
            new Level1(
                true,
                dependantField: true,
                child: new(){ new Level2(
                    true,
                    dependantField: true,
                    child: new(){new Level3(
                        true,
                        dependantField: true,
                        child: new(){new Level4(
                            true,
                            dependantField: true,
                            child: new(){new Level5(
                                true,
                                dependantField: true
                            ) }
                        ) }
                    ) }
                ) }
            );
    }

    public static Level1 GenerateInvalidTestData()
    {
        return
            new Level1(
                false,
                dependantField: true,
                child: new(){ new Level2(
                    false,
                    dependantField: true,
                    child: new(){new Level3(
                        false,
                        dependantField: true,
                        child: new(){new Level4(
                            false,
                            dependantField: true,
                            child: new(){new Level5(
                                false,
                                dependantField: true
                            ) }
                        ) }
                    ) }
                ) }
            );
    }
}