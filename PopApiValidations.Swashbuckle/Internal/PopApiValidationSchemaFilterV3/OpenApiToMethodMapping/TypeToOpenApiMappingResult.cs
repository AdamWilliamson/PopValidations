using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.MethodSimplification;
using PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.OpenApiSimplification;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.OpenApiToMethodMapping
{
    public class TypeToOpenApiMappingResult
    {
        public string Route { get; private set; }
        public string ResultPropertyHeirarchy { get; private set; }//=> PropertyMapping?.ResultPropertyName ?? string.Empty;
        public string OpenApiObjHeirarchy { get; private set; }
        public string OpenApiPropertyName { get; private set; }
        public PropertyMapping? PropertyMapping { get; private set; }
        //[Obsolete]
        public OpenApiSchema? DirectParentSchema { get; private set; }
        public OpenApiSchema? PropertySchema { get; private set; }
        public OpenApiSchema[]? PropertySecondarySchemas { get; private set; }
        public IDictionary<string, IOpenApiExtension> ParentPropertyExtensions { get; private set; }
        public OpenApiParameter? Parameter { get; private set; }
        public OpenApiRequestBody? RequestBody { get; private set; }
        public OpenApiSchema[] RequestBodyContentSchemas => RequestBody?.Content.Select(c => c.Value.Schema)?.ToArray() ?? new OpenApiSchema[0];
        public bool IsArray { get; private set; }
        public IOpenApiParameterMapping ParameterMapping { get; private set; } // Added to store the method parameter index
        public IGeneralMapping FunctionItemMapping { get; private set; }
        //public IGeneralMapping TypeMapping { get; set; } // Added to store the method parameter index
        public OpenApiResponseMapping? ResponseMapping { get; private set; }
        public ReturnMapping? FunctionReturnMapping { get => FunctionItemMapping as ReturnMapping; }

        private TypeToOpenApiMappingResult() { }

        public static TypeToOpenApiMappingResult ForParameter(
            bool asArrayNotation,
            string route,
            OpenApiParameterMapping parameter,
            ParameterMapping parameterMapping,
            PropertyMapping? foundProperty
            )
        {
            var result = new TypeToOpenApiMappingResult();

            result.Route = route;
            result.ParameterMapping = parameter; // Set the parameter index
            result.Parameter = parameter.Parameter;
            result.IsArray = asArrayNotation;
            result.FunctionItemMapping = parameterMapping;
            

            result.OpenApiObjHeirarchy = string.Empty;
            result.ResultPropertyHeirarchy = string.Empty;
            result.OpenApiObjHeirarchy = parameter.Name;
            result.OpenApiPropertyName = parameter.Name;

            if (foundProperty != null)// && foundProperty.ResultPropertyName != parameter.Name)
            {
                result.ResultPropertyHeirarchy = foundProperty.ResultPropertyName ?? string.Empty;
            }

            result.OpenApiObjHeirarchy += (asArrayNotation ? "[n]" : string.Empty);
            result.OpenApiPropertyName += (asArrayNotation ? "[n]" : string.Empty);
            result.ResultPropertyHeirarchy += (asArrayNotation ? "[n]" : string.Empty);

            //OpenApiPropertyName = property.Name + (asArrayNotation ? "[n]" : string.Empty);
            result.DirectParentSchema = parameter.Schemas.First();// parentSchema;

            result.PropertySchema = parameter.Schemas.First();//property.Schemas.First();
            result.PropertyMapping = foundProperty;
            //ResultPropertyHeirarchy = (foundProperty?.ResultPropertyName ?? string.Empty) + (asArrayNotation ? "[n]" : string.Empty);
            result.ParentPropertyExtensions = parameter.ParentPropertyExtensions;// property.ParentPropertyExtensions;
            result.PropertySecondarySchemas = parameter.Schemas.ToArray();
            //ResultPropertyHeirarchy = (foundProperty?.ResultPropertyName ?? string.Empty);

            return result;
        }

        public static TypeToOpenApiMappingResult ForProperty(
            bool asArrayNotation,
            string route,
            string parentResultPrefix,
            IOpenApiParameterMapping parameter,
            IGeneralMapping functionItemMapping,
            PropertyMapping foundProperty,
            OpenApiSchema propertySchema,
            IDictionary<string, IOpenApiExtension> parentProeprtyExtensions,
            OpenApiSchema? directParentSchema
            )
        {
            var result = new TypeToOpenApiMappingResult();

            result.Route = route;
            result.ParameterMapping = parameter; // Set the parameter index
            result.FunctionItemMapping = functionItemMapping;

            if (parameter is OpenApiParameterMapping op)
            {
                result.Parameter = op.Parameter;
            }
            else if (parameter is OpenApiRequestBodyMapping rb)
            {
                result.RequestBody = rb.RequestBody;
            }
            else if (parameter is OpenApiResponseMapping rm)
            {
                result.ResponseMapping = rm;
            }

            result.IsArray = asArrayNotation;

            result.OpenApiObjHeirarchy = parentResultPrefix;
            //if (asArrayNotation) { result.OpenApiObjHeirarchy += "[n]"; }
            result.ResultPropertyHeirarchy = parentResultPrefix;
            result.OpenApiPropertyName = string.Empty;

            if (foundProperty != null)
            {
                if (!string.IsNullOrWhiteSpace(parentResultPrefix)) {
                    if (!string.IsNullOrWhiteSpace(result.OpenApiObjHeirarchy)) result.OpenApiObjHeirarchy += ".";
                    if (!string.IsNullOrWhiteSpace(result.ResultPropertyHeirarchy)) result.ResultPropertyHeirarchy += ".";
                }

                result.OpenApiObjHeirarchy += foundProperty.OpenApiPropertyName;
                result.ResultPropertyHeirarchy = foundProperty.ResultPropertyName;
                result.OpenApiPropertyName = foundProperty.PropertyName;
            }
            result.PropertySchema = propertySchema; // parameter.Schemas.First();//property.Schemas.First();

            result.OpenApiObjHeirarchy += (asArrayNotation ? "[n]" : string.Empty);
            result.ResultPropertyHeirarchy += (asArrayNotation ? "[n]" : string.Empty);
            result.OpenApiPropertyName += (asArrayNotation ? "[n]" : string.Empty);

            //OpenApiPropertyName = property.Name + (asArrayNotation ? "[n]" : string.Empty);
            result.DirectParentSchema = directParentSchema;// parameter.Schemas.First();// parentSchema;
            //PropertySchema = property.Schemas.First();
            result.PropertyMapping = foundProperty;
            //ResultPropertyHeirarchy = (foundProperty?.ResultPropertyName ?? string.Empty) + (asArrayNotation ? "[n]" : string.Empty);
            result.ParentPropertyExtensions = parentProeprtyExtensions;//parameter.ParentPropertyExtensions;// property.ParentPropertyExtensions;
            result.PropertySecondarySchemas = parameter.Schemas.ToArray();
            //ResultPropertyHeirarchy = (foundProperty?.ResultPropertyName ?? string.Empty);

            return result;
        }


        public static TypeToOpenApiMappingResult ForRequestBody(
            bool asArrayNotation,
            string route,
            OpenApiRequestBodyMapping requestBody,
            ParameterMapping parameterMapping,
            PropertyMapping? foundProperty
            )
        {
            var result = new TypeToOpenApiMappingResult();

            result.Route = route;
            result.RequestBody = requestBody.RequestBody;
            result.IsArray = asArrayNotation;
            result.OpenApiObjHeirarchy = string.Empty;
            result.FunctionItemMapping = parameterMapping;
            result.ResultPropertyHeirarchy = string.Empty;
            result.OpenApiPropertyName = "RequestBody";

            if (foundProperty != null)
            {
                result.OpenApiObjHeirarchy = foundProperty.OpenApiPropertyName;
                result.ResultPropertyHeirarchy = foundProperty.ResultPropertyName ?? string.Empty;
                result.OpenApiPropertyName = foundProperty.PropertyName;
            }

            result.OpenApiObjHeirarchy += (asArrayNotation ? "[n]" : string.Empty);
            result.ResultPropertyHeirarchy += (asArrayNotation ? "[n]" : string.Empty);
            result.OpenApiPropertyName += (asArrayNotation ? "[n]" : string.Empty);


            //OpenApiPropertyName = property.Name + (asArrayNotation ? "[n]" : string.Empty);
            //result.DirectParentSchema = requestBody.Schemas.First();// parentSchema;
            //PropertySchema = property.Schemas.First();
            result.PropertyMapping = foundProperty;
            //ResultPropertyHeirarchy = (foundProperty?.ResultPropertyName ?? string.Empty) + (asArrayNotation ? "[n]" : string.Empty);
            
            //result.ParameterMapping = parameter; // Set the parameter index
            result.ParentPropertyExtensions = requestBody.ParentPropertyExtensions;
            result.PropertySecondarySchemas = requestBody.Schemas.ToArray();
            //ResultPropertyHeirarchy = (foundProperty?.ResultPropertyName ?? string.Empty);

            return result;
        }

        public static TypeToOpenApiMappingResult ForResponse(
            bool asArrayNotation,
            string route,
            OpenApiResponseMapping response,
            IGeneralMapping functionItemMapping,
            PropertyMapping? foundProperty
            )
        {
            var result = new TypeToOpenApiMappingResult();

            result.Route = route;
            result.ResponseMapping = response;
            result.IsArray = asArrayNotation;
            result.OpenApiObjHeirarchy = string.Empty;
            result.FunctionItemMapping = functionItemMapping;
            result.OpenApiPropertyName = "Response";
            result.ResultPropertyHeirarchy = string.Empty;

            if (foundProperty != null)
            {
                result.OpenApiObjHeirarchy = foundProperty.OpenApiPropertyName;
            }

            result.OpenApiObjHeirarchy += (asArrayNotation ? "[n]" : string.Empty);
            result.ResultPropertyHeirarchy += (asArrayNotation ? "[n]" : string.Empty);

            //OpenApiPropertyName = property.Name + (asArrayNotation ? "[n]" : string.Empty);
            //result.DirectParentSchema = requestBody.Schemas.First();// parentSchema;
            //PropertySchema = property.Schemas.First();
            result.PropertyMapping = foundProperty;
            //ResultPropertyHeirarchy = (foundProperty?.ResultPropertyName ?? string.Empty) + (asArrayNotation ? "[n]" : string.Empty);

            //result.ParameterMapping = parameter; // Set the parameter index
            result.ParentPropertyExtensions = response.ParentPropertyExtensions;
            result.PropertySecondarySchemas = response.Schemas.ToArray();
            //ResultPropertyHeirarchy = (foundProperty?.ResultPropertyName ?? string.Empty);

            return result;
        }

        //public TypeToOpenApiMappingResult(
        //    bool asArrayNotation,
        //    string route,
        //    //string resultPropertyHeirarchy,
        //    //string openApiObjHeirarchy,
        //    //string openApiPropertyName,
        //    string prefix,
        //    //string openApiPrefix,
        //    IOpenApiPropertyMapping property,
        //    OpenApiSchema? parentSchema,
        //    PropertyMapping? foundProperty,
        //    IOpenApiParameterMapping? parameter,
        //    IGeneralMapping parameterMapping
        //)
        //{
        //    OpenApiObjHeirarchy = string.Empty;
        //    if (!string.IsNullOrWhiteSpace(prefix) && property != null)
        //    {
        //        var newPrefix = string.IsNullOrWhiteSpace(prefix) ? property.Name: prefix + '.' + property.Name;
        //        OpenApiObjHeirarchy = newPrefix;// + (asArrayNotation ? "[n]" : string.Empty);
        //    }

        //    OpenApiObjHeirarchy += (asArrayNotation ? "[n]" : string.Empty);


        //    Route = route;

        //    OpenApiPropertyName = property.Name + (asArrayNotation ? "[n]" : string.Empty);
        //    DirectParentSchema = parentSchema;
        //    PropertySchema = property.Schemas.First();
        //    PropertyMapping = foundProperty;
        //    ResultPropertyHeirarchy = (foundProperty?.ResultPropertyName ?? string.Empty) + (asArrayNotation ? "[n]" : string.Empty);
        //    IsArray = asArrayNotation;
        //    ParameterMapping = parameterMapping; // Set the parameter index
        //    ParentPropertyExtensions = property.ParentPropertyExtensions;
        //    PropertySecondarySchemas = property.Schemas.ToArray();
        //    ResultPropertyHeirarchy = (foundProperty?.ResultPropertyName ?? string.Empty);

        //    if (parameter is OpenApiParameterMapping outParam)
        //    {
        //        Parameter = outParam.Parameter;
        //    }
        //    else if (parameter is OpenApiRequestBodyMapping outBody)
        //    {
        //        RequestBody= outBody.RequestBody;
        //    }
        //    else if (parameter is OpenApiResponseMapping)
        //    {
        //        // No specific OpenApi class
        //    }
        //}

    }
}
