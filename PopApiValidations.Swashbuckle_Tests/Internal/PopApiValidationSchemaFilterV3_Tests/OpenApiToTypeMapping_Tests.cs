using ApiValidations.Execution;
using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3;
using Swashbuckle.AspNetCore.SwaggerGen;
using PopApiValidations.Swashbuckle_Tests.Helpers;
using FluentAssertions;
using System.Reflection;
using DjvuNet.Tests.Xunit;
using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.MethodSimplification;
using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.OpenApiSimplification;
using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.OpenApiToMethodMapping;

namespace PopApiValidations.Swashbuckle_Tests.Internal.PopApiValidationSchemaFilterV3_Tests;

public record TypeToOpenApiMappingTestData(MethodInfo MethodInfo, string ObjHeirarchy, string? SchemaType, bool IsParameter = true)
{ 
    public override string ToString()
    {
        return $"{MethodInfo.Name} {ObjHeirarchy}, {SchemaType}, {IsParameter}";
    }

    public static implicit operator object[](TypeToOpenApiMappingTestData data)
    {
        return [data];
    }
}

public class OpenApiToTypeMapping_Tests
{
    [DjvuTheory]
    [MemberData(nameof(GetTestData))]
    public void Test(TypeToOpenApiMappingTestData testData)
    {
        // Arrange
        var openApiMapper = new OpenApiToSimplifier();
        var typeMapper = new MethodSimplifier();
        var prefix = PopApi.Configuation.DescribeValidatingParam?.Invoke(testData.MethodInfo, 0, null);

        SchemaRepository schemaRepository = new();
        var operation = OpenApiOperationBuilder.CreateFromAction(
            methodInfo: testData.MethodInfo,
            schemaRepository: schemaRepository,
            useReferences: false
        );

        var openApiToTypeMapper = new OpenApiToTypeMapper();

        // Act
        var openApiMapping = openApiMapper.MapOpenApiOperation(operation, schemaRepository, testData.MethodInfo);
        var flatMap = openApiToTypeMapper.MapOpenApiOperationToFunction(
            openApiMapping, 
            typeMapper.GetMethodMap(openApiMapping.MethodInfo)
        );

        // Assert
        var objMap = flatMap.Where(x => 
                x.PropertySchema?.Type == testData.SchemaType 
                || x.RequestBodyContentSchemas?.Any(rbs => rbs.Type == testData.SchemaType) == true
            );
        var paramMap = objMap.Where(x => (testData.IsParameter && x.Parameter != null) || (!testData.IsParameter && x.RequestBody is not null));
        var objHeir = paramMap.Where(x => x.OpenApiObjHeirarchy == testData.ObjHeirarchy);
        var namedMap = objHeir.Where(x => x.ParameterMapping is not null || x.RequestBodyMapping is not null);

        namedMap.Should().HaveCount(1);
    }

    public static TypeToOpenApiMappingTestData TD(MethodInfo MethodInfo, string ObjHeirarchy, string? SchemaType, bool IsParameter = true)
    {
        return new TypeToOpenApiMappingTestData(
            MethodInfo: MethodInfo, 
            ObjHeirarchy: ObjHeirarchy, 
            SchemaType: SchemaType, 
            IsParameter: IsParameter
        );
    }

