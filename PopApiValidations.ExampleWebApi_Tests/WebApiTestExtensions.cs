using System.Net;
using FluentAssertions;
using System.Text.Json;
using PopValidations.Execution.Validation;

namespace PopApiValidations.ExampleWebApi_Tests;

public static class WebApiTestExtensions
{
    public static void EnsureValidationErrors(this HttpResponseMessage response) 
    {
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableContent);
    }

    public static async Task<ValidationResult> ToValidationResult(this HttpResponseMessage response)
    {
        var result = await response.Content.ReadAsStringAsync();
        result.Should().NotBeEmpty();
        var objs = JsonSerializer.Deserialize<Response>(result);
        var vr = new ValidationResult();
        if (objs != null)
        {
            foreach (var err in objs.errors)
            {
                vr.Errors.Add(err.Key, err.Value);
            }
        }

        return vr;
    }
}
