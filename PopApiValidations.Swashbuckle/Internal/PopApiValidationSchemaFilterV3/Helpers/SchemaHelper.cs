using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.Helpers;

public static class SchemaHelper
{
    public static bool IsSimpleType(OpenApiSchema schema)
    {
        // Simple types are primitive types or well-known types like string, number, boolean, etc.
        return schema.Type == "string" || schema.Type == "number" || schema.Type == "integer" || schema.Type == "boolean";
    }
}
