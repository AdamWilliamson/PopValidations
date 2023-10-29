using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PopValidations.Execution.Description;
using PopValidations.Execution.Stores;
using PopValidations.Execution.Validation;
using PopValidations.ValidatorInternals;

namespace PopValidations.Execution;

public interface IValidationDescriber
{
    DescriptionResult Describe();
}

public interface IValidationRunner<TValidationType>: IValidationDescriber
{
    Task<ValidationResult> Validate(TValidationType instance);
}

public class ValidationRunner<TValidationType> : IValidationRunner<TValidationType>, IValidationDescriber
{
    private readonly IEnumerable<IMainValidator<TValidationType>> mainValidators;
    private readonly MessageProcessor messageProcessor;
    DescriptionResult? descriptionResult = null;

    public ValidationRunner(
        IEnumerable<IMainValidator<TValidationType>> mainValidators,
        MessageProcessor messageProcessor
        )
    {
        this.mainValidators = mainValidators;
        this.messageProcessor = messageProcessor;
    }

    public async Task<ValidationResult> Validate(TValidationType instance)
    {
        var validationResult = new ValidationResult();
        var store = new ValidationConstructionStore();
        var allItems = new List<ExpandedItem>();

        foreach (var mainValidator in mainValidators)
        {
            var expandedItems = mainValidator.Store.Compile(instance);
            if (expandedItems?.Any() == true)
            {
                allItems.AddRange(expandedItems);
            }
        }

        var groupedItems = allItems
            .GroupBy(x => new { x.PropertyName })
            .OrderBy(x => x.Key.PropertyName);

        var vitallyFailedFields = new List<string>();
        
        foreach (var validationObjectGroup in groupedItems)
        {
            Debug.WriteLine(validationObjectGroup.Key.PropertyName);
            foreach (var propertyGroup in validationObjectGroup)
            {
                Debug.WriteLine("--" + propertyGroup.FullAddressableName);
                if (vitallyFailedFields.Any(f => propertyGroup.PropertyName.StartsWith(f)))
                {
                    continue;
                }
                
                if (await propertyGroup.CanValidate(instance)) 
                {
                    var result = propertyGroup.Validate(instance);

                    if (!result.Success)
                    {
                        messageProcessor.ProcessErrorMessage(result);

                        validationResult.AddItem(propertyGroup, result);

                        if (propertyGroup.IsVital)
                        {
                            vitallyFailedFields.Add(propertyGroup.PropertyName);
                            break;
                        }
                    }
                    var failedFields = result.GetFailedDependantFields(propertyGroup.PropertyName, validationResult);
                    if (failedFields?.Any() == true)
                    {
                        if (propertyGroup.IsVital)
                        {
                            vitallyFailedFields.AddRange(failedFields);
                            break;
                        }
                    }
                }
            }
        }

        return validationResult;
    }

    public DescriptionResult Describe() 
    {
        if (descriptionResult != null) return descriptionResult;
        if (mainValidators == null) return new();

        descriptionResult = new();
        var store = new ValidationConstructionStore();
        var allItems = new List<ExpandedItem>();

        foreach (var mainValidator in mainValidators)
        {
            var expandedItems = mainValidator.Store.Describe();
            if (expandedItems?.Any() == true)
            {
                allItems.AddRange(expandedItems);
            }
        }

        var groupedItems = allItems
            .GroupBy(x => new { x.PropertyName });

        foreach (var validationObjectGroup in groupedItems)
        {
            foreach (var propertyGroup in validationObjectGroup)
            {
                var result = propertyGroup.Describe();
                messageProcessor.ProcessDescriptionMessage(result);
                descriptionResult.AddItem(propertyGroup, result);
            }
        }

        return descriptionResult;
    }
}
