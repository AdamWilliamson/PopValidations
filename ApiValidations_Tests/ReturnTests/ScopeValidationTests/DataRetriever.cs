namespace ApiValidations_Tests.ReturnTests.ScopeValidationTests;

public static class DataRetriever
{
    public static Task<bool> GetValue(Base v)
    {
        return Task.FromResult(v.ReturnValue);
    }

    public static Task<bool> GetMoreValue(Base v)
    {
        return Task.FromResult(v.ReturnValue);
    }
}
