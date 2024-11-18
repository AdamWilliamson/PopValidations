using System.Reflection;
using DjvuNet.Tests.Xunit;
using FluentAssertions;
using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.MethodSimplification;
using PopApiValidations.Swashbuckle_Tests.Helpers;

namespace PopApiValidations.Swashbuckle_Tests.Internal.PopApiValidationSchemaFilterV3_Tests;
using Request = PopApiValidations.Swashbuckle_Tests.Helpers.Request;
using SubRequest = PopApiValidations.Swashbuckle_Tests.Helpers.SubRequest;

public record TestData(
    MethodInfo MethodInfo, 
    string Route, 
    string Operation, 
    string? ObjHeirarchy, 
    Type Type,
    string ResultName,
    bool IsArray = false)
{
    public override string ToString()
    {
        return $"{Route}, {Operation}, {ObjHeirarchy}, {Type.Name}, Array({IsArray})";
    }

    public static implicit operator object[](TestData data)
    {
        return [data];
    }
}

public class TypeToMapping_Tests
{
    [DjvuTheory]
    [MemberData(nameof(GetControllersPropertyOutlay))]
    public void GivenAObjectHeirarchy_AndMatchingMethodConstraints_ThenItFindsItWasGenerated(TestData data)
    {
        // Arrange
        var mapper = new MethodSimplifier();

        // Act           
        var mapping = mapper.GetMethodMap(data.MethodInfo);
        var flatMap = GetOpenApiObjectHierarchy(mapping);//new List<(string Route, string Operation, string ObjHeirarchy, Type Type)>();

        var operationFlatMap = flatMap.Where(x => x.Operation == data.Operation).ToList();
        //var routeFlatMap = operationFlatMap.Where(x => x.Route == data.Route).ToList();
        var typeFlatMap = operationFlatMap.Where(x => x.Type == data.Type).ToList();
        var objFlatMap = typeFlatMap.Where(x => x.ObjHeirarchy == data.ObjHeirarchy).ToList();
        var paramFlatMap = objFlatMap.Where(x => x.ResultName == data.ResultName);

        //Assert
        paramFlatMap
            .Count()
            .Should()
            .Be(1, objFlatMap.FirstOrDefault().ToString());
    }

    private static IEnumerable<object[]> AbstractComplexObjectFields(
        MethodInfo methodInfo, 
        string route, 
        string operation, 
        string? objPrefix,
        string? resultPrefix)
    {
        yield return new TestData(
            MethodInfo: methodInfo, 
            Route: route, 
            Operation: operation, 
            ObjHeirarchy: objPrefix + "IntegerField", 
            Type: typeof(int),
            ResultName: resultPrefix + "IntegerField"
        );
        yield return new TestData(
            MethodInfo: methodInfo, 
            Route: route, 
            Operation: operation, 
            ObjHeirarchy: objPrefix + "StringField", 
            Type: typeof(string),
            ResultName: resultPrefix + "StringField");
        yield return new TestData(
            MethodInfo: methodInfo, 
            Route: route, 
            Operation: operation, 
            ObjHeirarchy: objPrefix + "ListOfStringsField", 
            Type: typeof(string),
            ResultName: resultPrefix + "ListOfStringsField",
            IsArray: true);
        yield return new TestData(
            MethodInfo: methodInfo, 
            Route: route, 
            Operation: operation, 
            ObjHeirarchy: objPrefix + "DataItemField", 
            Type: typeof(RequestDataItem),
            ResultName: resultPrefix + "DataItemField",
            IsArray: false);
        yield return new TestData(
            MethodInfo: methodInfo, 
            Route: route, 
            Operation: operation, 
            ObjHeirarchy: objPrefix + "ListOfRequestDataItemsField", 
            Type: typeof(RequestDataItem),
            ResultName: resultPrefix + "ListOfRequestDataItemsField",
            IsArray: true);
        yield return new TestData(
            MethodInfo: methodInfo, 
            Route: route, 
            Operation: operation, 
            ObjHeirarchy: objPrefix + "ListOfRequestDataItemsField.Identifier", 
            Type: typeof(string),
            ResultName: resultPrefix + "ListOfRequestDataItemsField[n].Identifier",
            IsArray: true);
        yield return new TestData(
            MethodInfo: methodInfo, 
            Route: route, 
            Operation: operation, 
            ObjHeirarchy: objPrefix + "ListOfRequestDataItemsField.Value", 
            Type: typeof(object),
            ResultName: resultPrefix + "ListOfRequestDataItemsField[n].Value",
            IsArray: false);
        yield return new TestData(
            MethodInfo: methodInfo, 
            Route: route, 
            Operation: operation, 
            ObjHeirarchy: objPrefix + "DictOfStringIntField", 
            Type: typeof(KeyValuePair<string, int>),
            ResultName: resultPrefix + "DictOfStringIntField",
            IsArray: true);
        yield return new TestData(
            MethodInfo: methodInfo, 
            Route: route, 
            Operation: operation, 
            ObjHeirarchy: objPrefix + "DictOfStringIntField.Key",
            Type: typeof(string),
            ResultName: resultPrefix + "DictOfStringIntField[n.Key]",
            IsArray: false);
        yield return new TestData(
            MethodInfo: methodInfo, 
            Route: route, 
            Operation: operation, 
            ObjHeirarchy: objPrefix + "DictOfStringIntField.Value", 
            Type: typeof(int),
            ResultName: resultPrefix + "DictOfStringIntField[n.Value]",
            IsArray: false);
    }

