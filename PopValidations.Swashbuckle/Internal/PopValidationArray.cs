using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using System.Net.Http.Headers;

namespace PopValidations.Swashbuckle.Internal;

public class PopValidationArray
{
    private readonly OpenApiArray array;
    private string lineHeader = String.Empty;

    public PopValidationArray(OpenApiArray array)
    {
        array = array ?? throw new ArgumentNullException(nameof(array));

        this.array = array;
    }

    public void SetLineHeader(string lineHeader)
    {
        this.lineHeader = lineHeader ?? string.Empty;
    }

    public void Add(string? item)
    {
        if (string.IsNullOrWhiteSpace(item)) return;
        if (array.Cast<OpenApiString>().Any(x => x?.Value?.Equals(lineHeader + item) ?? false)) return;

        array.Add(new OpenApiString(lineHeader + item));
    }

    public static PopValidationArray From(
        string extensionKey,
        IDictionary<string, IOpenApiExtension> extensionsDict,
        string propertyKey
    ) 
    {
        return new PopValidationArray(
            InitArray(
                InitExtension(extensionKey, extensionsDict), 
                propertyKey
            )
        );
    }

    private static OpenApiObject InitExtension(string extensionKey, IDictionary<string, IOpenApiExtension> extensionsDict)
    {
        if (!extensionsDict.ContainsKey(extensionKey))
        {
            extensionsDict.Add(extensionKey, new OpenApiObject());
        }
        else if (extensionsDict[extensionKey] is not OpenApiObject)
        {
            extensionsDict[extensionKey] = new OpenApiObject();
        }

        return (extensionsDict[extensionKey] as OpenApiObject)!;
    }

    private static OpenApiArray InitArray(
        OpenApiObject owningObject,
        string propertyKey
    )
    {
        if (!owningObject.ContainsKey(propertyKey))
        {
            owningObject.Add(propertyKey, new OpenApiArray());
        }
        else if (owningObject[propertyKey] is not OpenApiArray)
        {
            owningObject[propertyKey] = new OpenApiArray();
        }

        return (owningObject[propertyKey] as OpenApiArray)!;
    }
}
