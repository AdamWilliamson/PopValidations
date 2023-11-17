using FluentAssertions;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations;
using Xunit;

namespace PopValidations_Tests.PublicAccessTests
{
    public class SwitchValidator_PublicAccessTests
    {
        [Fact]
        public void SwitchValidator_OnlyHasTheAllowedFunctions()
        {
            // Arrange
            var allowedPublicItems = new string[]
            {
                "Case",
                "Ignore"
            };

            // Act
            var foundPublicItems = TypeHelper.GetPubliclyAccessibleValuesFromType(typeof(ISwitchValidator<,>));

            // Assert
            allowedPublicItems.Should().Equal(foundPublicItems);
        }
    }
}
