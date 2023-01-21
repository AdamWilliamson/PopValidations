using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PopValidations.Execution.Description;
using PopValidations.Execution.Validations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PopValidations.Swashbuckle.Internal;

public class PopValidationSchemaFilter : ISchemaFilter
{
    private readonly IValidationRunnerFactory factory;
    private readonly OpenApiConfig config;
    private readonly ILogger<PopValidationSchemaFilter> logger;

    public PopValidationSchemaFilter(
        IValidationRunnerFactory factory,
        OpenApiConfig config,
        ILogger<PopValidationSchemaFilter> logger
    )
    {
        this.factory = factory;
        this.config = config;
        this.logger = logger;
    }

    public void Apply(OpenApiSchema model, SchemaFilterContext context)
    {
        if (model.Properties?.Any() != true)
        {
            return;
        }

        var runner = factory.GetRunner(context.Type);

        if (runner == null)
            return;

        if (model.Required == null)
        {
            logger.LogInformation("TEST: Model.Required is null");
            model.Required = new HashSet<string>();
        }

        var results = runner.Describe();

        if (!results.Results.Any())
            return;

        RunRules("", model, results.Results, context, null);
    }

    private void RunRules(
        string parent,
        OpenApiSchema model,
        List<DescriptionItemResult> fieldDescriptions,
        SchemaFilterContext context,
        string? ownedby
    )
    {
        foreach (var key in model.Properties.Keys)
        {
            var fieldName = !string.IsNullOrWhiteSpace(parent) ? parent + "." + key : key;
            if (
                fieldDescriptions.Any(
                    x => x.Property.StartsWith(fieldName, StringComparison.OrdinalIgnoreCase)
                )
            )
            {
                var fieldOutcomes = fieldDescriptions.FirstOrDefault(
                    x => x.Property.Equals(fieldName, StringComparison.OrdinalIgnoreCase)
                );
                var arrayOutcomes = fieldDescriptions.FirstOrDefault(
                    x => x.Property.Equals(fieldName + "[n]", StringComparison.OrdinalIgnoreCase)
                );

                foreach (var outcomeSet in new[] { fieldOutcomes, arrayOutcomes })
                {
                    if (outcomeSet is null)
                        continue;

                    foreach (
                        var outcome in outcomeSet?.Outcomes
                            ?? Enumerable.Empty<DescriptionOutcome>()
                    )
                    {
                        foreach (var converter in config.Converters)
                        {
                            if (converter.Supports(outcome))
                            {
                                converter.UpdateSchema(model, model.Properties[key], key, outcome);
                            }
                        }
                    }

#pragma warning disable CS8604 // Possible null reference argument.
                    CustomRules(model, model.Properties[key], key, outcomeSet, ownedby);
#pragma warning restore CS8604 // Possible null reference argument.
                }

                var newOwner = ownedby ?? "Owned By " + context.Type.Name;
                if (
                    model.Properties[key].Reference != null
                    && context.SchemaRepository.Schemas.ContainsKey(
                        model.Properties[key].Reference.Id
                    )
                )
                {
                    var childObject = context.SchemaRepository.Schemas[
                        model.Properties[key].Reference.Id
                    ];

                    RunRules(
                        fieldName,
                        childObject,
                        fieldDescriptions,
                        context,
                        newOwner + "." + model.Properties[key].Reference.Id
                    );
                }

                if (
                    model.Properties[key].Items?.Reference != null
                    && context.SchemaRepository.Schemas.ContainsKey(
                        model.Properties[key].Items.Reference.Id
                    )
                )
                {
                    var childObject = context.SchemaRepository.Schemas[
                        model.Properties[key].Items.Reference.Id
                    ];
                    
                    RunRules(
                        fieldName + "[n]",
                        childObject,
                        fieldDescriptions,
                        context,
                        newOwner + "." + model.Properties[key].Items.Reference.Id
                    );
                }
            }
        }
    }

    private void CustomRules(
        OpenApiSchema modelSchema,
        OpenApiSchema propertySchema,
        string key,
        DescriptionItemResult outcome,
        string? ownedBy
    )
    {
        OpenApiObject modelValidations = new OpenApiObject();
        if (modelSchema.Extensions.ContainsKey("x-aemo-validation"))
        {
            if (modelSchema.Extensions["x-aemo-validation"] is OpenApiObject converted)
            {
                modelValidations = converted;
            }
            else
            {
                modelSchema.Extensions["x-aemo-validation"] = modelValidations;
            }
        }
        else
        {
            modelSchema.Extensions.Add("x-aemo-validation", modelValidations);
        }

        OpenApiArray array;
        if (modelValidations.ContainsKey(key))
        {
            array = modelValidations[key] as OpenApiArray ?? new OpenApiArray();
        }
        else
        {
            array = new OpenApiArray();
        }

        modelValidations[key] = array;

        foreach (var group in outcome.ValidationGroups)
        {
            array.Add(
                new OpenApiString(
                    (ownedBy
                        + RecursiveGroupDescriber(group, string.Empty, string.Empty)).Trim()
                )
            );
        }

        foreach (var description in outcome.Outcomes)
        {
            array.Add(new OpenApiString(description.Message?.Trim()));
        }
    }

    private string RecursiveGroupDescriber(
        DescriptionGroupResult group,
        string depth,
        string existing
    )
    {
        const string indentCharacter = "\t";

        if (!string.IsNullOrWhiteSpace(group.Description))
        {
            existing += Environment.NewLine + depth + group.Description;
            depth += indentCharacter;
        }

        foreach (var parentGroup in group.Children)
        {
            existing = RecursiveGroupDescriber(parentGroup, depth, existing);
        }

        foreach (var description in group.Outcomes)
        {
            existing += Environment.NewLine + depth + description.Message;
        }

        return existing;
    }
}
