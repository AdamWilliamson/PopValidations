using DjvuNet.Tests.Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using PopApiValidations.Swashbuckle.Internal.OperationFilter;
using PopValidations.Execution.Description;
using PopValidations.Execution.Validations;
using PopValidations.Validations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PopApiValidations.Swashbuckle_Tests.Internal.OperationFilter;

public class ValidationProcessorTestData
{
    public MethodInfo MethodInfo { get; set; }
    public int ParameterIndex { get; set; }
    public string OpenApiPropertyName { get; set; }
    public Type ParameterType => MethodInfo.GetParameters()[ParameterIndex].ParameterType;
    public List<DescriptionOutcome> DescriptionOutcomes { get; set; }
    public List<DescriptionItemResult> GetDescriptionResults(string prefix)
    {
        var validation = new DescriptionItemResult(prefix + "." + OpenApiPropertyName);
        DescriptionOutcomes.ForEach(d => validation.Outcomes.Add(d));
        //validation.Outcomes.Add(
        //    new PopValidations.Execution.Validations.DescriptionOutcome(nameof(IsNotNullValidation), "Must not be null.", new())
        //);
        return new() { validation };
    }

    public static implicit operator object?[](ValidationProcessorTestData d) => new object?[] { d };

    public new string ToString()
    {
        return $"{MethodInfo.Name} - {ParameterIndex} - OpenApi({OpenApiPropertyName})";
    }
}

public class ValidationToSchemaProcessor_Tests
{
    public static IEnumerable<object[]> ParameterTestData()
    {
        yield return new ValidationProcessorTestData
        {
            MethodInfo = typeof(Test_Api).GetMethod(nameof(Test_Api.BasicRouteFunction)),
            ParameterIndex = 0,
            OpenApiPropertyName = "id",
            DescriptionOutcomes = new()
            {
                new DescriptionOutcome(nameof(IsNotNullValidation), "Must not be null.", new())
            }
        };
    }

    public static IEnumerable<object[]> RequestBodyTestData()
    {
        yield return new ValidationProcessorTestData
        {
            MethodInfo = typeof(Test_Api).GetMethod(nameof(Test_Api.PostFunction)),
            ParameterIndex = 0,
            OpenApiPropertyName = null,
            DescriptionOutcomes = new()
            {
                new DescriptionOutcome(nameof(IsNotNullValidation), "Must not be null.", new())
            }
        };
    }

    private (
        OpenApiOperation Operation, 
        OpenApiParamBasis Parameter, 
        List<DescriptionItemResult> DescriptionResults,
        BaseData BaseData
        ) GetSUT(ValidationProcessorTestData testData)
    {
        var desc = ApiValidations.Execution.PopApiValidations.Configuation.DescribeValidatingParam.Invoke(
            testData.MethodInfo, Math.Max(testData.ParameterIndex, 0), null
        );

        var config = new TestWebApiConfig();
        var controllerTester = new PopApiControllerValidationTestBuilder<Test_Api, Test_ApiValidation>();
        var schemaRepository = new SchemaRepository();

        var operation = OpenApiOperationBuilder.CreateFromAction(
            methodInfo: testData.MethodInfo,
            schemaRepository: schemaRepository,
            useReferences: true
        );

        var validationResults = testData.GetDescriptionResults(desc);

        if (testData.OpenApiPropertyName is not null)
        {
            var testingParameter = operation.Parameters.First(x => x.Name == testData.OpenApiPropertyName);

            var baseData = new BaseData(config, operation, schemaRepository, testData.MethodInfo, validationResults);

            var openApiParamBasis = new OpenApiParamBasis(
                openApiPropertyName: testData.OpenApiPropertyName,
                currentObjectHeirarchy: desc + "." + testData.OpenApiPropertyName,
                objectType: testData.ParameterType,
                schemas: [testingParameter.Schema],
                parameterSchema: testingParameter,
                requestBody: null
            );

            return (operation, openApiParamBasis, validationResults, baseData);
        }
        else
        {
            var testingParameter = operation.RequestBody;

            var baseData = new BaseData(config, operation, schemaRepository, testData.MethodInfo, validationResults);

            var openApiParamBasis = new OpenApiParamBasis(
                openApiPropertyName: testData.OpenApiPropertyName,
                currentObjectHeirarchy: desc + "." + testData.OpenApiPropertyName,
                objectType: testData.ParameterType,
                schemas: operation.RequestBody.Content.Values.Select(x => x.Schema).ToArray(),//[testingParameter.Schema],
                parameterSchema: null,
                requestBody: operation.RequestBody
            );

            return (operation, openApiParamBasis, validationResults, baseData);
        }
    }

    [DjvuTheory]
    [MemberData(nameof(ParameterTestData))]
    public void Test(ValidationProcessorTestData testData)
    {
        // Arrange
        var config = new TestWebApiConfig();

        var (operation, openApiParamBasis, validationResults, baseData) = GetSUT(testData);

        var validationForSchema = ValidationProcessor.GetFlattenedValidationsFor(config, validationResults, openApiParamBasis.CurrentObjectHeirarchy);
        
        // Act
        ValidationToSchemaProcessor.ProcessOpenApiParameter(openApiParamBasis, validationForSchema, baseData);

        // Assert
        operation.Extensions[config.CustomValidationAttribute].Should().NotBeNull();
        GetValidationArray(operation.Extensions, config.CustomValidationAttribute, testData.OpenApiPropertyName)
            .Contains("Must not be null.")
            .Should().BeTrue();
        openApiParamBasis.ParameterSchema.Required.Should().BeTrue();
    }

    [DjvuTheory]
    [MemberData(nameof(RequestBodyTestData))]
    public void RequestBodyTests(ValidationProcessorTestData testData)
    {
        // Arrange
        var config = new TestWebApiConfig();

        var (operation, openApiParamBasis, validationResults, baseData) = GetSUT(testData);

        var validationForSchema = ValidationProcessor.GetFlattenedValidationsFor(config, validationResults, openApiParamBasis.CurrentObjectHeirarchy);

        // Act
        ValidationToSchemaProcessor.ProcessOpenApiParameter(openApiParamBasis, validationForSchema, baseData);

        // Assert
        operation.Extensions[config.CustomValidationAttribute].Should().NotBeNull();
        GetValidationArray(operation.Extensions, config.CustomValidationAttribute, "RequestBody")
            .Contains("Must not be null.")
            .Should().BeTrue();
        openApiParamBasis.RequestBody.Required.Should().BeTrue();
    }

    private List<string> GetValidationArray(IDictionary<string, IOpenApiExtension> extensions, string attributeName, string propertyName)
    {
        return ((extensions[attributeName] as OpenApiObject)[propertyName] as OpenApiArray)
            .Cast<OpenApiString>()
            .Select(s => s.Value)
            .ToList();
    }
}
