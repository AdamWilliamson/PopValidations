using PopValidations.Execution;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PopValidations;

public abstract class AbstractValidatableObject<T>
    where T : class
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        var runner = validationContext.GetService(typeof(IValidationRunner<T>)) as IValidationRunner<T>;
        if (runner == null) return results;

        if (this is T converted) {
            var runnerResults = runner.Validate(converted).Result;

            foreach (var item in runnerResults.Errors)
            {
                foreach (var err in item.Value) 
                {
                    results.Add(new ValidationResult(err, new List<string>() { item.Key }));
                }
            }
        }

        return results;
    }
}