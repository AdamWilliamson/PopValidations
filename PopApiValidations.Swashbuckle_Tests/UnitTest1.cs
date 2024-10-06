using ApiValidations;
using ApiValidations.Descriptors;
using ApiValidations_Tests.GenericTestableObjects;
using ApprovalTests;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PopApiValidations.Swashbuckle;
using PopApiValidations.Swashbuckle_Tests.Helpers;
using PopValidations;
using PopValidations.Swashbuckle;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;

namespace PopApiValidations.Swashbuckle_Tests;
public class WebApiConfig : PopApiOpenApiConfig { }

public class Request_CreationValidation : ApiSubValidator<Request>
{
    public Request_CreationValidation()
    {
        Describe(x => x.IntegerField).Vitally().IsEqualTo(0);
    }
}

public class OpenApiNavigator(PopApiOpenApiConfig config, JObject openApi)
{
    private readonly PopApiOpenApiConfig config = config;

    public JObject OpenApi { get; } = openApi;

    public JToken? Position { get; set; } = null;


    public JObject GetPaths()
    {
        return (JObject)OpenApi["paths"];
    }

    public JObject GetDefinitions()
    {
        return (JObject)OpenApi["components"]?["schemas"];
    }

    public JObject GetOperation(string path, string method)
    {
        var paths = GetPaths();
        if (paths != null && paths[path] != null)
        {
            return (JObject)paths[path][method.ToLower()];
        }

        return null;
    }

    public OpenApiNavigator Path(string url)
    {
        Position = openApi?["paths"]?[url];
        return this;
    }

    public OpenApiNavigator Type(string type)
    {
        Position = Position[type];
        return this;
    }

    public Dictionary<string, JObject> Parameters(string url, string type)
    {
        return openApi?["paths"]?[url]?[type]["parameters"]
                ?.Where(x => x is not null && x as JObject is not null && x.Value<string>("name") is not null)
                ?.ToDictionary(x => x.Value<string>("name"), x => x as JObject)
            ?? new();
    }

    public JObject? Parameter(string url, string type, string paramName)
    {
        var parameters = Parameters(url, type);
        if (parameters.ContainsKey(paramName))
            return Parameters(url, type)[paramName];
        return null;
    }

    public List<JObject> GetRequestBodies(string url, string type)
    {
        return openApi["paths"]?[url]?[type]?["requestBody"]?["content"]?.Values().Select(x => x as JObject).ToList().ToList()
        //return openApi
        //        ["paths"]?
        //        [url]?
        //        [type]?
        //        ["requestBody"]?["content"]?.Values().Select(x => x as JObject).ToList()
                ?? new List<JObject>();
    }

    public JObject? GetRequestBodySchema(string path, string method)
    {
        var operation = GetOperation(path, method);
        if (operation != null && operation["requestBody"] != null)
        {
            var requestBody = operation["requestBody"];
            if (requestBody?["content"] != null)
            {
                // Assuming application/json for simplicity
                var content = requestBody["content"]["application/json"];
                if (content != null)
                {
                    var schemaRef = content["schema"]?["$ref"]?.ToString();
                    if (schemaRef != null)
                    {
                        return GetDefinitions()?[schemaRef.Split('/')[^1]] as JObject;
                    }
                    else if (content["schema"] != null)
                    {
                        return content["schema"] as JObject;
                    }
                }
            }
        }

        return null;
    }

    public List<JObject> GetParamSchemas(string url, string type, string paramName)
    {
        var parameter = Parameter(url, type, paramName);
        if (parameter is not null)
        {
            return new List<JObject> { parameter["schema"] as JObject };
        }

        return GetRequestBodies(url, type)
            .Where(x => x["schema"] is JObject)
            .Select(x => x["schema"] as JObject)
            .ToList();
    }

