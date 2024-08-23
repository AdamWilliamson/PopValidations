namespace ApiValidations_Tests.ParamTests.ScopeValidationTests;

public static class ModelCreation
{
    public static Level1 GenerateTestData()
    {
        return
            new Level1(
                dependantField: true,
                child: new(){ new Level2(
                    dependantField: true,
                    child: new(){new Level3(
                        dependantField: true,
                        child: new(){new Level4(
                            dependantField: true,
                            child: new(){new Level5(
                                dependantField: true
                            ) }
                        ) }
                    ) }
                ) }
            );
    }
}