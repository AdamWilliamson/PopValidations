using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PopValidations.Execution.Description;
using PopValidations.Execution.Stores;
using PopValidations.Execution.Validation;
using PopValidations.ValidatorInternals;

namespace PopValidations.Execution;

public interface IValidationRunner<TValidationType>
{
    Task<ValidationResult> Validate(TValidationType instance);
    DescriptionResult Describe();
}

public class ValidationRunner<TValidationType> : IValidationRunner<TValidationType>
{
    private readonly IEnumerable<IMainValidator<TValidationType>> mainValidators;
    private readonly MessageProcessor messageProcessor;

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
            .GroupBy(x => new { x.PropertyName });

        var vitallyFailedFields = new List<string>();
        foreach (var validationObjectGroup in groupedItems)
        {
            foreach (var propertyGroup in validationObjectGroup)
            {
                if (vitallyFailedFields
                    .Any(f => propertyGroup.PropertyName.StartsWith(f))
                )
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
        var descriptionResult = new DescriptionResult();
        var store = new ValidationConstructionStore();
        //var executionStore = new ValidationExecutionStore();
        var allItems = new List<ExpandedItem>();

        foreach (var mainValidator in mainValidators)
        {
            var expandedItems = mainValidator.Store.Describe();//ExpandToDescribe();
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
