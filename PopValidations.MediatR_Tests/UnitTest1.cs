using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PopValidations.MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations.MediatR_Tests
{
    public record TestCommand(int Id) : IRequest;

    public class TestInputValidator : AbstractValidator<TestCommand>
    {
        public TestInputValidator()
        {
            Describe(x => x.Id).IsLessThan(20);
        }
    }

    public class TestCommandHandler : IRequestHandler<TestCommand>
    {
        public Task Handle(TestCommand request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .RegisterRunner()
                .RegisterAllMainValidators(typeof(TestCommand).Assembly);

            serviceCollection.AddMediatR(
                (x) => x
                    .RegisterServicesFromAssembly(typeof(TestCommand).Assembly)
                    .AddPopValidations()
                //cfg.AddBehavior<PingPongBehavior>();
                //cfg.AddStreamBehavior<PingPongStreamBehavior>();
                //cfg.AddRequestPreProcessor<PingPreProcessor>();
                //cfg.AddRequestPostProcessor<PingPongPostProcessor>();
                //cfg.AddOpenBehavior(typeof(GenericBehavior<,>));
                .AddOpenBehavior(typeof(PopValidationBehavior<,>))
            );

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var mediator = serviceProvider.GetService<IMediator>();

            // Act
            Func<Task> act = () => mediator.Send(new TestCommand(30));

            // Assert
            await act.Should().ThrowAsync<PopValidationHttpException>();
        }
    }
}