//using ApiValidations.Execution;
//using FluentAssertions;
//using Microsoft.OpenApi.Any;
//using Microsoft.OpenApi.Models;
//using PopApiValidations.Swashbuckle.Internal.OperationFilter;
//using Swashbuckle.AspNetCore.SwaggerGen;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace PopApiValidations.Swashbuckle_Tests.Internal.OperationFilter;

//public class OpenApiParamBasis_Tests
//{
//    public (OpenApiParamBasis, OpenApiOperation) GetSut(
//        MethodInfo methodInfo,
//        string openApiPropertyName,
//        string prefix = ""
//        )
//    {
//        //var paramName = "requests";
//        //var ordinalIndicator = "[n]";
//        //prefix = PopApi.Configuation.DescribeValidatingParam?.Invoke(methodInfo, 0, null);

//        var operation = OpenApiOperationBuilder.CreateFromAction(
//            methodInfo: methodInfo,
//            schemaRepository: new SchemaRepository(),
//            useReferences: true
//        );

//        var sut = new OpenApiParamBasis(
//            openApiPropertyName: openApiPropertyName,//"Integer",
//            openApiHeirarchy: null,
//            propertyName: null,
//            currentObjectHeirarchy: prefix,
//            objectType: typeof(int),
//            operation: operation,
//            schemas: [operation.Parameters[0].Schema],
//            parameterSchema: operation.Parameters[0],
//            null,
//            disableArray: false
//        );

//        return (sut, operation);
//    }

//    [Fact]
//    public void GivenNoneValidation_WhenGettingTheValidationArray_ItReturnsNone()
//    {
//        // Arrange
//        var attr = "x-validation";
//        var methodInfo = typeof(Test_Api).GetMethod(nameof(Test_Api.QueryListFunction));
//        var (sut, operation) = GetSut(
//            methodInfo: methodInfo,
//            openApiPropertyName: "Integer"
//            //, prefix: PopApi.Configuation.DescribeValidatingParam?.Invoke(methodInfo, 0, null)
//        );

//        // Act
//        var array = sut.GetValidationArray(PopValidations.Swashbuckle.ValidationLevel.None,attr, null);
//        array?.Add("TEST");

//        // Assert
//        sut.ParameterSchema.Extensions.ContainsKey(attr).Should().BeFalse();
//        operation.Extensions.ContainsKey(attr).Should().BeFalse();
//    }

//    [Fact]
//    public void GivenValidationAttributeInBase_WhenGettingTheValidationArray_ItReturnsOnlyOnOperation()
//    {
//        // Arrange
//        var attr = "x-validation";
//        var methodInfo = typeof(Test_Api).GetMethod(nameof(Test_Api.QueryListFunction));
//        var (sut, operation) = GetSut(
//            methodInfo: methodInfo,
//            openApiPropertyName: "Integer"
//        );

//        // Act
//        var array = sut.GetValidationArray(PopValidations.Swashbuckle.ValidationLevel.ValidationAttributeInBase, attr, null);
//        array?.Add("TEST");

//        // Assert
//        sut.ParameterSchema.Extensions.ContainsKey(attr).Should().BeFalse();
//        operation.Extensions.ContainsKey(attr).Should().BeTrue();
//        (operation.Extensions[attr]as OpenApiObject).ContainsKey("Integer").Should().BeTrue();
//    }

//    [Fact]
//    public void GivenValidationAttributeInParam_WhenGettingTheValidationArray_ItReturnsOnlyOnParameter()
//    {
//        // Arrange
//        var attr = "x-validation";
//        var methodInfo = typeof(Test_Api).GetMethod(nameof(Test_Api.QueryListFunction));
//        var (sut, operation) = GetSut(
//            methodInfo: methodInfo,
//            openApiPropertyName: "Integer"
//        );

//        // Act
//        var array = sut.GetValidationArray(PopValidations.Swashbuckle.ValidationLevel.ValidationAttribute, attr, null);
//        array?.Add("TEST");

//        // Assert
//        operation.Extensions.ContainsKey(attr).Should().BeTrue();
//        (operation.Extensions[attr] as OpenApiObject).ContainsKey("Integer").Should().BeTrue();
//        //operation.Extensions.ContainsKey(attr).Should().BeFalse();
//    }
//}
