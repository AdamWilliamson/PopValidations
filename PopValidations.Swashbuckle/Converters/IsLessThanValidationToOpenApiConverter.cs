﻿using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PopValidations.Execution.Validations;
using PopValidations.Swashbuckle.Converters.Base;
using PopValidations.Swashbuckle.Helpers;
using PopValidations.Validations;

namespace PopValidations.Swashbuckle.Converters;

public class IsLessThanValidationToOpenApiConverter : IValidationToOpenApiConverter
{
    public bool Supports(DescriptionOutcome description)
    {
        return description.Validator == GenericNameHelper
            .GetNameWithoutGenericArity(typeof(IsLessThanValidation));
    }

    public void UpdateSchema(
        OpenApiSchema owningObjectSchema,
        OpenApiSchema propertySchema,
        string property,
        DescriptionOutcome description
    )
    {
        var value = description.Values.FirstOrDefault(x => x.Key == "value").Value;
        if (decimal.TryParse(value, out var decimalValue))
        {
            propertySchema.Maximum = decimalValue;
            propertySchema.ExclusiveMinimum = true;
        }
    }

    public void UpdateAttribute(
        OpenApiSchema owningObjectSchema,
        OpenApiSchema propertySchema,
        string property,
        DescriptionOutcome description,
        OpenApiArray attributeDescription
    )
    {
        attributeDescription.Add(new OpenApiString(description.Message));
    }
}