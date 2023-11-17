using FluentAssertions;
using PopValidations.FieldDescriptors.Base;
using Xunit;

namespace PopValidations_Tests.PublicAccessTests
{
    public class FieldDescriptor_PublicAccessTests
    {
        [Fact]
        public void FieldDecorator_OnlyHasTheAllowedFunctions()
        {
            // Arrange
            var allowedPublicItems = new string[]
            {
                "AddSubValidator",
                "AddSelfDescribingEntity",
                "NextValidationIsVital",
                "AddValidation"
            };

            // Act
            var foundPublicItems = TypeHelper.GetPubliclyAccessibleValuesFromType(typeof(IFieldDescriptor<,>));

            // Assert
            allowedPublicItems.Should().Equal(foundPublicItems);
        }
    }
}
