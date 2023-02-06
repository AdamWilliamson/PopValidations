using Newtonsoft.Json;
using System;
using PopValidations.Scopes;
using System.Collections.Generic;

namespace PopValidations.Execution.Validations;

public class DescriptionGroupResult
{
    [JsonIgnore]
    public Guid Id { get; }
    [JsonIgnore]
    public IParentScope ParentScope { get; }
    public string Name = "";
    public string Description = "";
    public List<DescriptionGroupResult> Children { get; set; } = new();
    public List<DescriptionOutcome> Outcomes { get; set; } = new();

    public DescriptionGroupResult(
        IParentScope parentScope)
    {
        Id = parentScope.Id;
        ParentScope = parentScope;
        Name = parentScope.Name;
        Description = parentScope.Name;
    }
}
