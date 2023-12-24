using ApprovalTests;
using ApprovalTests.Namers;
using FluentAssertions;
using FluentAssertions.Execution;
using PopValidations;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations;
using PopValidations.ValidatorInternals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Xunit;

namespace PopValidations_Tests.PublicAccessTests;

public record TestObject();

public class AbstractValidator_PublicAccessTests
{
    static readonly List<String> Ignore = new()
    {
            "Equals", "ReferenceEquals", "GetType", "ToString", "GetHashCode", "MemberwiseClone", "Finalize"
    };

    [Fact]
    public void AbstractValidator_OnlyHasTheExpectedFunctions()
    {
        // Arrange
        var type = typeof(AbstractValidator<>);

        var props =
            new List<string>() { "------------ Ignore -------------" }
            .Concat(Ignore)
            .Concat(new List<string>() { "------------ Public Methods -------------" })
            .Concat(
                type
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Where(x => !x.IsPrivate)
                    .Where(x => !Ignore.Contains(x.Name))
                    .Select(InfoToString)
            )
            .Concat(new List<string>() { "------------ Protected Methods -------------" })
            .Concat(
                type
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Where(x => !x.IsPrivate)
                    .Where(x => !Ignore.Contains(x.Name))
                    .Select(InfoToString)
            )
            .Concat(new List<string>() { "------------ Public Properties -------------" })
            .Concat(
                type
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Where(IsNotHidden)
                    .Where(x => !Ignore.Contains(x.Name))
                    .Select(InfoToString)
            )
            .Concat(new List<string>() { "------------ Protected Properties -------------" })
            .Concat(
                type
                    .GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Where(IsNotHidden)
                    .Where(x => !Ignore.Contains(x.Name))
                    .Select(InfoToString)
            )
            .Concat(new List<string>() { "------------ Public Fields -------------" })
            .Concat(
                type
                    .GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Where(IsNotHidden)
                    .Where(x => !Ignore.Contains(x.Name))
                    .Select(InfoToString)
            )
            .Concat(new List<string>() { "------------ Protected Fields -------------" })
            .Concat(
                type
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Where(IsNotHidden)
                    .Where(x => !Ignore.Contains(x.Name))
                    .Select(InfoToString)
            );

        // Act
        // Assert
        using (ApprovalResults.ForScenario(RuntimeInformation.FrameworkDescription))
        {
            Approvals.Verify(
                string.Join(Environment.NewLine + "-", props)
            );
        }
    }

    [Fact]
    public void AbstractSubValidator_OnlyHasTheExpectedFunctions()
    {
        // Arrange
        var type = typeof(AbstractSubValidator<>);

        var props =
            new List<string>() { "------------ Ignore -------------" }
            .Concat(Ignore)
            .Concat(new List<string>() { "------------ Public Methods -------------" })
            .Concat(
                type
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Where(x => !x.IsPrivate)
                    .Where(x => !Ignore.Contains(x.Name))
                    .Select(InfoToString)
            )
            .Concat(new List<string>() { "------------ Protected Methods -------------" })
            .Concat(
                type
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Where(x => !x.IsPrivate)
                    .Where(x => !Ignore.Contains(x.Name))
                    .Select(InfoToString)
            )
            .Concat(new List<string>() { "------------ Public Properties -------------" })
            .Concat(
                type
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Where(IsNotHidden)
                    .Where(x => !Ignore.Contains(x.Name))
                    .Select(InfoToString)
            )
            .Concat(new List<string>() { "------------ Protected Properties -------------" })
            .Concat(
                type
                    .GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Where(IsNotHidden)
                    .Where(x => !Ignore.Contains(x.Name))
                    .Select(InfoToString)
            )
            .Concat(new List<string>() { "------------ Public Fields -------------" })
            .Concat(
                type
                    .GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Where(IsNotHidden)
                    .Where(x => !Ignore.Contains(x.Name))
                    .Select(InfoToString)
            )
            .Concat(new List<string>() { "------------ Protected Fields -------------" })
            .Concat(
                type
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Where(IsNotHidden)
                    .Where(x => !Ignore.Contains(x.Name))
                    .Select(InfoToString)
            );

        // Act
        // Assert
        using (ApprovalResults.ForScenario(RuntimeInformation.FrameworkDescription))
        {
            Approvals.Verify(
                string.Join(Environment.NewLine + "-", props)
            );
        }
    }

    [Fact]
    public void AbstractValidator_Describe_ReturnsTheCorrectType()
    {
        // Arrange
        var type = typeof(AbstractValidator<>);

        var validatorDescribes = type
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
            .Where(x => x.Name == nameof(AbstractValidator<object>.Describe) || x.Name == nameof(AbstractValidator<object>.DescribeEnumerable));

        // Act
        // Assert
        validatorDescribes.All(x => x.ReturnType.GetGenericTypeDefinition() == typeof(IFieldDescriptor<,>)).Should().BeTrue();
    }

    [Fact]
    public void AbstractSubValidator_Describe_ReturnsTheCorrectType()
    {
        // Arrange
        var subType = typeof(AbstractSubValidator<>);

        var subValidatorDescribes = subType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
            .Where(x => x.Name == nameof(AbstractSubValidator<object>.Describe) || x.Name == nameof(AbstractSubValidator<object>.DescribeEnumerable));

        // Act
        // Assert
        subValidatorDescribes.All(x => x.ReturnType.GetGenericTypeDefinition() == typeof(IFieldDescriptor<,>)).Should().BeTrue();
    }

    [Fact]
    public void AbstractValidator_Switch_ReturnsTheCorrectType()
    {
        // Arrange
        var type = typeof(AbstractValidator<>);

        var validatorDescribes = type
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
            .Where(x => x.Name == nameof(AbstractValidator<object>.Switch));

        // Act
        // Assert
        validatorDescribes.All(x => x.ReturnType.GetGenericTypeDefinition() == typeof(ISwitchValidator<,>)).Should().BeTrue();
    }

    [Fact]
    public void AbstractSubValidator_Switch_ReturnsTheCorrectType()
    {
        // Arrange
        var subType = typeof(AbstractSubValidator<>);

        var subValidatorDescribes = subType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
            .Where(x => x.Name == nameof(AbstractSubValidator<object>.Switch));

        // Act
        // Assert
        subValidatorDescribes.All(x => x.ReturnType.GetGenericTypeDefinition() == typeof(ISwitchValidator<,>)).Should().BeTrue();
    }

    private static bool IsNotHidden(PropertyInfo propertyInfo)
    {
        return !propertyInfo.Name.StartsWith("PopValidations.");
    }

    private static bool IsNotHidden(FieldInfo fieldInfo)
    {
        return !fieldInfo.Name.StartsWith("PopValidations.");
    }

    private static string InfoToString(MethodInfo methodInfo)
    {
        return $"{methodInfo.Name} | {string.Join(',', methodInfo.Attributes.ToString())}";
    }

    private static string InfoToString(PropertyInfo propertyInfo)
    {
        return $"{propertyInfo.Name} | {string.Join(',', propertyInfo.Attributes.ToString())}";
    }

    private static string InfoToString(FieldInfo fieldInfo)
    {
        return $"{fieldInfo.Name} | {string.Join(',', fieldInfo.Attributes.ToString())}";
    }
}