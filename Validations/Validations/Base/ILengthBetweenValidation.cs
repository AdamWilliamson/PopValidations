namespace PopValidations.Validations.Base;

public interface ILengthBetweenValidation<TPropertyType> : IValidationComponent
{
    IPropertyLengthComparer<TPropertyType> Comparer { get; set; }
}
