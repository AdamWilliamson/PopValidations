using Microsoft.OpenApi.Any;

namespace PopValidations.Swashbuckle.Internal;

public class PopValidationArray
{
    private readonly OpenApiArray array;
    private string lineHeader;

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
        array.Add(new OpenApiString(lineHeader + item));
    }
}
