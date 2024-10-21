using ApiValidations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using PopApiValidations.Swashbuckle_Tests.Helpers;

namespace PopApiValidations.Swashbuckle_Tests;

//public class ParamBuilder<TParamType, TParentType>
//{
//    private readonly PopApiOpenApiConfig config;
//    private ParamDescriptor<TParamType, TParentType> paramDescriptor;
//    private readonly string paramName;
//    private readonly string url;
//    private readonly string type;
//    //private JToken paramOpenApi;
//    //private JToken parentOpenApi;
//    //private JToken operationOpenApi;
//    OpenApiNavigator openApiNavigator;

//    public ParamBuilder(
//        PopApiOpenApiConfig config,
//        ParamDescriptor<TParamType, TParentType> paramDescriptor,
//        OpenApiNavigator openApiNavigator,
//        string paramName,
//        string url, 
//        string type)
//    {
//        this.config = config;
//        this.paramDescriptor = paramDescriptor;
//        this.paramName = paramName;
//        this.url = url;
//        this.type = type;
//        this.openApiNavigator = openApiNavigator;

//        //SchemaFor();
//    }

//    //private void SchemaFor()
//    //{
//    //    operationOpenApi = openApi?["paths"]?[url]?[type] ?? throw new Exception("Unable to find Operation");

//    //    parentOpenApi = operationOpenApi?["parameters"]?.FirstOrDefault(x => x.Value<string>("name") == paramName) 
//    //        ?? operationOpenApi?["requestBody"]
//    //        ?? throw new Exception("Unable to find Parent");

//    //    var erequestBodySchema = operationOpenApi?["requestBody"]?["content"]?["application/json"]?["schema"];
//    //    var parameterSchema = operationOpenApi?["parameters"]?.FirstOrDefault(x => x.Value<string>("name") == paramName)?["schema"];
//    //    var componentSchema = openApi?["components"]?[typeof(TParamType).Name];

//    //    isParam = parameterSchema is not null;

//    //    paramOpenApi =  parameterSchema ?? componentSchema ?? erequestBodySchema ?? throw new Exception("Unable to find Parameter");
//    //}

//    //public JArray? GetParamValidationAttribute()
//    //{
//    //    var parameter = openApi
//    //        ["paths"]
//    //        [url]
//    //        [type
//    //        ]["parameters"]?
//    //        .FirstOrDefault(x => x["name"].Value<string>() == paramName);

//    //    if (parameter is null)  // It a requestBody. woo. :/
//    //    {
//    //        return null;
//    //    }

//    //    if (parameter is not null && parameter?["schema"]?[config.CustomValidationAttribute] is not null)
//    //    {
//    //        return openApi
//    //            ["paths"]
//    //            [url]
//    //            [type
//    //            ]["parameters"]
//    //            .First(x => x["name"].Value<string>() == paramName)
//    //            ["schema"]
//    //            [config.CustomValidationAttribute] as JArray;
//    //    }

//    //    var arr = new JArray();
//    //    openApi
//    //        ["paths"]
//    //        [url]
//    //        [type
//    //        ]["parameters"]
//    //        .First(x => x["name"].Value<string>() == paramName)
//    //        ["schema"]
//    //        [config.CustomValidationAttribute] = arr;

//    //    return arr;
//    //}

//    //JObject GetApiValidationAttribute()
//    //{
//    //    if (openApi
//    //        ["paths"]
//    //        [url]
//    //        [type]
//    //        [config.CustomValidationAttribute] is not null)
//    //    {
//    //        return openApi
//    //            ["paths"]
//    //            [url]
//    //            [type]
//    //            [config.CustomValidationAttribute] as JObject;
//    //    }

//    //    var jobj = new JObject();
//    //    openApi
//    //        ["paths"]
//    //        [url]
//    //        [type]
//    //        [config.CustomValidationAttribute] = jobj;

//    //    return jobj;
//    //}

//    //List<JObject> GetRequestBodies()
//    //{
//    //    return openApi
//    //            ["paths"]?
//    //            [url]?
//    //            [type]?
//    //            ["requestBody"]?["content"]?.Values().SelectMany(x => x.Children().Cast<JObject>().ToList()).ToList()
//    //            ?? new List<JObject>();
//    //}

