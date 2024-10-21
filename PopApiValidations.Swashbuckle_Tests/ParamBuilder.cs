using FluentAssertions;
using Microsoft.OpenApi.Any;
using Newtonsoft.Json.Linq;
using PopApiValidations.Swashbuckle;
using PopValidations.Swashbuckle;
using System.ComponentModel.DataAnnotations;

namespace PopApiValidations.Swashbuckle_Tests;

public enum ParamType
{
    Auto,
    FromBody,
    FromQuery,
    FromUrl
}

public class ParamBuilder<TParamType>
{
    private readonly PopApiOpenApiConfig config;
    //private readonly string paramName;
    //private readonly string? propertyName = null;
    private readonly string url;
    private readonly string type;
    OpenApiNavigator openApiNavigator;
    private readonly string[] objHeirarcy;
    readonly Type[] BasicDataTypes = [typeof(int), typeof(double), typeof(string), typeof(bool), typeof(DateTime), typeof(DateTimeOffset)];

    public ParamType ParamType { get; }

    public ParamBuilder(
        ParamType paramType,
        PopApiOpenApiConfig config,
        OpenApiNavigator openApiNavigator,
        string[] objHeirarcy,
        string url,
        string type)
    {
        ParamType = paramType;
        this.config = config;
        this.url = url;
        this.type = type;
        this.openApiNavigator = openApiNavigator;
        this.objHeirarcy = objHeirarcy;
    }

    //public void SetParameterValidation(string validation)
    //{
    //    var validationLevel = config.TypeValidationLevel?.Invoke(typeof(TParamType)) ?? ValidationLevel.FullDetails;

    //    if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
    //    {
    //        var arrays = openApiNavigator.GetParamValidationsArray(url, type, objHeirarcy[0]);

    //        foreach (var valArray in arrays)
    //        {
    //            valArray.Add(validation);
    //        }
    //    }
    //    else if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
    //    {
    //        openApiNavigator.GetPathValidations(url, type, paramName).Add(validation);
    //    }
    //}

    public void SetParameterValidation2(string validation)
    {
        var validationLevel = config.TypeValidationLevel?.Invoke(typeof(TParamType)) ?? ValidationLevel.FullDetails;

        if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
        {
            openApiNavigator.GetParamOrRequestBodyPropertyValidationArrayPair(url, type, objHeirarcy)
                .ModifyList(clean => clean.Add(validation))
                .AssertList((schema, result) => result.SetResult(schema.Values().Contains(validation)));
        }
        else if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
        {
            openApiNavigator.GetPathValidationsPair(url, type, objHeirarcy[0])
                .Modify(clean => clean.Add(validation))
                .Assert((schema, result) => result.SetResult(schema.Contains(validation)));
        }
    }

    //public void SetParameterPropertyValidation(string parent, string propertyname, string validation)
    //{
    //    var validationLevel = config.TypeValidationLevel?.Invoke(typeof(TParamType)) ?? ValidationLevel.FullDetails;

    //    if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
    //    {
    //        var schemas = openApiNavigator.GetParamPropertyValidationObjects(url, type, paramName);

    //        foreach (var valobj in schemas)
    //        {
    //            if (valobj[propertyName] is null)
    //            {
    //                valobj.Add(propertyname, new JArray());
    //            }

    //            (valobj[propertyName] as JArray).Add(validation);
    //        }
    //    }
    //    else if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
    //    {
    //        var pathValObj = openApiNavigator.GetPathValidationObject(url, type);
    //        if (pathValObj[$"{parent}.{propertyname}"] is null)
    //        {
    //            pathValObj.Add($"{parent}.{propertyname}", new JArray());
    //        }

    //        (pathValObj[$"{parent}.{propertyname}"] as JArray).Add(validation);
    //    }
    //}

    //public void SetParameterPropertyValidation(string validation)
    //{
    //    if (objHeirarcy.Length == 0)
    //    {
    //        throw new Exception("no validation param set");
    //    }
    //    if (objHeirarcy.Length == 1 && ParamType != ParamType.FromQuery)
    //    {
    //        SetParameterValidation(validation);
    //        return;
    //    }

    //    var validationLevel = config.TypeValidationLevel?.Invoke(typeof(TParamType)) ?? ValidationLevel.FullDetails;

    //    if (ParamType != ParamType.FromQuery && validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
    //    {
    //        var nonFieldHeirarchy = objHeirarcy[..^1];
    //        var schemas = openApiNavigator.GoToSchema(url, type, nonFieldHeirarchy);
    //        var propertyname = objHeirarcy[^1];

