using MediatR;

namespace PopValidations.ExampleWebApi.Handlers;

public class TestRequest : IRequest<TestResponse> { }

public class TestHandler : IRequestHandler<TestRequest, TestResponse>
{
    public Task<TestResponse> Handle(TestRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new TestResponse());
    }
}

public class TestResponse { }