//    //List<JObject> GetRequestBodyAttributes()
//    //{
//    //    return openApi
//    //            ["paths"]?
//    //            [url]?
//    //            [type]?
//    //            ["requestBody"]?["content"]?.Values().SelectMany(x => x.Cast<JObject>().ToList()).ToList()
//    //            ?? new List<JObject>();
//    //}

//    //public void SetParameterValidation(string validation)
//    //{
//    //    //if (config.TypeValidationLevel.Invoke(typeof(TParamType)).HasFlag(ValidationLevel.ValidationAttribute)) {
//    //    //    var attr = GetParamValidationAttribute();
//    //    //    if (attr is null)
//    //    //    {
//    //    //        // request body.
//    //    //        foreach(var body in GetRequestBodyAttributes())
//    //    //        {
//    //    //            body.Add(validation);
//    //    //        }
//    //    //    }
//    //    //    else
//    //    //    {
//    //    //        attr.Add(validation);
//    //    //    }
//    //    //}

//    //    //if (config.TypeValidationLevel.Invoke(typeof(TParamType)).HasFlag(ValidationLevel.ValidationAttributeInBase))
//    //    //{
//    //    //    var attr = GetApiValidationAttribute();
//    //    //    if (attr.Property(paramName) is null)
//    //    //    {
//    //    //        attr[paramName] = new JArray();
//    //    //    }

//    //    //    var arr = attr[paramName] as JArray;

//    //    //    arr!.Add(validation);
//    //    //}
//    //}

//    private bool built = false;

//    public ParamDescriptor<TParamType, TParentType> Build()
//    {
//        built = true;
//        return paramDescriptor;
//    }

//    //public JToken GetExtension()
//    //{
//    //    var extension = operationOpenApi[config.CustomValidationAttribute];
//    //    if (extension == null)
//    //    {
//    //        extension = new JObject();
//    //        operationOpenApi[config.CustomValidationAttribute] = extension;//.Add(new JToken("extension"));
//    //    }

//    //    return extension;
//    //}

//    public void SetParameterValidation(string validation)
//    {
//        var BasicDataTypes = new[] { typeof(int), typeof(double), typeof(string), typeof(bool), typeof(DateTime), typeof(DateTimeOffset) };

//        var validationLevel = config.TypeValidationLevel?.Invoke(typeof(TParamType)) ?? ValidationLevel.FullDetails;

//        if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
//        {
//            var arrays = openApiNavigator.GetParamValidationsArray(url, type, paramName);

//            foreach(var valArray in arrays)
//            {
//                valArray.Add(validation);
//            }

//            //var param = openApiNavigator.GetParamSchemas(url, type, paramName);
//            //if (param.Any())
//            //{
//            //    //var parentValidation = openApiNavigator.GetParentOfParam(url, type, paramName);
//            //    var validationObjList = openApiNavigator.GetObjectParamValidations(url, type, paramName);
//            //    foreach (var loc in validationObjList)
//            //    {
//            //        if (loc[paramName] is null)
//            //        {
//            //            loc[paramName] = new JArray();
//            //        }

//            //        (loc[paramName] as JArray).Add(validation);
//            //    }
//            //    //parentValidation.Add();
//            //}

//            //var validationArrList = openApiNavigator.GetArrayParamValidations(url, type, paramName);
//            //foreach (var loc in validationArrList)
//            //{
//            //    loc.Add(validation);
//            //}
//        }
//        else if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
//        {
//            openApiNavigator.GetPathValidations(url, type, paramName).Add(validation);
//        }




//        //if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
//        //{
//        //    if (BasicDataTypes.Contains(typeof(TParamType))){
//        //        var attr = openApiNavigator.GetArrayParamValidations(url, type, paramName);

//            //        foreach(var location in attr)
//            //        {
//            //            location.Add(validation);
//            //        }
//            //    }
//            //    else
//            //    {
//            //        var attr = openApiNavigator.GetObjectParamValidations(url, type, paramName);
//            //        foreach (var location in attr)
//            //        {
//            //            if (location[paramName] is null)
//            //            {
//            //                location.Add(paramName, new JArray());
//            //            }

//            //            (location[paramName] as JArray).Add(validation);
//            //        }
//            //    }
//            //}
//    }

//    public ParamBuilder<TParamType, TParentType> IsNotNull()
//    {
//        if (built) return this;

