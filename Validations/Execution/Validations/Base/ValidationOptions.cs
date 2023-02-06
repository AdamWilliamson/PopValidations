using PopValidations.Validations.Base;

namespace PopValidations.Execution.Validations.Base;

public abstract class ValidationOptions<TCustomValidation>
    where TCustomValidation : IValidationComponent
{
    protected TCustomValidation Component { get; private set; }

    public ValidationOptions(TCustomValidation validationComponent)
    {
        this.Component = validationComponent;
    }

    public virtual ValidationOptions<TCustomValidation> WithDescription(string desc)
    {
        Component.SetDescriptionTemplate(desc);
        return this;
    }

    public virtual ValidationOptions<TCustomValidation> WithErrorMessage(string error)
    {
        Component.SetErrorTemplate(error);
        return this;
    }
}

public class ValidationOptions : ValidationOptions<IValidationComponent>
{
    public ValidationOptions(IValidationComponent validationComponent)
        : base(validationComponent)
    { }
}