    public JObject GetParentOfParam(string url, string type, string paramName)
    {
        var parameter = Parameter(url, type, paramName);
        if (parameter is JObject convertedParam)
        {
            return convertedParam;
        }

        return openApi["paths"]?[url]?[type]?["requestBody"] as JObject; //?["content"]?.Values().Select(x => x as JObject).ToList().ToList()

        //return GetRequestBodies(url, type)
        //    .Where(x => x["schema"] is JObject)
        //    .Select(x => x["schema"] as JObject)
        //    .ToList();
    }

    public JArray GetPathValidations(string url, string type, string paramName)
    {
        var path = openApi?["paths"]?[url]?[type];

        if (path[config.CustomValidationAttribute] is null)
        {
            path[config.CustomValidationAttribute] = new JObject();
        }

        var vobj = path[config.CustomValidationAttribute] as JObject;

        if (vobj[paramName] is null)
        {
            (vobj[paramName] as JObject).Add(paramName, new JArray());
        }

        return vobj[paramName][paramName] as JArray;
    }

    public List<JObject> GetObjectParamValidations(string url, string type, string paramName)
    {
        var schemas = GetParamSchemas(url, type, paramName);

        foreach(var schema in schemas)
        {
            if (schema[config.CustomValidationAttribute] is null)
            {
                schema[config.CustomValidationAttribute] = new JObject();
            }

            //return new() { schema[config.CustomValidationAttribute] as JObject };
        }

        return schemas.Select(x => x[config.CustomValidationAttribute] as JObject).ToList();
    }

    public List<JArray> GetArrayParamValidations(string url, string type, string paramName)
    {
        var schemas = GetParamSchemas(url, type, paramName);

        foreach (var schema in schemas)
        {
            if (schema[config.CustomValidationAttribute] is null)
            {
                schema[config.CustomValidationAttribute] = new JArray();
            }

            //return new() { schema[config.CustomValidationAttribute] as JArray };
        }

        return schemas.Select(x => x[config.CustomValidationAttribute] as JArray).Where(x => x != null).ToList();
    }

    public List<JArray> GetParamValidationsArray(string url, string type, string paramName)
    {
        //var schemas = GetParamSchemas(url, type, paramName);
        var validationAttributes = new List<JArray>();

        if (Parameter(url, type, paramName) is not null) 
        {
            var pathObj = openApi?["paths"]?[url]?[type];

            if (pathObj[config.CustomValidationAttribute] is null)
            {
                (pathObj as JObject).Add(config.CustomValidationAttribute, new JObject());
            }

            if (pathObj[config.CustomValidationAttribute][paramName] is null)
            {
                (pathObj[config.CustomValidationAttribute] as JObject).Add(paramName, new JArray());
            }

            validationAttributes.Add(pathObj[config.CustomValidationAttribute][paramName] as JArray);
        }


        var requestBody = openApi["paths"]?[url]?[type]?["requestBody"];

        if (requestBody is not null)
        {
            if (openApi["paths"]?[url]?[type]?["requestBody"][config.CustomValidationAttribute] is null)
            {
                (openApi["paths"]?[url]?[type]?["requestBody"] as JObject).Add(config.CustomValidationAttribute, new JArray());
            }

            validationAttributes.Add(openApi["paths"]?[url]?[type]?["requestBody"][config.CustomValidationAttribute] as JArray);
        }

        return validationAttributes;
    }


    public JArray GetOrCreateParamValidation(string url, string type, string paramName)
    {
        var parameter = Parameter(url, type, paramName);
        if (parameter is not null)
        {
            var api = openApi?["paths"]?[url]?[type];
            if (api[config.CustomValidationAttribute] is null)
            {
                api[config.CustomValidationAttribute] = new JObject();
            }

            if (api[config.CustomValidationAttribute][paramName] is null)
            {
                api[config.CustomValidationAttribute][paramName] = new JArray();
            }

            return api[config.CustomValidationAttribute][paramName] as JArray;
        }

        if (openApi
                ["paths"]?
                [url]?
                [type]?
                ["requestBody"] is null)
        {
            openApi
                ["paths"]
                [url]
                [type]
                ["requestBody"] = new JArray();
        }

        return openApi
                ["paths"]?
                [url]?
                [type]?
                ["requestBody"] as JArray;
    }

}

