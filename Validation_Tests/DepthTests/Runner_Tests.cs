using ApprovalTests;
using FluentAssertions;
using FluentAssertions.Execution;
using PopValidations_Tests.TestHelpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations_Tests.DepthTests
{
    public class Runner_Tests
    {
        [Fact]
        public async Task GivenASuccessfulObject_ItShouldProduceNoValidationErrors()
        {
            // Arrange
            var validationRunner = ValidationRunnerHelper.BasicRunnerSetup(new Level1Validator());

            // Act
            var validationResult = await validationRunner.Validate(
                new Level1(
                    FieldObjectBuilder.CreateValidTestObject(nameof(Level1)),
                    new Level2(
                        FieldObjectBuilder.CreateValidTestObject(nameof(Level2)),
                        new Level3(FieldObjectBuilder.CreateValidTestObject(nameof(Level3))),
                        new List<Level3>()
                        {
                            new Level3(FieldObjectBuilder.CreateValidTestObject(nameof(Level3))),
                            new Level3(FieldObjectBuilder.CreateValidTestObject(nameof(Level3))),
                            new Level3(FieldObjectBuilder.CreateValidTestObject(nameof(Level3)))
                        }),
                    new List<Level2>()
                    {
                        new Level2(
                            FieldObjectBuilder.CreateValidTestObject(nameof(Level2)),
                            new Level3(FieldObjectBuilder.CreateValidTestObject(nameof(Level3))),
                            new List<Level3>()
                                {
                                    new Level3(FieldObjectBuilder.CreateValidTestObject(nameof(Level3))),
                                    new Level3(FieldObjectBuilder.CreateValidTestObject(nameof(Level3))),
                                    new Level3(FieldObjectBuilder.CreateValidTestObject(nameof(Level3)))
                                }
                        ),
                        new Level2(
                                FieldObjectBuilder.CreateValidTestObject(nameof(Level2)),
                            new Level3(FieldObjectBuilder.CreateValidTestObject(nameof(Level3))),
                            new List<Level3>()
                                {
                                    new Level3(FieldObjectBuilder.CreateValidTestObject(nameof(Level3))),
                                    new Level3(FieldObjectBuilder.CreateValidTestObject(nameof(Level3))),
                                    new Level3(FieldObjectBuilder.CreateValidTestObject(nameof(Level3)))
                                }
                        ),
                        new Level2(
                                FieldObjectBuilder.CreateValidTestObject(nameof(Level2)),
                            new Level3(FieldObjectBuilder.CreateValidTestObject(nameof(Level3))),
                            new List<Level3>()
                                {
                                    new Level3(FieldObjectBuilder.CreateValidTestObject(nameof(Level3))),
                                    new Level3(FieldObjectBuilder.CreateValidTestObject(nameof(Level3))),
                                    new Level3(FieldObjectBuilder.CreateValidTestObject(nameof(Level3)))
                                }
                        ),
                    }
                )
            );

            // Assert
            Approvals.VerifyJson(JsonConverter.ToJson(validationResult));
        }

        [Fact]
        public async Task GivenASuccessfulObject_ItShouldProduceEveryValidationError()
        {
            // Arrange
            var validationRunner = ValidationRunnerHelper.BasicRunnerSetup(new Level1Validator());

            // Act
            var validationResult = await validationRunner.Validate(
                new Level1(
                    FieldObjectBuilder.CreateFailingObject(nameof(Level1Validator)),
                    new Level2(
                        FieldObjectBuilder.CreateFailingObject(nameof(Level2Validator)),
                        new Level3(FieldObjectBuilder.CreateFailingObject(nameof(Level3Validator))),
                        new List<Level3>()
                        {
                            new Level3(FieldObjectBuilder.CreateFailingObject(nameof(Level3Validator))),
                            new Level3(FieldObjectBuilder.CreateFailingObject(nameof(Level3Validator))),
                            new Level3(FieldObjectBuilder.CreateFailingObject(nameof(Level3Validator)))
                        }),
                    new List<Level2>()
                        {
                            new Level2(
                                FieldObjectBuilder.CreateFailingObject(nameof(Level2Validator)),
                                new Level3(FieldObjectBuilder.CreateFailingObject(nameof(Level3Validator))),
                                new List<Level3>()
                                    {
                                        new Level3(FieldObjectBuilder.CreateFailingObject(nameof(Level3Validator))),
                                        new Level3(FieldObjectBuilder.CreateFailingObject(nameof(Level3Validator))),
                                        new Level3(FieldObjectBuilder.CreateFailingObject(nameof(Level3Validator)))
                                    }
                            ),
                            new Level2(
                                  FieldObjectBuilder.CreateFailingObject(nameof(Level2Validator)),
                                new Level3(FieldObjectBuilder.CreateFailingObject(nameof(Level3Validator))),
                                new List<Level3>()
                                    {
                                        new Level3(FieldObjectBuilder.CreateFailingObject(nameof(Level3Validator))),
                                        new Level3(FieldObjectBuilder.CreateFailingObject(nameof(Level3Validator))),
                                        new Level3(FieldObjectBuilder.CreateFailingObject(nameof(Level3Validator)))
                                    }
                            ),
                            new Level2(
                                  FieldObjectBuilder.CreateFailingObject(nameof(Level2Validator)),
                                new Level3(FieldObjectBuilder.CreateFailingObject(nameof(Level3Validator))),
                                new List<Level3>()
                                    {
                                        new Level3(FieldObjectBuilder.CreateFailingObject(nameof(Level3Validator))),
                                        new Level3(FieldObjectBuilder.CreateFailingObject(nameof(Level3Validator))),
                                        new Level3(FieldObjectBuilder.CreateFailingObject(nameof(Level3Validator)))
                                    }
                            ),
                        }
                    )
            );

            // Assert
            Approvals.VerifyJson(JsonConverter.ToJson(validationResult));
            using (new AssertionScope()) 
            {
                foreach (var validation in ValidationHelper.GetAllValidatorsNames())
                {
                    validationResult.Errors.Any(x => x.Key.Contains(validation)).Should().BeTrue(validation);
                }
            }
        }

        [Fact]
        public void Description()
        {
            // Arrange
            var descriptionRunner = ValidationRunnerHelper.BasicRunnerSetup(new Level1Validator());

            // Act
            var descriptionResult = descriptionRunner.Describe();

            // Assert
            Approvals.VerifyJson(JsonConverter.ToJson(descriptionResult));
            using (new AssertionScope())
            {
                foreach (var validation in ValidationHelper.GetAllValidatorsNames())
                {
                    descriptionResult.Results.Any(x => x.Property.Contains(validation)).Should().BeTrue(validation);
                }
            }
        }
    }
}