//        paramDescriptor = paramDescriptor.IsNotNull();

//        var parent = openApiNavigator.GetParentOfParam(url, type, paramName);
//        if (parent != null) parent["required"] = true;
//        SetParameterValidation("Must not be null.");

//        //var validationLevel = config.TypeValidationLevel?.Invoke(typeof(TParamType)) ?? ValidationLevel.FullDetails;
//        //if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
//        //{
//        //    var extensions = GetExtension();
//        //    extensions[paramName] = new JArray(new object[] { "Must not be null." });
//        //}

//        return this;
//    }

//    //public ParamBuilder<TParamType, TParentType> Child<TChildType>(
//    //string prop,
//    //    Expression<Func<TParamType, TChildType>> expr,
//    //    Func<ParamBuilder<TChildType, TParamType>, TChildType> func)
//    //{
//    //    var childValidator = new TestSubValidation<TParamType>();

//    //    func.Invoke(
//    //        new ParamBuilder<TChildType, TParamType>(
//    //            config,
//    //            childValidator.Describe(expr),
//    //            //childValidator.Param.Is<TChildType>(),
//    //            //new(config, openApi),
//    //            openApiNavigator,
//    //            paramName + '.' + prop,
//    //            url,
//    //            type
//    //        )
//    //    );

//    //    paramDescriptor = paramDescriptor.SetValidator(childValidator);
//    //    return this;
//    //}
//}

//public class ApiValidationBuilder<T>
//{
//    private readonly PopApiOpenApiConfig config;
//    private readonly ApiValidator<T> validator;
//    private readonly JObject? openApi;
//    private readonly string url;
//    private readonly string type;

//    public ApiValidationBuilder(PopApiOpenApiConfig config, ApiValidator<T> validator, JObject? openApi, string url, string type)
//    {
//        this.config = config;
//        this.validator = validator;
//        this.openApi = openApi;
//        this.url = url;
//        this.type = type;

//        //AddValidationAttribute();
//    }

//    //public JArray GetParamValidationAttribute(string url, string type, string paramName)
//    //{
//    //    if (openApi
//    //        ["paths"]
//    //        [url]
//    //        [type
//    //        ]["parameters"]
//    //        .First(x => x["name"].Value<string>() == paramName)
//    //        ["schema"]
//    //        [config.CustomValidationAttribute] is not null)
//    //    {
//    //        return openApi
//    //            ["paths"]
//    //            [url]
//    //            [type
//    //            ]["parameters"]
//    //            .First(x => x["name"].Value<string>() == paramName)
//    //            ["schema"]
//    //            [config.CustomValidationAttribute] as JArray;
//    //    }

//    //    var arr = new JArray();
//    //    openApi
//    //        ["paths"]
//    //        [url]
//    //        [type
//    //        ]["parameters"]
//    //        .First(x => x["name"].Value<string>() == paramName)
//    //        ["schema"]
//    //        [config.CustomValidationAttribute] = arr;

//    //    return arr;
//    //}

//    //private void AddValidationAttribute()
//    //{
//    //    foreach (var path in openApi["paths"].Children())// as Newtonsoft.Json.Linq.JObject)?.PropertyValues( ) ?? []
//    //    {
//    //        var pathKey = path.Path;
//    //        var pathValue = path.First;

//    //        foreach(var getpostput in path.Children())
//    //        {
//    //            foreach (var getpostputObject in getpostput.Children())
//    //            {
//    //                foreach (var getpostputContent in getpostputObject.Children())
//    //                {
//    //                    getpostputContent[config.CustomValidationAttribute] = new JObject();

//    //                    if (getpostputContent["parameters"] is not null)
//    //                    {
//    //                        foreach(var parameter in (getpostputContent["parameters"] as JArray))
//    //                        {
//    //                            parameter["schema"][config.CustomValidationAttribute] = new JArray();
//    //                        }
//    //                    }
//    //                }
//    //            }
//    //        }
//    //    }
//    //}

//    public ParamBuilder<TParamType, T> ParamIs<TParamType>(string paramName)
//    {
//        return new ParamBuilder<TParamType, T>(config, validator.Param.Is<TParamType>(), new(config, openApi), paramName, url, type);
//    }
//}

