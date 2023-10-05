using System.Collections.Generic;
using System.Threading.Tasks;

namespace PopValidations_Tests.ValidationsTests.ScopeWhenValidationTests.ScopeWhen_ForEachValidationTests;

public static class DataRetriever
{
    public static Task<string> GetValue(Base v)
    {
        return Task.FromResult(v?.DependantField + " GetValue");
    }

    public static Task<string> GetMoreValue(Base v)
    {
        return Task.FromResult(v?.DependantField + " GetMoreValue");
    }
}
