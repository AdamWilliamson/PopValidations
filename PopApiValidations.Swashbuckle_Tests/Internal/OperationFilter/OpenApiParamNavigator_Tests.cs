using FluentAssertions;
using PopApiValidations.Swashbuckle;
using PopApiValidations.Swashbuckle.Internal.OperationFilter;
using Swashbuckle.AspNetCore.SwaggerGen;
using ApiValidations.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions.Execution;

namespace PopApiValidations.Swashbuckle_Tests.Internal.OperationFilter;

public class OpenApiParamNavigator_Tests
{
    [Fact]
    public void GivenAIntegerPropertyOnAnObject_WhenGettingParamBases_TheyAreCorrect()
    {
        // Arrange
        var methodInfo = typeof(Test_Api).GetMethod(nameof(Test_Api.QueryFunction));
        var paramName = "Integer";
        var ordinalIndicator = "[n]";
        var prefix = PopApi.Configuation.DescribeValidatingParam?.Invoke(methodInfo, 0, null);

        var operation = OpenApiOperationBuilder.CreateFromAction(
            methodInfo: methodInfo,
            schemaRepository: new SchemaRepository(),
            useReferences: true
        );
        var testingParameter = operation.Parameters.First(x => x.Name == paramName);

        var navigator = new OpenApiParamNavigator(
            operation: operation, 
            parameterInfo:methodInfo.GetParameters()[0],
            openApiParameterName: paramName,
            schemas: [testingParameter.Schema],
            parameterSchema: testingParameter,
            requestBody: null,
            parameterName: null
        );

        // Act
        var paramBases = navigator.GetParamBases(prefix, ordinalIndicator).ToList();

        // Assert
        using (new AssertionScope())
        {
            paramBases.Should().HaveCount(2);
            paramBases.Should().Contain(x => x.CurrentObjectHeirarchy == prefix + "." + paramName);
            paramBases.Should().Contain(x => x.CurrentObjectHeirarchy == prefix + "." + paramName + ordinalIndicator);
            paramBases.ForEach(x =>
            {
                x.Schemas.Should().NotBeNull();
                x.Schemas.Should().Contain(testingParameter.Schema);
                x.ObjectType.Should().Be(typeof(Request));
                x.OpenApiPropertyName.Should().StartWith(paramName);
                x.RequestBody.Should().BeNull();
                x.ParameterSchema.Should().Be(testingParameter);
            });
        }
    }

    [Fact]
    public void GivenAListOfObjects_WhenGettingParamBases_TheyAreCorrect()
    {
        // Arrange
        var methodInfo = typeof(Test_Api).GetMethod(nameof(Test_Api.QueryListFunction));
        var paramName = "Integer";
        var ordinalIndicator = "[n]";
        var prefix = PopApi.Configuation.DescribeValidatingParam?.Invoke(methodInfo, 0, null);

        var operation = OpenApiOperationBuilder.CreateFromAction(
            methodInfo: methodInfo,
            schemaRepository: new SchemaRepository(),
            useReferences: false
        );
        var testingParameter = operation.Parameters.First();

        var navigator = new OpenApiParamNavigator(
            operation: operation,
            parameterInfo: methodInfo.GetParameters()[0],
            openApiParameterName: paramName,
            schemas: [testingParameter.Schema],
            parameterSchema: testingParameter,
            requestBody: null,
            parameterName: null
        );

        // Act
        var paramBases = navigator.GetParamBases(prefix, ordinalIndicator).ToList();

        // Assert
        using (new AssertionScope())
        {
            paramBases.Should().HaveCount(2);
            paramBases.Should().Contain(x => x.OpenApiHeirarchy == prefix + "." + paramName);
            paramBases.Should().Contain(x => x.OpenApiHeirarchy == prefix + "." + paramName + ordinalIndicator);
            paramBases.ForEach(x =>
            {
                x.Schemas.Should().NotBeNull();
                x.Schemas.Should().Contain(testingParameter.Schema);
                x.ObjectType.Should().Be(typeof(Request));
                x.OpenApiPropertyName.Should().StartWith(paramName);
                x.RequestBody.Should().BeNull();
                x.ParameterSchema.Should().Be(testingParameter);
            });
        }
    }
}