public class ParamBuilder<TParamType, TParentType>
{
    private readonly ApiValidationBuilder<TParentType> parent;
    private readonly PopApiOpenApiConfig config;
    private ParamDescriptor<TParamType, TParentType> paramDescriptor;
    private readonly string paramName;
    private readonly JObject openApi;
    private readonly string url;
    private readonly string type;
    //private JToken paramOpenApi;
    //private JToken parentOpenApi;
    //private JToken operationOpenApi;
    bool isParam = true;
    OpenApiNavigator openApiNavigator;

    public ParamBuilder(
        ApiValidationBuilder<TParentType> parent,
        PopApiOpenApiConfig config,
        ParamDescriptor<TParamType, TParentType> paramDescriptor,
        string paramName,
        JObject openApi,
        string url, 
        string type)
    {
        this.parent = parent;
        this.config = config;
        this.paramDescriptor = paramDescriptor;
        this.paramName = paramName;
        this.openApi = openApi;
        this.url = url;
        this.type = type;
        openApiNavigator = new(config, openApi);

        //SchemaFor();
    }

    //private void SchemaFor()
    //{
    //    operationOpenApi = openApi?["paths"]?[url]?[type] ?? throw new Exception("Unable to find Operation");

    //    parentOpenApi = operationOpenApi?["parameters"]?.FirstOrDefault(x => x.Value<string>("name") == paramName) 
    //        ?? operationOpenApi?["requestBody"]
    //        ?? throw new Exception("Unable to find Parent");

    //    var erequestBodySchema = operationOpenApi?["requestBody"]?["content"]?["application/json"]?["schema"];
    //    var parameterSchema = operationOpenApi?["parameters"]?.FirstOrDefault(x => x.Value<string>("name") == paramName)?["schema"];
    //    var componentSchema = openApi?["components"]?[typeof(TParamType).Name];

    //    isParam = parameterSchema is not null;

    //    paramOpenApi =  parameterSchema ?? componentSchema ?? erequestBodySchema ?? throw new Exception("Unable to find Parameter");
    //}

    //public JArray? GetParamValidationAttribute()
    //{
    //    var parameter = openApi
    //        ["paths"]
    //        [url]
    //        [type
    //        ]["parameters"]?
    //        .FirstOrDefault(x => x["name"].Value<string>() == paramName);

    //    if (parameter is null)  // It a requestBody. woo. :/
    //    {
    //        return null;
    //    }

    //    if (parameter is not null && parameter?["schema"]?[config.CustomValidationAttribute] is not null)
    //    {
    //        return openApi
    //            ["paths"]
    //            [url]
    //            [type
    //            ]["parameters"]
    //            .First(x => x["name"].Value<string>() == paramName)
    //            ["schema"]
    //            [config.CustomValidationAttribute] as JArray;
    //    }

    //    var arr = new JArray();
    //    openApi
    //        ["paths"]
    //        [url]
    //        [type
    //        ]["parameters"]
    //        .First(x => x["name"].Value<string>() == paramName)
    //        ["schema"]
    //        [config.CustomValidationAttribute] = arr;

    //    return arr;
    //}

    //JObject GetApiValidationAttribute()
    //{
    //    if (openApi
    //        ["paths"]
    //        [url]
    //        [type]
    //        [config.CustomValidationAttribute] is not null)
    //    {
    //        return openApi
    //            ["paths"]
    //            [url]
    //            [type]
    //            [config.CustomValidationAttribute] as JObject;
    //    }

    //    var jobj = new JObject();
    //    openApi
    //        ["paths"]
    //        [url]
    //        [type]
    //        [config.CustomValidationAttribute] = jobj;

    //    return jobj;
    //}

