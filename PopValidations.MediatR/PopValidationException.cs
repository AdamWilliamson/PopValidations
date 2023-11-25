using System.Net;
using System.Web.Http;

namespace PopValidations.MediatR;

public class PopValidationHttpException : HttpResponseException
{
    public PopValidationHttpException(Dictionary<string, List<string>> errors)
        : base(HttpStatusCode.UnprocessableEntity)
    {
        Errors = errors;
    }

    public Dictionary<string, List<string>> Errors { get; }
}
