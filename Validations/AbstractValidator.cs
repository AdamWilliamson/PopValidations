﻿using PopValidations.Execution.Stores;
using PopValidations.Scopes;
using PopValidations.ValidatorInternals;

namespace PopValidations;

public abstract class AbstractValidator<TValidationType> 
    : AbstractValidatorBase<TValidationType>, IMainValidator<TValidationType>
{
    protected AbstractValidator(IParentScope? parent = null) : base(parent,new ValidationConstructionStore()) {}

    ValidationConstructionStore IMainValidator<TValidationType>.Store { get { return (ValidationConstructionStore)((IStoreContainer)this).Store; } }
}