    //List<JObject> GetRequestBodies()
    //{
    //    return openApi
    //            ["paths"]?
    //            [url]?
    //            [type]?
    //            ["requestBody"]?["content"]?.Values().SelectMany(x => x.Children().Cast<JObject>().ToList()).ToList()
    //            ?? new List<JObject>();
    //}

    //List<JObject> GetRequestBodyAttributes()
    //{
    //    return openApi
    //            ["paths"]?
    //            [url]?
    //            [type]?
    //            ["requestBody"]?["content"]?.Values().SelectMany(x => x.Cast<JObject>().ToList()).ToList()
    //            ?? new List<JObject>();
    //}

    //public void SetParameterValidation(string validation)
    //{
    //    //if (config.TypeValidationLevel.Invoke(typeof(TParamType)).HasFlag(ValidationLevel.ValidationAttribute)) {
    //    //    var attr = GetParamValidationAttribute();
    //    //    if (attr is null)
    //    //    {
    //    //        // request body.
    //    //        foreach(var body in GetRequestBodyAttributes())
    //    //        {
    //    //            body.Add(validation);
    //    //        }
    //    //    }
    //    //    else
    //    //    {
    //    //        attr.Add(validation);
    //    //    }
    //    //}

    //    //if (config.TypeValidationLevel.Invoke(typeof(TParamType)).HasFlag(ValidationLevel.ValidationAttributeInBase))
    //    //{
    //    //    var attr = GetApiValidationAttribute();
    //    //    if (attr.Property(paramName) is null)
    //    //    {
    //    //        attr[paramName] = new JArray();
    //    //    }

    //    //    var arr = attr[paramName] as JArray;

    //    //    arr!.Add(validation);
    //    //}
    //}

    private bool built = false;

    public ParamDescriptor<TParamType, TParentType> Build()
    {
        built = true;
        return paramDescriptor;
    }

    //public JToken GetExtension()
    //{
    //    var extension = operationOpenApi[config.CustomValidationAttribute];
    //    if (extension == null)
    //    {
    //        extension = new JObject();
    //        operationOpenApi[config.CustomValidationAttribute] = extension;//.Add(new JToken("extension"));
    //    }

    //    return extension;
    //}

    public void SetParameterValidation(string validation)
    {
        var BasicDataTypes = new[] { typeof(int), typeof(double), typeof(string), typeof(bool), typeof(DateTime), typeof(DateTimeOffset) };

        var validationLevel = config.TypeValidationLevel?.Invoke(typeof(TParamType)) ?? ValidationLevel.FullDetails;

        if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
        {
            var arrays = openApiNavigator.GetParamValidationsArray(url, type, paramName);

            foreach(var valArray in arrays)
            {
                valArray.Add(validation);
            }

            //var param = openApiNavigator.GetParamSchemas(url, type, paramName);
            //if (param.Any())
            //{
            //    //var parentValidation = openApiNavigator.GetParentOfParam(url, type, paramName);
            //    var validationObjList = openApiNavigator.GetObjectParamValidations(url, type, paramName);
            //    foreach (var loc in validationObjList)
            //    {
            //        if (loc[paramName] is null)
            //        {
            //            loc[paramName] = new JArray();
            //        }

            //        (loc[paramName] as JArray).Add(validation);
            //    }
            //    //parentValidation.Add();
            //}

            //var validationArrList = openApiNavigator.GetArrayParamValidations(url, type, paramName);
            //foreach (var loc in validationArrList)
            //{
            //    loc.Add(validation);
            //}
        }
        else if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
        {
            openApiNavigator.GetPathValidations(url, type, paramName).Add(validation);
        }




        //if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
        //{
        //    if (BasicDataTypes.Contains(typeof(TParamType))){
        //        var attr = openApiNavigator.GetArrayParamValidations(url, type, paramName);

            //        foreach(var location in attr)
            //        {
            //            location.Add(validation);
            //        }
            //    }
            //    else
            //    {
            //        var attr = openApiNavigator.GetObjectParamValidations(url, type, paramName);
            //        foreach (var location in attr)
            //        {
            //            if (location[paramName] is null)
            //            {
            //                location.Add(paramName, new JArray());
            //            }

            //            (location[paramName] as JArray).Add(validation);
            //        }
            //    }
            //}
    }

