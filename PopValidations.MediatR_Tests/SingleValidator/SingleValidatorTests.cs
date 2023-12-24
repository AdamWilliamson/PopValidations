using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PopValidations.MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations.MediatR_Tests.SingleValidator
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

    public class SingleValidatorTests
    {
        [Fact]
        public async Task GivenASingleValidator_WhenSetupInCorrectly_ItCorrectlyFails()
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
            );

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var mediator = serviceProvider.GetService<IMediator>();

            // Act
            Func<Task> act = () => mediator!.Send(new TestCommand(30));

            // Assert
            await act.Should().ThrowAsync<PopValidationMediatRException>();
        }

        [Fact]
        public async Task GivenASingleValidator_WhenSetupCorrectly_ItSucceeds()
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
            );

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var mediator = serviceProvider.GetService<IMediator>();

            // Act
            Func<Task> act = () => mediator!.Send(new TestCommand(10));

            // Assert
            await act.Should().NotThrowAsync();
        }
    }
}