    private static IEnumerable<object[]> RequestFields(
        MethodInfo methodInfo, 
        string route, 
        string operation, 
        string? objPrefix,
        string? resultPrefix
        )
    {
        foreach (var item in AbstractComplexObjectFields(methodInfo, route, operation, objPrefix, resultPrefix))
        {
            yield return item;
        }

        yield return new TestData(
            MethodInfo: methodInfo, 
            Route: route, 
            Operation: operation, 
            ObjHeirarchy: objPrefix + "SubRequestField",
            Type: typeof(SubRequest),
            ResultName: resultPrefix + "SubRequestField");
        foreach (var item in AbstractComplexObjectFields(methodInfo, route, operation, objPrefix + "SubRequestField.", resultPrefix + "SubRequestField."))
        {
            yield return item;
        }

        yield return new TestData(
            MethodInfo: methodInfo, 
            Route: route, 
            Operation: operation, 
            ObjHeirarchy: objPrefix + "SubRequestFieldList",
            Type: typeof(SubRequest),
            ResultName: resultPrefix + "SubRequestFieldList",
            IsArray: true);
        foreach (var item in AbstractComplexObjectFields(methodInfo, route, operation, objPrefix + "SubRequestFieldList.", resultPrefix + "SubRequestFieldList[n]."))
        {
            yield return item;
        }
    }