    public ParamBuilder<TParamType, TParentType> IsNotNull()
    {
        if (built) return this;

        paramDescriptor = paramDescriptor.IsNotNull();

        var parent = openApiNavigator.GetParentOfParam(url, type, paramName);
        parent["required"] = true;
        SetParameterValidation("Must not be null.");

        //var validationLevel = config.TypeValidationLevel?.Invoke(typeof(TParamType)) ?? ValidationLevel.FullDetails;
        //if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
        //{
        //    var extensions = GetExtension();
        //    extensions[paramName] = new JArray(new object[] { "Must not be null." });
        //}

        return this;
    }
}

public class ApiValidationBuilder<T>
{
    private readonly PopApiOpenApiConfig config;
    private readonly ApiValidator<T> validator;
    private readonly JObject? openApi;
    private readonly string url;
    private readonly string type;

    public ApiValidationBuilder(PopApiOpenApiConfig config, ApiValidator<T> validator, JObject? openApi, string url, string type)
    {
        this.config = config;
        this.validator = validator;
        this.openApi = openApi;
        this.url = url;
        this.type = type;

        //AddValidationAttribute();
    }

    //public JArray GetParamValidationAttribute(string url, string type, string paramName)
    //{
    //    if (openApi
    //        ["paths"]
    //        [url]
    //        [type
    //        ]["parameters"]
    //        .First(x => x["name"].Value<string>() == paramName)
    //        ["schema"]
    //        [config.CustomValidationAttribute] is not null)
    //    {
    //        return openApi
    //            ["paths"]
    //            [url]
    //            [type
    //            ]["parameters"]
    //            .First(x => x["name"].Value<string>() == paramName)
    //            ["schema"]
    //            [config.CustomValidationAttribute] as JArray;
    //    }

    //    var arr = new JArray();
    //    openApi
    //        ["paths"]
    //        [url]
    //        [type
    //        ]["parameters"]
    //        .First(x => x["name"].Value<string>() == paramName)
    //        ["schema"]
    //        [config.CustomValidationAttribute] = arr;

    //    return arr;
    //}

    //private void AddValidationAttribute()
    //{
    //    foreach (var path in openApi["paths"].Children())// as Newtonsoft.Json.Linq.JObject)?.PropertyValues( ) ?? []
    //    {
    //        var pathKey = path.Path;
    //        var pathValue = path.First;

    //        foreach(var getpostput in path.Children())
    //        {
    //            foreach (var getpostputObject in getpostput.Children())
    //            {
    //                foreach (var getpostputContent in getpostputObject.Children())
    //                {
    //                    getpostputContent[config.CustomValidationAttribute] = new JObject();

    //                    if (getpostputContent["parameters"] is not null)
    //                    {
    //                        foreach(var parameter in (getpostputContent["parameters"] as JArray))
    //                        {
    //                            parameter["schema"][config.CustomValidationAttribute] = new JArray();
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    public ParamBuilder<TParamType, T> ParamIs<TParamType>(string paramName)
    {
        return new ParamBuilder<TParamType, T>(this, config, validator.Param.Is<TParamType>(), paramName, openApi, url, type);
    }
}

class JsonUtil
{
    public static string JsonPrettify(string json)
    {
        using (var stringReader = new StringReader(json))
        using (var stringWriter = new StringWriter())
        {
            var jsonReader = new JsonTextReader(stringReader);
            var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Newtonsoft.Json.Formatting.Indented };
            jsonWriter.WriteToken(jsonReader);
            return stringWriter.ToString();
        }
    }
}