//class JsonUtil
//{
//    public static string JsonPrettify(string json)
//    {
//        using (var stringReader = new StringReader(json))
//        using (var stringWriter = new StringWriter())
//        {
//            var jsonReader = new JsonTextReader(stringReader);
//            var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Newtonsoft.Json.Formatting.Indented };
//            jsonWriter.WriteToken(jsonReader);
//            return stringWriter.ToString();
//        }
//    }
//}

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

    public async Task<OpenApiHelper> GetHelper<TFuncOutput>(
       TestWebApiConfig config,
       string methodName,
       string url,
       TValidator validator
    )
    {
        var cleanOpenApi = await setup.GetCleanContent();
        if (cleanOpenApi == null) { throw new Exception("Clean api json is missing"); }

        //var controllerurl = typeof(TController).GetCustomAttributes(typeof(RouteAttribute)).Cast<RouteAttribute>().First().Template;
        var method = typeof(TController).GetMethod(methodName);

        var postname = method?.GetCustomAttributes(typeof(HttpPostAttribute), false).Cast<IRouteTemplateProvider>().FirstOrDefault();
        var getname = method?.GetCustomAttributes(typeof(HttpGetAttribute), false).Cast<IRouteTemplateProvider>().FirstOrDefault();
        var putname = method?.GetCustomAttributes(typeof(HttpPutAttribute), false).Cast<IRouteTemplateProvider>().FirstOrDefault();
        var patchname = method?.GetCustomAttributes(typeof(HttpPatchAttribute), false).Cast<IRouteTemplateProvider>().FirstOrDefault();
        var deletename = method?.GetCustomAttributes(typeof(HttpDeleteAttribute), false).Cast<IRouteTemplateProvider>().FirstOrDefault();
        var headname = method?.GetCustomAttributes(typeof(HttpHeadAttribute), false).Cast<IRouteTemplateProvider>().FirstOrDefault();
        var optionsname = method?.GetCustomAttributes(typeof(HttpOptionsAttribute), false).Cast<IRouteTemplateProvider>().FirstOrDefault();

        IRouteTemplateProvider attr = postname ?? getname ?? putname ?? patchname ?? deletename ?? optionsname ?? headname ?? throw new Exception("no type found");
        //var name = attr?.Template ?? "";

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
        return await OpenApiSetup.GetHelperv2(config,  cleanOpenApi, url, type, validator);
    }

    public async Task<ApiValidationBuilder> GetBuilder<TFuncOutput>(
       TestWebApiConfig config,
       string methodName,
       string url,
       TValidator validator
    )
    {
        var cleanOpenApi = await setup.GetCleanContent();
        if (cleanOpenApi == null) { throw new Exception("Clean api json is missing"); }

        //var controllerurl = typeof(TController).GetCustomAttributes(typeof(RouteAttribute)).Cast<RouteAttribute>().First().Template;
        var method = typeof(TController).GetMethod(methodName);

        var postname = method?.GetCustomAttributes(typeof(HttpPostAttribute), false).Cast<IRouteTemplateProvider>().FirstOrDefault();
        var getname = method?.GetCustomAttributes(typeof(HttpGetAttribute), false).Cast<IRouteTemplateProvider>().FirstOrDefault();
        var putname = method?.GetCustomAttributes(typeof(HttpPutAttribute), false).Cast<IRouteTemplateProvider>().FirstOrDefault();
        var patchname = method?.GetCustomAttributes(typeof(HttpPatchAttribute), false).Cast<IRouteTemplateProvider>().FirstOrDefault();
        var deletename = method?.GetCustomAttributes(typeof(HttpDeleteAttribute), false).Cast<IRouteTemplateProvider>().FirstOrDefault();
        var headname = method?.GetCustomAttributes(typeof(HttpHeadAttribute), false).Cast<IRouteTemplateProvider>().FirstOrDefault();
        var optionsname = method?.GetCustomAttributes(typeof(HttpOptionsAttribute), false).Cast<IRouteTemplateProvider>().FirstOrDefault();

        IRouteTemplateProvider attr = postname ?? getname ?? putname ?? patchname ?? deletename ?? optionsname ?? headname ?? throw new Exception("no type found");
        //var name = attr?.Template ?? "";

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
        return (await OpenApiSetup.GetHelperv2(config, cleanOpenApi, url, type, validator)).Builder;
    }
}
