using Microsoft.OpenApi.Any;

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
}
