namespace ApiValidations_Tests.ReturnTests.ScopeValidationTests;

public static class DataRetriever
{
    public static Task<string> GetValue(Base v)
    {
        return Task.FromResult(v?.DependantField + " GetValue");
    }

    public static Task<bool> GetMoreValue(Base v)
    {
        return Task.FromResult(v?.DependantField ?? false);
    }
}