    public static IEnumerable<object[]> GetTestData()
    {
        //GetById
        var methodInfo = typeof(TestController).GetMethod(nameof(TestController.GetById));
        yield return TD(MethodInfo: methodInfo, ObjHeirarchy: "id", SchemaType: "number", IsParameter: true);

        // Create
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.Create));
        foreach (var item in CreateForRequest(methodInfo: methodInfo, prefix: "", IsParameter: false))
        {
            yield return item;
        }

        // CreateByUrl
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.CreateByUrl));
        yield return TD(
            MethodInfo: methodInfo,
            ObjHeirarchy: "id",
            SchemaType: "number",
            IsParameter: true);
        yield return TD(
            MethodInfo: methodInfo,
            ObjHeirarchy: "stringField",
            SchemaType: "string",
            IsParameter: true);

        // CreateByQuery
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.CreateByQuery));
        foreach (var item in CreateForRequest(methodInfo: methodInfo, prefix: "", IsParameter: true))
        {
            yield return item;
        }

        //CreateByBody
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.CreateByBody));
        yield return TD(MethodInfo: methodInfo, ObjHeirarchy: "", SchemaType: null, IsParameter: false);
        foreach (var item in CreateForRequest(methodInfo: methodInfo, "", false))
        {
            yield return item;
        }

        // Update
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.Update));
        yield return TD(MethodInfo: methodInfo, ObjHeirarchy: "", SchemaType: null, IsParameter: false);
        foreach (var item in CreateForRequest(methodInfo: methodInfo, "", false))
        {
            yield return item;
        }

        // UpdateByUrl
        //yield return new TestData(Route: controllerPath + "/UpdateByUrl/{id}/{stringField}", Operation: "PUT", ObjHeirarchy: "RequestBody", Type: typeof(Request));
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.UpdateByUrl));
        yield return TD(
            MethodInfo: methodInfo,
            ObjHeirarchy: "id",
            SchemaType: "number",
            IsParameter: true);
        yield return TD(
            MethodInfo: methodInfo,
            ObjHeirarchy: "stringField",
            SchemaType: "string",
            IsParameter: true);

        // UpdateByQuery
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.UpdateByQuery));
        //yield return new TestData(Route: controllerPath + "/UpdateByQuery", Operation: "PUT", ObjHeirarchy: "RequestBody", Type: typeof(Request));
        foreach (var item in CreateForRequest(methodInfo: methodInfo, "", true))
        {
            yield return item;
        }

        //UpdateByBody
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.UpdateByBody));
        yield return TD(MethodInfo: methodInfo, ObjHeirarchy: "", SchemaType: "object", IsParameter: false);
        foreach (var item in CreateForRequest(methodInfo: methodInfo, "", false))
        {
            yield return item;
        }

        //Delete
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.Delete));
        yield return TD(MethodInfo: methodInfo, ObjHeirarchy: "id", SchemaType: "number", IsParameter: true);

        //DeleteByQuery
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.DeleteByQuery));
        yield return TD(
            MethodInfo: methodInfo,
            ObjHeirarchy: "id",
            SchemaType: "number",
            IsParameter: true);

        //DeleteByBody
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.DeleteByBody));
        yield return TD(MethodInfo: methodInfo, ObjHeirarchy: "", SchemaType: "number", IsParameter: false);

    }

    public static IEnumerable<object[]> CreateForRequest(MethodInfo methodInfo, string prefix, bool IsParameter)
    {
        if (!IsParameter) yield return TD(MethodInfo: methodInfo, ObjHeirarchy: prefix + "SubRequestField", SchemaType: "object", IsParameter);
        foreach (var item in CreateForAbstractComplexObject(methodInfo, prefix + "SubRequestField.", IsParameter))
        {
            yield return item;
        }

        yield return TD(methodInfo, prefix + "SubRequestFieldList", "array", IsParameter: IsParameter);
        foreach (var item in CreateForAbstractComplexObject(methodInfo, prefix + "SubRequestFieldList[n].", IsParameter))
        {
            yield return item;
        }

        foreach (var item in CreateForAbstractComplexObject(methodInfo, prefix, IsParameter))
        {
            yield return item;
        }
    }

    public static IEnumerable<object[]> CreateForAbstractComplexObject(MethodInfo methodInfo, string prefix, bool IsParameter)
    {
        yield return TD(methodInfo, prefix + "IntegerField", "number", IsParameter);
        yield return TD(methodInfo, prefix + "StringField", "string", IsParameter);
        yield return TD(methodInfo, prefix + "ListOfStringsField", "array", IsParameter: IsParameter);

        if (!IsParameter) yield return TD(methodInfo, prefix + "DataItemField", "object", IsParameter);
        foreach (var item in CreateForRequestDataItem(methodInfo, prefix + "DataItemField.", IsParameter: IsParameter))
        {
            yield return item;
        }

        yield return TD(methodInfo, prefix + "ListOfRequestDataItemsField", "array", IsParameter: IsParameter);
        foreach (var item in CreateForRequestDataItem(methodInfo, prefix + "ListOfRequestDataItemsField[n].", IsParameter: IsParameter))
        {
            yield return item;
        }
        //yield return new OpenApiMappingData(methodInfo, route, method, prefix + "ListOfRequestDataItemsField".Insert().Replace., "array");

        yield return TD(methodInfo, prefix + "DictOfStringIntField", "array", IsParameter);
    }

    public static IEnumerable<object[]> CreateForRequestDataItem(MethodInfo methodInfo, string prefix, bool IsParameter)
    {
        yield return TD(methodInfo, prefix + "Identifier", "string", IsParameter: IsParameter);
        yield return TD(methodInfo, prefix + "Value", "object", IsParameter: IsParameter);
    }
}
