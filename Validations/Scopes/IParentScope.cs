using System;

namespace PopValidations.Scopes;

public interface IParentScope
{
    Guid Id { get; }
    IParentScope? Parent { get; }
    string Name { get; }
}