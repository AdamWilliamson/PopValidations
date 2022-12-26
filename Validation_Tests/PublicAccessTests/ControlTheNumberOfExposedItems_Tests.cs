//using FluentAssertions;
//using FluentAssertions.Execution;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using Validations;
//using PopValidations.Internal;
//using PopValidations.Scopes;
//using Xunit;

//namespace Validations_Tests
//{
//    public class ControlTheNumberOfExposedItems_Tests
//    {
//        protected List<string> GetAllPublicItems(Type type)
//        {
//            var allPublicFunctions = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Select(m => m.Name).ToList();
//            var allPublicProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Select(p => p.Name).ToList();
//            return allPublicFunctions.Concat(allPublicProperties).ToList();
//        }

//        protected List<string> GetAllNonPublicItems(Type type)
//        {
//            var allProtectedFunctions = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).Select(m => m.Name).ToList();
//            var allProtectedProperties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).Select(p => p.Name).ToList();
//            return allProtectedFunctions.Concat(allProtectedProperties).ToList();
//        }

//        protected List<string> GetAllStaticItems(Type type)
//        {
//            var staticFunctions = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).Select(m => m.Name).ToList();
//            var staticProperties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).Select(m => m.Name).ToList();
//            return staticFunctions.Concat(staticProperties).ToList();
//        }

//        [Fact]
//        public void AbstractValidator_OnlyHasTheExpectedFunctions()
//        {
//            // Arrange
//            var validPublicNames = new string[]
//            {
//                nameof(IScopedValidator<AllFieldTypesDto>.GetScope),
//            };

//            var validProtectedNames = new string[]
//            {
//                "Describe",
//                "Include",
//                "Scope"
//            };

//            var type = typeof(AbstractValidator<>);

//            // Act
//            // Assert
//            using (new AssertionScope())
//            {
//                GetAllPublicItems(type).Should().BeEquivalentTo(validPublicNames);
//                GetAllNonPublicItems(type).Should().BeEquivalentTo(validProtectedNames);
//                GetAllStaticItems(type).Should().BeEquivalentTo(new string[] { "Equals", "ReferenceEquals" });
//            }
//        }

//        [Fact]
//        public void FieldChainValidator_OnlyHasTheExpectedFunctions()
//        {
//            // Arrange
//            var validPublicNames = new string[]
//            {
//                "get_" + nameof(IFieldDescriptor<AllFieldTypesDto>.Property),
//                nameof(IFieldDescriptor<AllFieldTypesDto>.Property),
//                nameof(IFieldDescriptor<AllFieldTypesDto>.Matches),
//                nameof(IFieldDescriptor<AllFieldTypesDto>.AddValidator),
//                nameof(IFieldDescriptor<AllFieldTypesDto>.GetValidations),
//                nameof(IFieldDescriptor<AllFieldTypesDto>.GetPropertyValue),
//            };

//            var validProtectedNames = new string[]
//            {
//                "Validations",
//                "get_Validations"
//            };

//            var type = typeof(FieldDescriptor<AllFieldTypesDto, int>);

//            // Act
//            // Assert
//            using (new AssertionScope())
//            {
//                GetAllPublicItems(type).Should().BeEquivalentTo(validPublicNames);
//                GetAllNonPublicItems(type).Should().BeEquivalentTo(validProtectedNames);
//                GetAllStaticItems(type).Should().BeEquivalentTo(new string[] { "Equals", "ReferenceEquals" });
//            }
//        }

//        [Fact]
//        public void Runner_OnlyHasTheExpectedFunctions()
//        {
//            // Arrange
//            var validPublicNames = new string[]
//            {
//                nameof(IValidationRunner<AllFieldTypesDto>.Validate),
//                nameof(IValidationRunner<AllFieldTypesDto>.Describe)
//            };

//            var type = typeof(ValidationRunner<AllFieldTypesDto>);

//            // Act
//            // Assert
//            using (new AssertionScope())
//            {
//                GetAllPublicItems(type).Should().BeEquivalentTo(validPublicNames);
//                GetAllStaticItems(type).Should().BeEquivalentTo(new string[] { "Equals", "ReferenceEquals" });
//            }
//        }

//        [Fact]
//        public void ScopeData_OnlyHasTheExpectedFunctions()
//        {
//            // Arrange
//            var validPublicNames = new string[]
//            {
//                nameof(ScopedData<int, int>.Describe),
//                nameof(ScopedData<int, int>.GetValue)
//            };

//            var validProtectedNames = new string[]
//            {
//                "PassThroughFunction",
//                "get_PassThroughFunction",
//                "Description",
//                "get_Description",
//                "Result",
//                "get_Result"
//            };

//            var type = typeof(ScopedData<int, int>);

//            // Act
//            // Assert
//            using (new AssertionScope())
//            {
//                GetAllPublicItems(type).Should().BeEquivalentTo(validPublicNames);
//                GetAllNonPublicItems(type).Should().BeEquivalentTo(validProtectedNames);
//                GetAllStaticItems(type).Should().BeEquivalentTo(new string[] { "Equals", "ReferenceEquals" });
//            }
//        }

//        [Fact]
//        public void ValidationScope_OnlyHasTheExpectedFunctions()
//        {
//            // Arrange
//            var validPublicNames = new string[]
//            {
//                nameof(ValidationScope<AllFieldTypesDto>.GetFieldValidators),
//                nameof(ValidationScope<AllFieldTypesDto>.AddFieldDescriptor),
//                nameof(ValidationScope<AllFieldTypesDto>.Include),
//                nameof(ValidationScope<AllFieldTypesDto>.AddValidatableObject),
//                nameof(ValidationScope<AllFieldTypesDto>.GetFieldDescriptor)
//            };

//            var type = typeof(ValidationScope<AllFieldTypesDto>);

//            // Act
//            // Assert
//            using (new AssertionScope())
//            {
//                GetAllPublicItems(type).Should().BeEquivalentTo(validPublicNames);
//            }
//        }
//    }
//}