public class PopApiControllerValidationTestBuilder<TController, TValidator>
    where TController : Controller, new()
    where TValidator : ApiValidator<TController>, new()
{
    TestSetup<TController, TValidator> setup;
    TestSetup<TController, TValidator> OpenApiSetup;

    public PopApiControllerValidationTestBuilder()
    {
        setup = new TestSetup<TController, TValidator>();
        OpenApiSetup = new TestSetup<TController, TValidator>();
    }

    public async Task<(JObject Clean, JObject Swagger)> GetComparableOpenApiSpecs<TFuncOutput>(
        WebApiConfig config,
        string methodName,
        string url,
        Func<ApiValidationBuilder<TController>, Expression<Func<TController, TFuncOutput>> > expression
    )
    {
        var cleanOpenApi = await setup.GetCleanContent();
        if (cleanOpenApi == null ) { throw new Exception("Clean api json is missing"); }

        var controllerurl = typeof(TController).GetCustomAttributes(typeof(RouteAttribute)).Cast<RouteAttribute>().First().Template;
        var method = typeof(TController).GetMethod(methodName);

        var postname = method?.GetCustomAttributes(typeof(HttpPostAttribute), false).Cast<IRouteTemplateProvider>().FirstOrDefault();
        var getname = method?.GetCustomAttributes(typeof(HttpGetAttribute), false).Cast<IRouteTemplateProvider>().FirstOrDefault();
        var putname = method?.GetCustomAttributes(typeof(HttpPutAttribute), false).Cast<IRouteTemplateProvider>().FirstOrDefault();
        var patchname = method?.GetCustomAttributes(typeof(HttpPatchAttribute), false).Cast<IRouteTemplateProvider>().FirstOrDefault();
        var deletename = method?.GetCustomAttributes(typeof(HttpDeleteAttribute), false).Cast<IRouteTemplateProvider>().FirstOrDefault();
        var headname = method?.GetCustomAttributes(typeof(HttpHeadAttribute), false).Cast<IRouteTemplateProvider>().FirstOrDefault();
        var optionsname = method?.GetCustomAttributes(typeof(HttpOptionsAttribute), false).Cast<IRouteTemplateProvider>().FirstOrDefault();
        
        IRouteTemplateProvider attr = postname ?? getname ?? putname ?? patchname ?? deletename ?? optionsname ?? headname ?? throw new Exception("no type found");
        var name = attr?.Template ?? "";

        //var route = string.Join(',', controllerurl.Replace("[controller]", typeof(TController).Name.Replace("Controller", "")), name);

        string type = new Dictionary<object, string>() {
            { postname ?? new object(), "post"},
            { getname  ?? new object(), "get"},
            { putname  ?? new object(), "put"},
            { patchname  ?? new object(), "patch"},
            { deletename  ?? new object(), "delete"},
            { optionsname ?? new object(), "options"},
            { headname  ?? new object(), "head"}
        }[attr];


        // Act
        var helper = await OpenApiSetup.GetHelper(config, cleanOpenApi, url, type, expression);

        if (helper.Content is null) throw new Exception("validation effected json is missing");

        return (cleanOpenApi, JObject.Parse(helper.Content));
    }
}