    public static IEnumerable<object[]> GetControllersPropertyOutlay()
    {
        var controllerPath = "api/" + nameof(TestController).Replace("Controller", "").ToLower();

        // GetById
        var methodInfo = typeof(TestController).GetMethod(nameof(TestController.GetById));  
        yield return new TestData(
            MethodInfo: methodInfo, 
            Route: controllerPath + "/{id}", 
            Operation: "GET", 
            ObjHeirarchy: "id", 
            Type: typeof(int?),
            ResultName: null);

        // Create
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.Create));
        yield return new TestData(
            MethodInfo: methodInfo, 
            Route: controllerPath, 
            Operation: "POST", 
            ObjHeirarchy: null, 
            Type: typeof(Request),
            ResultName: null);
        foreach(var item in RequestFields(methodInfo, controllerPath, "POST", null, null))
        {
            yield return item;
        }

        // CreateByUrl
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.CreateByUrl));
        yield return new TestData(
            MethodInfo: methodInfo,
            Route: controllerPath + "/CreateByUrl/{id}/{stringField}", 
            Operation: "POST", 
            ObjHeirarchy: "id", 
            Type: typeof(int?),
            ResultName: null,
            IsArray: false);
        yield return new TestData(
            MethodInfo: methodInfo,
            Route: controllerPath + "/CreateByUrl/{id}/{stringField}",
            Operation: "POST",
            ObjHeirarchy: "stringField",
            Type: typeof(string),
            ResultName: null,
            IsArray: false);

        // CreateByQuery
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.CreateByQuery));
        foreach (var item in RequestFields(methodInfo, controllerPath + "/CreateByQuery", "POST", "", null))
        {
            yield return item;
        }

        //CreateByBody
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.CreateByBody));
        yield return new TestData(
            MethodInfo: methodInfo, 
            Route: controllerPath + "/CreateByBody", 
            Operation: "POST", 
            ObjHeirarchy: null, 
            Type: typeof(Request),
            ResultName: null);
        foreach (var item in RequestFields(methodInfo, controllerPath + "/CreateByBody", "POST", null, null))
        {
            yield return item;
        }

        // Update
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.Update));
        yield return new TestData(
            MethodInfo: methodInfo, 
            Route: controllerPath, 
            Operation: "PUT", 
            ObjHeirarchy: null, 
            Type: typeof(Request),
            ResultName: null);
        foreach (var item in RequestFields(methodInfo, controllerPath, "PUT", null, null))
        {
            yield return item;
        }

        // UpdateByUrl
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.UpdateByUrl));
        //yield return new TestData(Route: controllerPath + "/UpdateByUrl/{id}/{stringField}", Operation: "PUT", ObjHeirarchy: "RequestBody", Type: typeof(Request));
        yield return new TestData(
            MethodInfo: methodInfo,
            Route: controllerPath + "/UpdateByUrl/{id}/{stringField}",
            Operation: "PUT",
            ObjHeirarchy: "id",
            Type: typeof(int),
            ResultName: null,
            IsArray: false);
        yield return new TestData(
            MethodInfo: methodInfo,
            Route: controllerPath + "/UpdateByUrl/{id}/{stringField}",
            Operation: "PUT",
            ObjHeirarchy: "stringField",
            Type: typeof(string),
            ResultName: null,
            IsArray: false);

        // UpdateByQuery
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.UpdateByQuery));
        //yield return new TestData(Route: controllerPath + "/UpdateByQuery", Operation: "PUT", ObjHeirarchy: "RequestBody", Type: typeof(Request));
        foreach (var item in RequestFields(methodInfo, controllerPath + "/UpdateByQuery", "PUT", null, null))
        {
            yield return item;
        }

        //UpdateByBody
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.UpdateByBody));
        yield return new TestData(
            MethodInfo: methodInfo, 
            Route: controllerPath + "/UpdateByBody", 
            Operation: "PUT", 
            ObjHeirarchy: null, 
            Type: typeof(Request),
            ResultName: null);
        foreach (var item in RequestFields(methodInfo, controllerPath + "/UpdateByBody", "PUT", null, null))
        {
            yield return item;
        }

        //Delete
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.Delete));
        yield return new TestData(
            MethodInfo: methodInfo, 
            Route: controllerPath + "/{id}", 
            Operation: "DELETE", 
            ObjHeirarchy: "id", 
            Type: typeof(int),
            ResultName: null);

        // DeleteByQuery
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.DeleteByQuery));
        yield return new TestData(
            MethodInfo: methodInfo,
            Route: controllerPath + "/DeleteByQuery",
            Operation: "DELETE",
            ObjHeirarchy: "id",
            Type: typeof(int),
            ResultName: null,
            IsArray: false);

        //DeleteByBody
        methodInfo = typeof(TestController).GetMethod(nameof(TestController.DeleteByBody));
        yield return new TestData(
            MethodInfo: methodInfo, 
            Route: controllerPath + "/DeleteByBody", 
            Operation: "DELETE", 
            ObjHeirarchy: null, 
            Type: typeof(int),
            ResultName: null);
    }


    public List<(string Route, string Operation, string ObjHeirarchy, Type Type, string? ResultName)> GetOpenApiObjectHierarchy(FunctionMapping mapping)
    {
        var flatListOfProperties = new List<(string Route, string Operation, string ObjHeirarchy, Type Type, string? ResultName)>();
        var route = mapping.OpenApiPath;
        var operation = mapping.OpenApiOperation;
        
        foreach(var parameter in  mapping.Parameters)
        {
            if (parameter.IsOpenApiRequestBody)
            {
                flatListOfProperties.Add((
                    Route: route, 
                    Operation: operation, 
                    ObjHeirarchy: null, 
                    Type: parameter.ParameterInfo.ParameterType,
                    PropertyMapping: null
                ));

                foreach (var property in parameter.Properties)
                {
                    if (parameter.IsOpenApiRequestBody)
                    {
                        flatListOfProperties.AddRange(GetPropertiesObjectHierarchy(route, operation, null, property));
                    }
                }
            }
            else
            {
                if (!parameter.Properties.Any())
                {
                    flatListOfProperties.Add((
                        Route: route,
                        Operation: operation,
                        ObjHeirarchy: parameter.OpenApiParameterName,
                        Type: parameter.ParameterInfo.ParameterType,
                        PropertyMapping: null
                    ));
                }

                foreach (var property in parameter.Properties)
                {
                    flatListOfProperties.AddRange(GetPropertiesObjectHierarchy(route, operation, "", property));
                }
            }
        }

        return flatListOfProperties;
    }

    public List<(string Route, string Operation, string ObjHeirarchy, Type Type, string? ResultName)> GetPropertiesObjectHierarchy(
        string route, 
        string operation, 
        string prefix, 
        PropertyMapping mapping)
    {
        var flatListOfProperties = new List<(string Route, string Operation, string ObjHeirarchy, Type Type, string? ResultName)>();
        prefix = string.IsNullOrWhiteSpace(prefix) ? mapping.OpenApiPropertyName : prefix + "." + mapping.OpenApiPropertyName;

        flatListOfProperties.Add((
            Route: route,
            Operation: operation,
            ObjHeirarchy: prefix,
            Type: mapping.PropertyType,
            ResultName: mapping.ResultPropertyName
        ));
        
        foreach (var property in mapping.Properties)
        {
            flatListOfProperties.AddRange(GetPropertiesObjectHierarchy(route, operation, prefix, property));
        }

        return flatListOfProperties;
    }

}
