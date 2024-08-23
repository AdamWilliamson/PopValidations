namespace ApiValidations_Tests.ParamTests.ScopeValidationTests;

public static class DataRetriever
{
    public static Task<bool> GetValue(Base v)
    {
        return Task.FromResult(v?.DependantField ?? true);
    }

    public static Task<bool> GetMoreValue(Base v)
    {
        return Task.FromResult(v?.DependantField ?? true);
    }
}