    //        foreach (var valobj in schemas)
    //        {
    //            if (valobj[config.CustomValidationAttribute] is null)
    //            {
    //                valobj.Add(config.CustomValidationAttribute, new JObject());
    //                //throw new ValidationException("Custom Validation Attribute");
    //            }

    //            if (valobj[config.CustomValidationAttribute][propertyname] is null)
    //            {
    //                (valobj[config.CustomValidationAttribute]as JObject).Add(propertyname, new JArray());
    //            }

    //           (valobj[config.CustomValidationAttribute][propertyname] as JArray).Add(validation);
    //        }
    //    }
    //    else if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
    //    {
    //        string propertyString = string.Join('.', objHeirarcy);
    //        var pathValObj = openApiNavigator.GetPathValidationObject(url, type);
            
    //        if (pathValObj[propertyString] is null)
    //        {
    //            pathValObj.Add(propertyString, new JArray());
    //        }

    //        (pathValObj[propertyString] as JArray).Add(validation);
    //    }
    //}

    public void SetParameterPropertyValidation2(string validation)
    {
        if (openApiNavigator.CheckIfParameterExists(url, type, objHeirarcy))
        {
            
        }

        if (objHeirarcy.Length == 1 && ParamType != ParamType.FromQuery)
        {
            SetParameterValidation2(validation);
            return;
        }

        var validationLevel = config.TypeValidationLevel?.Invoke(typeof(TParamType)) ?? ValidationLevel.FullDetails;

        if (ParamType != ParamType.FromQuery && validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
        {
            var nonFieldHeirarchy = objHeirarcy[..^1];
            var schemas = openApiNavigator.GoToParamSchemaPair(url, type, nonFieldHeirarchy);
            var propertyname = objHeirarcy[^1];

            schemas
                .Modify(
                    (config.CustomValidationAttribute, () => new JObject()),
                    (propertyname, () => new JArray()),
                    (null, () => validation)
                )
                //.Modify(clean =>
                //{
                //    foreach (var valobj in clean)
                //    {
                //        if (valobj[config.CustomValidationAttribute] is null)
                //        {
                //            valobj.Add(config.CustomValidationAttribute, new JObject());
                //        }

                //        if (valobj[config.CustomValidationAttribute][propertyname] is null)
                //        {
                //            (valobj[config.CustomValidationAttribute] as JObject).Add(propertyname, new JArray());
                //        }

                //        (valobj[config.CustomValidationAttribute][propertyname] as JArray).Add(validation);
                //    }
                //})
                .Assert((schema, result) => 
                    result.SetResult(
                        (schema[config.CustomValidationAttribute][propertyname] as JArray).Contains(validation)
                    )
                );
        }
        else if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
        {
            string propertyString = string.Join('.', objHeirarcy);
            openApiNavigator.GetPathValidationObjectPair(url, type)
                .Modify((propertyString, () => new JArray()), (null, () => validation))
                //.Modify(clean =>
                //{
                //    if (clean[propertyString] is null)
                //    {
                //        clean.Add(propertyString, new JArray());
                //    }

                //    (clean[propertyString] as JArray).Add(validation);
                //})
                .Assert(
                    (schema, result) => result.SetResult((schema[propertyString] as JArray).Values().Contains(validation))
                );
        }
    }

    //public ParamBuilder<TParamType> IsNotNull()
    //{
    //    if (objHeirarcy.Length == 1 && ParamType == ParamType.FromQuery)
    //    {
    //        SetParameterPropertyValidation("Must not be null.");
    //        return this;
    //    }

    //    if (objHeirarcy.Length == 1)
    //    {
    //        var parent = openApiNavigator.GetParamValidationOwner(url, type, objHeirarcy[^1]);

    //        if (parent != null)
    //        {
    //            parent["required"] = true;
    //            parent.Remove("nullable");
    //        }

    //        SetParameterValidation("Must not be null.");

    //        return this;
    //    }
    //    else if (ParamType == ParamType.FromQuery)
    //    {
    //        var propertyname = string.Join('.', objHeirarcy);
    //        var parentSchemas = openApiNavigator.GetParamObjects(url, type, propertyname);

    //        foreach (var parent in parentSchemas.Where(x => x != null))
    //        {
    //            parent["required"] = true;
    //            parent.Remove("nullable");
    //        }

    //        SetParameterPropertyValidation("Must not be null.");

    //        return this;
    //    }
    //    else
    //    {
    //        var parentSchemas = openApiNavigator.GetParamValidationObjectSchemas(url, type, objHeirarcy[..^1]);
    //        var propertyname = objHeirarcy[^1];

    //        foreach (var parent in parentSchemas.Where(x => x != null))
    //        {
    //            if (parent["required"] is not JArray)
    //            {
    //                parent.AddFirst(new JProperty("required", new JArray()));
    //            }

    //            if (!(parent["required"] as JArray).Any(x => x.Value<string>() == propertyname))
    //            {
    //                (parent["required"] as JArray).Add(propertyname);

    //                parent["required"] = new JArray(parent["required"].OrderBy(x => x));
    //            }

    //            (parent["properties"][propertyname] as JObject).Remove("nullable");

    //        }

    //        SetParameterPropertyValidation("Must not be null.");

    //        return this;
    //    }

    //}
    //=====

    public bool HasChildren()
    {
        return
            (openApiNavigator.CheckIfRequestBody(url, type, objHeirarcy) && objHeirarcy?.Any() == true)
            || (openApiNavigator.CheckIfRequestBody(url, type, objHeirarcy) && objHeirarcy!.Length > 1);
    }

    public ParamBuilder<TParamType> IsNotNull2()
    {
        Pair<List<JObject>> ParamSchema;

        if (openApiNavigator.CheckIfRequestBody(url, type, objHeirarcy))
        {
            if (!HasChildren())//objHeirarcy is null || objHeirarcy.Length == 0)
            {
                openApiNavigator.GetPath(url, type)
                    .Modify(
                        (config.CustomValidationAttribute, () => new JObject()),
                        ("RequestBody", () => new JArray()),
                        (null, () => "Must not be null.")
                    )
                    .Assert((schema, result) => result.SetResult(schema[config.CustomValidationAttribute]?["RequestBody"]?.Values().Contains("Must not be null.") == true));

                openApiNavigator.GetRequestBodyParentPair(url, type)
                    .Modify(("required", () => true))
                    .Assert((schema, result) => result.SetResult(schema["required"]?.Value<bool>() == true));

                return this;
            }

            ParamSchema = openApiNavigator.GetRequestBodySchemasPair(url, type, objHeirarcy);
        }
        else
        {
            if (!HasChildren())
            {
                openApiNavigator.GetPath(url, type)
                    .Modify(
                        (config.CustomValidationAttribute, () => new JObject()),
                        (objHeirarcy[0], () => new JArray()),
                        (null, () => "Must not be null.")
                    )
                    .Assert((schema, result) =>
                        result.SetResult(schema[config.CustomValidationAttribute]?[objHeirarcy[0]]?.Values()?.Contains("Must not be null.") == true)
                    );

                openApiNavigator.ParameterPairByName(url, type, objHeirarcy)
                    .Modify(("required", () => true))
                    .Assert((schema, result) => result.SetResult(schema["required"]?.Value<bool>() == true));

                return this;
            }

            var paramBase = openApiNavigator.GoToParamSchemaPair(url, type, objHeirarcy);
            
            if (HasChildren()) 
            {
                paramBase
                    .Modify(("required", () => true))
                    .ModifyRemove("nullable")
                    .Assert((schema, result) =>
                    {
                        result.SetResult(schema["required"]?.Value<bool>() == true);
                        result.SetResult(schema["nullable"] == null);
                    });
            }

            ParamSchema = paramBase.Nav(x => new List<JObject>() { x });
        }

        var ParamName = objHeirarcy[^1];
        ParamSchema
            .ModifyList(
                (config.CustomValidationAttribute, () => new JObject()),
                (ParamName, () => new JArray()),
                (null, () => "Must not be null.")
            )
            .AssertList((schema, result) =>
                result.SetResult(schema[config.CustomValidationAttribute]?[ParamName]?.Values().Contains("Must not be null.") == true)
            )
            .ModifyList(
                ("required", () => new JArray()),
                (null, () => ParamName)
            )
            .AssertList((schema, result) =>
                result.SetResult(schema["required"]?.Values().Contains(ParamName) == true)
            )
            //.NavList(schema => schema["properties"]?[ParamName])
            //.ModifyList(("required", () => true))
            //.ModifyListRemove("nullable")
            //.AssertList((schema, result) =>
            //{
            //    result.SetResult(schema["required"]?.Value<bool>() == true);
            //    result.SetResult(schema["nullable"] == null);
            //})
            ;


        //if (openApiNavigator.CheckIfParameterExists(url, type, objHeirarcy))
        //{
        //    if (objHeirarcy.Length == 1)
        //    {
        //        SetParameterValidation2("Must not be null.");
        //    }
        //    else
        //    {

        //    }

        //    openApiNavigator.GetParamValidationOwnerPair(url, type, objHeirarcy)
        //        .Modify(("required", () => true))
        //        .ModifyRemove("nullable")
        //        .Assert((schema, result) => {
        //            result.SetResult(schema["required"]?.Value<bool>() == true);
        //            result.SetResult(schema["nullable"] == null);
        //        });
        //}
        //else if (openApiNavigator.CheckIfRequestBody(url, type, objHeirarcy))
        //{
        //    if (objHeirarcy is null || objHeirarcy.Length == 0)
        //    {
        //        openApiNavigator.GetRequestBodyParentPair(url, type)
        //            .Modify(("required", () => true))
        //            .Assert((schema, result) => result.SetResult(schema["required"]?.Value<bool>() == true));
        //    }
        //    else
        //    {
        //        var propertyname = objHeirarcy[^1];

        //        openApiNavigator.GetRequestBodySchemasPair(url, type, objHeirarcy)
        //            .ModifyList(("required", () => new JArray()), (null, () => propertyname))
        //            .AssertList((schema, result) =>
        //                result.SetResult((schema["required"] as JArray)?.ToList()?.Contains(propertyname) == true)
        //            )
        //            .NavList(schema => schema["properties"]?[propertyname] as JObject)
        //            .ModifyListRemove("nullable")
        //            .AssertList((schema, result) => result.SetResult(schema["nullable"] == null))
        //            ;
        //    }

        //    SetParameterPropertyValidation2("Must not be null.");

        //    return this;
        //}

        //===================================================================================================
        //if (objHeirarcy.Length == 1 && ParamType == ParamType.FromQuery)
        //{
        //    SetParameterPropertyValidation2("Must not be null.");
        //    return this;
        //}

        //if (objHeirarcy.Length == 1)
        //{
        //    openApiNavigator.GetParamValidationOwnerPair(url, type, objHeirarcy)
        //        .Modify(("required", () => true))
        //        .ModifyRemove("nullable")
        //        //.Modify(clean =>
        //        //{
        //        //    clean["required"] = true;
        //        //    clean.Remove("nullable");
        //        //})
        //        .Assert((schema, result) => {
        //            result.SetResult(schema["required"].Value<bool>() == true);
        //            result.SetResult(schema["nullable"] == null);
        //        });

        //    SetParameterValidation2("Must not be null.");

        //    return this;
        //}
        //else if (ParamType == ParamType.FromQuery)
        //{
        //    var propertyname = string.Join('.', objHeirarcy);
        //    openApiNavigator.GetParamValidationOwnerPair(url, type, objHeirarcy)
        //        .Modify(clean =>
        //        {
        //            clean["required"] = true;
        //            clean.Remove("nullable");
        //        })
        //        .Assert((schema, result) => {
        //            result.SetResult(schema["required"].Value<bool>() == true);
        //            result.SetResult(schema["nullable"] == null);
        //            //result.SetResult(schema["properties"][propertyname]["nullable"] == null);
        //        });

        //    SetParameterPropertyValidation2("Must not be null.");

        //    return this;
        //}
        //else
        //{
        //    var parentSchemas = openApiNavigator.GetRequestBodySchemasPair(url, type, objHeirarcy);
        //    var propertyname = objHeirarcy[^1];

        //    parentSchemas
        //        .ModifyList(("required", () => new JArray()), (null, () => propertyname))
        //        .Assert((schema, result) => {
        //            schema.ForEach(x =>
        //            {
        //                result.SetResult((x["required"] as JArray).ToList().Contains(propertyname));
        //            });
        //        })
        //        .Nav(schema => 
        //            schema.Select(x => x["properties"][propertyname] as JObject).ToList()
        //        )
        //        .ModifyListRemove("nullable")
        //        //.Modify(clean =>
        //        //{
        //        //    foreach (var parent in clean.Where(x => x != null))
        //        //    {
        //        //        if (parent["required"] is not JArray)
        //        //        {
        //        //            parent.AddFirst(new JProperty("required", new JArray()));
        //        //        }

        //        //        if (!(parent["required"] as JArray).Any(x => x.Value<string>() == propertyname))
        //        //        {
        //        //            (parent["required"] as JArray).Add(propertyname);
        //        //        }

        //        //        (parent["properties"][propertyname] as JObject).Remove("nullable");

        //        //    }
        //        //})
        //        .Assert((schema, result) => {
        //            schema.ForEach(x =>
        //            {
        //                result.SetResult(x["nullable"] == null);
        //            });
        //        })
        //        //.Assert((schema, result) => {
        //        //    schema.ForEach(x =>
        //        //    {
        //        //        result.SetResult((x["required"] as JArray).Contains(propertyname));
        //        //        result.SetResult(x["properties"][propertyname]["nullable"] == null);
        //        //    });
        //        //})
        //        ;

        //    SetParameterPropertyValidation2("Must not be null.");

        //return this;
        //}
        return this;
    }
}