public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        // Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        //var validation = new TestControllerValidation();

        //var setup = new TestSetup<
        //        TestController,
        //        TestControllerValidation
        //    >(validation);
        //var config = new WebApiConfig();

        //var cleanOpenApi = await setup.GetCleanContent();

        //var controllerurl = typeof(TestController).GetCustomAttributes(typeof(RouteAttribute)).Cast<RouteAttribute>().First().Template;
        //var name = typeof(TestController).GetMethod(nameof(TestController.Create))?.GetCustomAttributes(typeof(HttpPostAttribute), false).Cast<HttpPostAttribute>().First().Name;

        //var route = string.Join(',', controllerurl.Replace("[controller]", typeof(TestController).Name.Replace("Controller","")), name);

        //var type = "post";


        //var testingBuilder = new ApiValidationBuilder<TestController>(
        //    validation,
        //    cleanOpenApi,
        //    "/api/Test", "post"
        //) ;
        //validation.DescribeFunc(x => x.Create(testingBuilder.ParamIs<Request>("request").IsNotNull().Build()));

        // Act
        //var helper = await setup.GetHelper(config);

        var result = await controllerTester.GetComparableOpenApiSpecs<ActionResult<Response>>(
            new WebApiConfig(),
            nameof(TestController.Create),
            "/api/Test",
            (b) => (x) => x.Create(b.ParamIs<Request>("request").IsNotNull().Build())
            );

        // Assert
        //Approvals.AssertEquals(cleanOpenApi.ToString(Newtonsoft.Json.Formatting.Indented), JsonUtil.JsonPrettify(helper.Content));
        Approvals.AssertEquals(result.Clean.ToString(Newtonsoft.Json.Formatting.Indented), result.Swagger.ToString(Newtonsoft.Json.Formatting.Indented));
    }

    [Fact]
    public async Task OpenApiNavigatorTests()
    {
        // Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new WebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(nameof(TestController.Create));

        var result = await controllerTester.GetComparableOpenApiSpecs<ActionResult<Response>>(
            config,
            nameof(TestController.Create),
            "/api/Test",
            (b) => (x) => x.Create(b.ParamIs<Request>("request").Build())
            );

        var navigator = new OpenApiNavigator(config, result.Clean);

        var GetByIdParamSchema = navigator.GetParamSchemas("/api/Test/{id}", "get", "id");
        GetByIdParamSchema.Should().HaveCount(1);
        GetByIdParamSchema.First()["type"].Value<string>().Should().Be("integer");

        var CreateByUrl = navigator.GetParamSchemas("/api/Test/CreateByUrl/{id}/{stringField}/{listOfIntField}", "post", "listOfIntField");
        CreateByUrl.Should().HaveCount(1);
        CreateByUrl.Select(x => x["type"].Value<string>()).Should().AllBe("string");

        var CreateByBody = navigator.GetParamSchemas("/api/Test/CreateByBody", "post", "request");
        CreateByBody.Should().HaveCount(3);
        CreateByBody.Select(x => x["type"].Value<string>()).Should().AllBe("object");

        var CreateByBodyParams = navigator.GetParamSchemas("/api/Test/CreateByBody", "post", "request");
        CreateByBodyParams.Should().HaveCount(3);
        CreateByBodyParams.Select(x => x["type"].Value<string>()).Should().AllBe("object");

        var CreateByBodyParent = navigator.GetParentOfParam("/api/Test/CreateByBody", "post", "request");
        CreateByBodyParent["content"].Should().NotBeNull();

        var GetByIdParent = navigator.GetParentOfParam("/api/Test/{id}", "get", "id");
        GetByIdParent["parameters"].Should().NotBeNull();
    }

    [Fact]
    public async Task Create_Basic_Validation()
    {
        // Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new WebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(nameof(TestController.Create));

        // Act
        var result = await controllerTester.GetComparableOpenApiSpecs<ActionResult<Response>>(
            config,
            nameof(TestController.Create),
            "/api/Test",
            (b) => (x) => x.Create(b.ParamIs<Request>("request").IsNotNull().Build())
        );

        //Assert
        Approvals.AssertEquals(result.Clean.ToString(Formatting.Indented), result.Swagger.ToString(Formatting.Indented));
    }

    [Fact]
    public async Task GetById_Basic_Validation()
    {
        // Arrange
        string methodName = nameof(TestController.GetById);
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new WebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(methodName);

        // Act
        var result = await controllerTester.GetComparableOpenApiSpecs<ActionResult<Response>>(
            config,
            methodName,
            "/api/Test/{id}",
            (b) => (x) => x.GetById(b.ParamIs<int>("id").IsNotNull().Build())
        );

        //Assert
        Approvals.AssertEquals(result.Clean.ToString(Formatting.Indented), result.Swagger.ToString(Formatting.Indented));
    }
}