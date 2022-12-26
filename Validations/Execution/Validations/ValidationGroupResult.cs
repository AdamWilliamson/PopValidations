using Newtonsoft.Json;
using System;
using PopValidations.Scopes;

namespace PopValidations.Execution.Validation;

public class ValidationGroupResult
{
    [JsonIgnore]
    public Guid Id { get; }
    [JsonIgnore]
    public IParentScope ParentScope { get; }
    public string Name = "";
    public string Description = "";
    public ValidationGroupResult? Group;

    public ValidationGroupResult(IParentScope parentScope, ValidationGroupResult? group)
    {
        Id = parentScope.Id;
        ParentScope = parentScope;
        Name = parentScope.Name;
        Description = parentScope.Name;
        Group = group;
    }
}
