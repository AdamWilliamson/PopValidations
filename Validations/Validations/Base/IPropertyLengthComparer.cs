namespace PopValidations.Validations.Base;

public interface IPropertyLengthComparer<TPropertyType>
{
    int Compare(TPropertyType propertyValue, int comparedValue);
}
