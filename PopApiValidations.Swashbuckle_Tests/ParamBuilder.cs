using Microsoft.OpenApi.Any;
using Newtonsoft.Json.Linq;
using PopApiValidations.Swashbuckle;
using PopValidations.Swashbuckle;

namespace PopApiValidations.Swashbuckle_Tests;

public class ParamBuilder<TParamType>
{
    private readonly PopApiOpenApiConfig config;
    private readonly string paramName;
    private readonly string? propertyName = null;
    private readonly string url;
    private readonly string type;
    OpenApiNavigator openApiNavigator;
    private readonly string[] objHeirarcy;
    readonly Type[] BasicDataTypes = [typeof(int), typeof(double), typeof(string), typeof(bool), typeof(DateTime), typeof(DateTimeOffset)];

    public ParamBuilder(
        PopApiOpenApiConfig config,
        OpenApiNavigator openApiNavigator,
        string paramName,
        string url,
        string type)
    {
        this.config = config;
        this.paramName = paramName;
        this.url = url;
        this.type = type;
        this.openApiNavigator = openApiNavigator;
    }

    public ParamBuilder(
       PopApiOpenApiConfig config,
       OpenApiNavigator openApiNavigator,
       string paramName,
       string propertyName,
       string url,
       string type)
    {
        this.config = config;
        this.paramName = paramName;
        this.propertyName = propertyName;
        this.url = url;
        this.type = type;
        this.openApiNavigator = openApiNavigator;
    }

    public ParamBuilder(
    PopApiOpenApiConfig config,
    OpenApiNavigator openApiNavigator,
    string[] objHeirarcy,
    string url,
    string type)
    {
        this.config = config;
        this.url = url;
        this.type = type;
        this.openApiNavigator = openApiNavigator;
        this.objHeirarcy = objHeirarcy;
    }

    public void SetParameterValidation(string validation)
    {
        var validationLevel = config.TypeValidationLevel?.Invoke(typeof(TParamType)) ?? ValidationLevel.FullDetails;

        if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
        {
            var arrays = openApiNavigator.GetParamValidationsArray(url, type, objHeirarcy[0]);

            foreach (var valArray in arrays)
            {
                valArray.Add(validation);
            }
        }
        else if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
        {
            openApiNavigator.GetPathValidations(url, type, paramName).Add(validation);
        }
    }

    public void SetParameterPropertyValidation(string parent, string propertyname, string validation)
    {
        var validationLevel = config.TypeValidationLevel?.Invoke(typeof(TParamType)) ?? ValidationLevel.FullDetails;

        if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
        {
            var schemas = openApiNavigator.GetParamPropertyValidationObjects(url, type, paramName);

            foreach (var valobj in schemas)
            {
                if (valobj[propertyName] is null)
                {
                    valobj.Add(propertyname, new JArray());
                }

                (valobj[propertyName] as JArray).Add(validation);
            }
        }
        else if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
        {
            var pathValObj = openApiNavigator.GetPathValidationObject(url, type);
            if (pathValObj[$"{parent}.{propertyname}"] is null)
            {
                pathValObj.Add($"{parent}.{propertyname}", new JArray());
            }

            (pathValObj[$"{parent}.{propertyname}"] as JArray).Add(validation);
        }
    }

    public void SetParameterPropertyValidation(string validation)
    {
        if (objHeirarcy.Length == 0)
        {
            throw new Exception("no validation param set");
        }
        if (objHeirarcy.Length == 1)
        {
            SetParameterValidation(validation);
            return;
        }

        var validationLevel = config.TypeValidationLevel?.Invoke(typeof(TParamType)) ?? ValidationLevel.FullDetails;

        if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
        {
            var nonFieldHeirarchy = objHeirarcy[..^1];
            var schemas = openApiNavigator.GoToSchema(url, type, nonFieldHeirarchy);
            var propertyname = objHeirarcy[^1];

            foreach (var valobj in schemas)
            {
                if (valobj[config.CustomValidationAttribute] is null)
                {
                    valobj.Add(config.CustomValidationAttribute, new JObject());
                }

                if (valobj[config.CustomValidationAttribute][propertyname] is null)
                {
                    (valobj[config.CustomValidationAttribute]as JObject).Add(propertyname, new JArray());
                }

               (valobj[config.CustomValidationAttribute][propertyname] as JArray).Add(validation);
            }
        }
        else if (validationLevel.HasFlag(ValidationLevel.ValidationAttribute))
        {
            string propertyString = string.Join('.', objHeirarcy);
            var pathValObj = openApiNavigator.GetPathValidationObject(url, type);
            
            if (pathValObj[propertyString] is null)
            {
                pathValObj.Add(propertyString, new JArray());
            }

            (pathValObj[propertyString] as JArray).Add(validation);
        }
    }

    public ParamBuilder<TParamType> IsNotNull()
    {
        if (objHeirarcy.Length == 1)
        {
            var parent = openApiNavigator.GetParamValidationOwner(url, type, objHeirarcy[^1]);
            if (parent != null)
            {
                parent["required"] = true;
                parent.Remove("nullable");
            }
            SetParameterValidation("Must not be null.");

            return this;
        }
        else
        {
            //var parent = openApiNavigator.GetParamValidationOwner(url, type, paramName);
            //var parentSchemas = openApiNavigator.GetParamValidationObjectSchemas(url, type, objHeirarcy[^1]);
            var parentSchemas = openApiNavigator.GetParamValidationObjectSchemas(url, type, objHeirarcy[..^1]);
            var propertyname = objHeirarcy[^1];

            foreach (var parent in parentSchemas.Where(x => x != null))
            {
                if (parent["required"] is not JArray)
                {
                    //parent.Add("required", new JArray());
                    parent.AddFirst(new JProperty("required", new JArray()));
                }

                if (!(parent["required"] as JArray).Any(x => x.Value<string>() == propertyname))
                {
                    (parent["required"] as JArray).Add(propertyname);

                    parent["required"] = new JArray(parent["required"].OrderBy(x => x));
                }

                (parent["properties"][propertyname] as JObject).Remove("nullable");

            }

            //SetParameterPropertyValidation(paramName, propertyName, "Must not be null.");
            SetParameterPropertyValidation("Must not be null.");
            //var property = openApiNavigator.GetParamValidationObjects(url, type, paramName);
            //var property = openApiNavigator.GetParamValidationObjects(url, type, paramName, propertyName);
            //property["properties"][propertyName].

            return this;
        }
    }
}
