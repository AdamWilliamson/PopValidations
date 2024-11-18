using ApiValidations.Execution;
using DjvuNet.Tests.Xunit;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PopApiValidations.Swashbuckle_Tests.Helpers;
using System.Reflection.Metadata;
using System.Data.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;
using FluentAssertions;
using System.Reflection;
using DiffEngine;
using static PopApiValidations.Swashbuckle_Tests.Internal.PopApiValidationSchemaFilterV3_Tests.OpenApiToMapping_Tests;
using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.OpenApiSimplification;

namespace PopApiValidations.Swashbuckle_Tests.Internal.PopApiValidationSchemaFilterV3_Tests;

public class OpenApiToMapping_Tests
{
    public record OpenApiMappingData(MethodInfo MethodInfo, string Route, string Operation, string ObjHeirarchy, string Type, bool IsArray = false)
    {
        public override string ToString()
        {
            return $"{MethodInfo.Name}, {Route}, {Operation}, {ObjHeirarchy}, {Type}, Array({IsArray})";
        }

        public static implicit operator object[](OpenApiMappingData data)
        {
            return [data];
        }
    }

    [DjvuTheory]
    [MemberData(nameof(GetControllersPropertyOutlay))]
    public void Test(OpenApiMappingData testData)
    {
        // Arrange
        var mapper = new OpenApiToSimplifier();
        var prefix = PopApi.Configuation.DescribeValidatingParam?.Invoke(testData.MethodInfo, 0, null);

        SchemaRepository schemaRepository = new();
        var operation = OpenApiOperationBuilder.CreateFromAction(
            methodInfo: testData.MethodInfo,
            schemaRepository: schemaRepository,
            useReferences: false
        );

        // Act
        var mapping = mapper.MapOpenApiOperation(operation, schemaRepository, testData.MethodInfo);
        var flatMap = GetOpenApiObjectHierarchy(mapping);

        var operationFlatMap = flatMap.Where(x => x.Operation == testData.Operation).ToList();
        var routeFlatMap = operationFlatMap.Where(x => x.Route == testData.Route).ToList();
        var typeFlatMap = routeFlatMap.Where(x => x.Type == testData.Type).ToList();
        var objFlatMap = typeFlatMap.Where(x => x.ObjHeirarchy == testData.ObjHeirarchy).ToList();
        var arrayFlatMap = objFlatMap.Where(x => x.IsArray == testData.IsArray).ToList();

        // Assert
        arrayFlatMap
            .Count()
            .Should()
            .Be(1, objFlatMap.FirstOrDefault().ToString());
    }

