using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PopValidations.MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations.MediatR_Tests.MultiValidator
{
    public record TestCommand(int Id) : IRequest;

    public class TestInputValidator1 : AbstractValidator<TestCommand>
    {
        public TestInputValidator1()
        {
            Describe(x => x.Id).IsLessThan(20);
        }
    }

    public class TestInputValidator2 : AbstractValidator<TestCommand>
    {
        public TestInputValidator2()
        {
            Describe(x => x.Id).IsGreaterThan(10);
        }
    }

    public class TestCommandHandler : IRequestHandler<TestCommand>
    {
        public Task Handle(TestCommand request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public class MultiValidatorTests
    {
        [Theory]
        [InlineData(30)]
        [InlineData(10)]
        public async Task GivenASingleValidator_WhenSetupInCorrectly_ItCorrectlyFails(int value)
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
            Func<Task> act = () => mediator.Send(new TestCommand(value));

            // Assert
            await act.Should().ThrowAsync<PopValidationHttpException>();
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
            Func<Task> act = () => mediator.Send(new TestCommand(15));

            // Assert
            await act.Should().NotThrowAsync();
        }
    }
}