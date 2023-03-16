using FluentAssertions;
using PopValidations.Scopes.ForEachs;
using PopValidations.Swashbuckle.Converters.Base;
using PopValidations.Swashbuckle.Helpers;
using PopValidations.Validations;
using PopValidations.Validations.Base;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace PopValidations.Swashbuckle_Tests;

public class ConverterCoverage_Tests
{
    private readonly ITestOutputHelper output;

    public ConverterCoverage_Tests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void EachValidatorShouldHaveASwashbuckleConverter()
    {
        // Arrange + Act
        var excludeValidations = new[]
        {
            typeof(VitallyForEachValidation),
            typeof(IsCustomValidation<>),
            typeof(IsCustomScopedValidation<>)
        };

        var AllValidators = Assembly.GetAssembly(typeof(IValidationComponent))
            ?.DefinedTypes
            .Where(x => x.GetInterfaces().Contains(typeof(IValidationComponent)))
            .Where(x => !x.IsAbstract)
            .Where(x => !excludeValidations.Contains(x))
            .Select(x => GenericNameHelper.GetNameWithoutGenericArity(x))
            .ToList();

        var AllConverters = Assembly.GetAssembly(typeof(IValidationToOpenApiConverter))
            ?.DefinedTypes
            .Where(x => x.GetInterfaces().Contains(typeof(IValidationToOpenApiConverter)))
            .Select(x => x.Name)
            .ToList();

        // Assert
        AllValidators.Should().NotBeNull();
        output.WriteLine(
            string.Join(
                Environment.NewLine, 
                AllValidators?.Where(v => !AllConverters?.Any(c => c.Contains(v)) ?? false).ToList()
                ?? new List<string>()
            )
        );
        AllValidators?.All(v => AllConverters?.Any(c => c.Contains(v)) ?? false).Should().BeTrue();
    }
}