    public static IEnumerable<object[]> GetControllersPropertyOutlay()
    {
        MethodInfo methodInfo;
        //var methodInfo = typeof(TestController).GetMethod(nameof(TestController.Create));
        //foreach (var item in CreateForRequest(methodInfo: methodInfo, route: "api/test", method: "POST", prefix: "RequestBody."))
        //{
        //    yield return item;
        //}

        //=  new ====================================================================

        var controllerPath = "api/" + nameof(TestController).Replace("Controller", "").ToLower();

        //GetById
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.GetById));
        yield return new OpenApiMappingData(MethodInfo: methodInfo, Route: controllerPath + "/{id}", Operation: "GET", ObjHeirarchy: "id", Type: "number");

        // Create
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.Create));
        foreach (var item in CreateForRequest(methodInfo: methodInfo, route: "api/test", method: "POST", prefix: "RequestBody.", false))
        {
            yield return item;
        }

        // CreateByUrl
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.CreateByUrl));
        yield return new OpenApiMappingData(
            MethodInfo: methodInfo,
            Route: controllerPath + "/CreateByUrl/{id}/{stringField}",
            Operation: "POST",
            ObjHeirarchy: "id",
            Type: "number",
            IsArray: false);
        yield return new OpenApiMappingData(
            MethodInfo: methodInfo,
            Route: controllerPath + "/CreateByUrl/{id}/{stringField}",
            Operation: "POST",
            ObjHeirarchy: "stringField",
            Type: "string",
            IsArray: false);

        // CreateByQuery
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.CreateByQuery));
        foreach (var item in CreateForRequest(methodInfo: methodInfo, controllerPath + "/CreateByQuery", "POST", "", true))
        {
            yield return item;
        }

        //CreateByBody
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.CreateByBody));
        yield return new OpenApiMappingData(MethodInfo: methodInfo, Route: controllerPath + "/CreateByBody", Operation: "POST", ObjHeirarchy: "RequestBody", Type: "object");
        foreach (var item in CreateForRequest(methodInfo: methodInfo, controllerPath + "/CreateByBody", "POST", "RequestBody.", false))
        {
            yield return item;
        }

        // Update
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.Update));
        yield return new OpenApiMappingData(MethodInfo: methodInfo, Route: controllerPath, Operation: "PUT", ObjHeirarchy: "RequestBody", Type: "object");
        foreach (var item in CreateForRequest(methodInfo: methodInfo, controllerPath, "PUT", "RequestBody.", false))
        {
            yield return item;
        }

        // UpdateByUrl
        //yield return new TestData(Route: controllerPath + "/UpdateByUrl/{id}/{stringField}", Operation: "PUT", ObjHeirarchy: "RequestBody", Type: typeof(Request));
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.UpdateByUrl));
        yield return new OpenApiMappingData(
            MethodInfo: methodInfo,
            Route: controllerPath + "/UpdateByUrl/{id}/{stringField}",
            Operation: "PUT",
            ObjHeirarchy: "id",
            Type: "number",
            IsArray: false);
        yield return new OpenApiMappingData(
            MethodInfo: methodInfo,
            Route: controllerPath + "/UpdateByUrl/{id}/{stringField}",
            Operation: "PUT",
            ObjHeirarchy: "stringField",
            Type: "string",
            IsArray: false);

        // UpdateByQuery
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.UpdateByQuery));
        //yield return new TestData(Route: controllerPath + "/UpdateByQuery", Operation: "PUT", ObjHeirarchy: "RequestBody", Type: typeof(Request));
        foreach (var item in CreateForRequest(methodInfo: methodInfo, controllerPath + "/UpdateByQuery", "PUT", "", true))
        {
            yield return item;
        }

        //UpdateByBody
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.UpdateByBody));
        yield return new OpenApiMappingData(MethodInfo: methodInfo, Route: controllerPath + "/UpdateByBody", Operation: "PUT", ObjHeirarchy: "RequestBody", Type: "object");
        foreach (var item in CreateForRequest(methodInfo: methodInfo, controllerPath + "/UpdateByBody", "PUT", "RequestBody.", false))
        {
            yield return item;
        }

        //Delete
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.Delete));
        yield return new OpenApiMappingData(MethodInfo: methodInfo, Route: controllerPath + "/{id}", Operation: "DELETE", ObjHeirarchy: "id", Type: "number");

        //DeleteByQuery
       methodInfo = typeof(TestController).GetMethod(nameof(TestController.DeleteByQuery));
        yield return new OpenApiMappingData(
            MethodInfo: methodInfo,
            Route: controllerPath + "/DeleteByQuery",
            Operation: "DELETE",
            ObjHeirarchy: "id",
            Type: "number",
            IsArray: false);

        //DeleteByBody
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.DeleteByBody));
        yield return new OpenApiMappingData(MethodInfo: methodInfo, Route: controllerPath + "/DeleteByBody", Operation: "DELETE", ObjHeirarchy: "RequestBody", Type: "number");

        //============================================================================
    }

    public static IEnumerable<object[]> CreateForRequest(MethodInfo methodInfo, string route, string method, string prefix, bool isParameter)
    {
        if (!isParameter) yield return new OpenApiMappingData(methodInfo, route, method, prefix + "SubRequestField", "object");
        foreach (var item in CreateForAbstractComplexObject(methodInfo, route, method, prefix + "SubRequestField.", isParameter))
        {
            yield return item;
        }

        yield return new OpenApiMappingData(methodInfo, route, method, prefix + "SubRequestFieldList", "array", IsArray: true);
        foreach (var item in CreateForAbstractComplexObject(methodInfo, route, method, prefix + "SubRequestFieldList.", isParameter))
        {
            yield return item;
        }

        foreach(var item in CreateForAbstractComplexObject(methodInfo, route, method, prefix, isParameter))
        {
            yield return item;
        }
    }

    public static IEnumerable<object[]> CreateForAbstractComplexObject(MethodInfo methodInfo, string route, string method, string prefix, bool isParameter)
    {
        yield return new OpenApiMappingData(methodInfo, route, method, prefix + "IntegerField", "number");
        yield return new OpenApiMappingData(methodInfo, route, method, prefix + "StringField", "string");
        yield return new OpenApiMappingData(methodInfo, route, method, prefix + "ListOfStringsField", "array", IsArray: true);
        if (!isParameter) yield return new OpenApiMappingData(methodInfo, route, method, prefix + "DataItemField", "object");
        foreach (var item in CreateForRequestDataItem(methodInfo, route, method, prefix + "DataItemField."))
        {
            yield return item;
        }

        yield return new OpenApiMappingData(methodInfo, route, method, prefix + "ListOfRequestDataItemsField", "array", IsArray: true);
        foreach(var item in CreateForRequestDataItem(methodInfo, route, method, prefix + "ListOfRequestDataItemsField."))
        {
            yield return item;
        }
        //yield return new OpenApiMappingData(methodInfo, route, method, prefix + "ListOfRequestDataItemsField".Insert().Replace., "array");

        yield return new OpenApiMappingData(methodInfo, route, method, prefix + "DictOfStringIntField", "object");
    }

    public static IEnumerable<object[]> CreateForRequestDataItem(MethodInfo methodInfo, string route, string method, string prefix)
    {
        yield return new OpenApiMappingData(methodInfo, route, method, prefix + "Identifier", "string");
        yield return new OpenApiMappingData(methodInfo, route, method, prefix + "Value", "object");
    }


    public List<(string Route, string Operation, string ObjHeirarchy, string Type, bool IsArray)> GetOpenApiObjectHierarchy(OpenApiOperationMapping mapping)
    {
        var flatListOfProperties = new List<(string Route, string Operation, string ObjHeirarchy, string Type, bool IsArray)>();
        var route = mapping.Path;
        var operation = mapping.HttpMethod;

        foreach (var parameter in mapping.Parameters)
        {
            if (!parameter.PropertyMappings.Any() || parameter.IsArray)
            {
                flatListOfProperties.Add((
                    Route: route,
                    Operation: operation,
                    ObjHeirarchy: parameter.Name,
                    Type: parameter.Schema.Type,
                    IsArray: parameter.IsArray
                ));
            }

            foreach (var property in parameter.PropertyMappings)
            {
                flatListOfProperties.AddRange(GetPropertiesObjectHierarchy(route, operation,
                    parameter.IsArray? parameter.Name : "", property));
            }
        }

        if (mapping.RequestBody is not null)
        {
            foreach (var bodyMap in mapping.RequestBody.ContentSchemas)
            {
                flatListOfProperties.Add((
                    Route: route,
                    Operation: operation,
                    ObjHeirarchy: "RequestBody",
                    Type: bodyMap.Value.Type,
                    IsArray: bodyMap.Value.Items is not null
                ));
            }

            foreach (var property in mapping.RequestBody.PropertyMappings)
            {
                flatListOfProperties.AddRange(GetPropertiesObjectHierarchy(route, operation, "RequestBody", property));
            }
        }
        

        return flatListOfProperties;
    }

    public List<(string Route, string Operation, string ObjHeirarchy, string Type, bool IsArray)> GetPropertiesObjectHierarchy(
        string route,
        string operation,
        string prefix,
        OpenApiPropertyMapping mapping)
    {
        var flatListOfProperties = new List<(string Route, string Operation, string ObjHeirarchy, string Type, bool IsArray)>();
        prefix = string.IsNullOrWhiteSpace(prefix) ? mapping.PropertyName : prefix + "." + mapping.PropertyName;

        flatListOfProperties.Add((
            Route: route,
            Operation: operation,
            ObjHeirarchy: prefix,
            Type: mapping.PropertySchema.Type,
            IsArray: mapping.IsArray
        ));

        foreach (var property in mapping.NestedProperties)
        {
            flatListOfProperties.AddRange(GetPropertiesObjectHierarchy(route, operation, prefix, property));
        }

        return flatListOfProperties;
    }
}
