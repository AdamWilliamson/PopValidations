using PopApiValidations.Swashbuckle_Tests.Helpers;
using PopApiValidations.Swashbuckle_Tests.ValidationModificationTests.TestClasses;
using System.Reflection;

namespace PopApiValidations.Swashbuckle_Tests.ValidationModificationTests.BasicDataTypeTests;

public abstract class BasicReturnDataTypeTestsBase<TType>
{
    //static bool AreMethodsEqual(MethodInfo method1, MethodInfo method2)
    //{
    //    if (method1 == null || method2 == null)
    //        return false;

    //    // Check method name, declaring type, return type, and parameters
    //    if (method1.Name != method2.Name)
    //        return false;

    //    if (method1.DeclaringType != method2.DeclaringType)
    //        return false;

    //    if (method1.ReturnType != method2.ReturnType)
    //        return false;

    //    // Compare parameters count and types
    //    var parameters1 = method1.GetParameters();
    //    var parameters2 = method2.GetParameters();

    //    if (parameters1.Length != parameters2.Length)
    //        return false;

    //    for (int i = 0; i < parameters1.Length; i++)
    //    {
    //        if (parameters1[i].ParameterType != parameters2[i].ParameterType)
    //            return false;
    //    }

    //    return true;
    //}

    public async Task<ApiValidationBuilder> SUT(MethodInfo EndPointToValidate, string url)
    {
        var controllerTester = new PopApiControllerValidationTestBuilder<GenericVariantsController<TType>, IntVariantsControllerValidator<TType>>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint =
            (m) => m.Equals(EndPointToValidate);

        var validator = new IntVariantsControllerValidator<TType>();

        var builder = await controllerTester.GetBuilder<TType>(
            config,
            EndPointToValidate.Name,
            "/api/GenericVariantsController" + url,
            validator
        );

        return builder;
    }

    [Fact]
    public async Task ValidatingGet_WhenCheckingForValidation_ItFindsIt()
    {
        // Arrange
        var sut = await SUT(
            typeof(GenericVariantsController<TType>).GetMethod(nameof(GenericVariantsController<TType>.Get)),
            string.Empty
        );

        // Act
        sut.ReturnIs<TType>().IsNotNull();

        //Assert
        sut.Validate();
    }

    [Fact]
    public async Task ValidatingPost_WhenCheckingForValidation_ItFindsIt()
    {
        // Arrange
        var sut = await SUT(
            typeof(GenericVariantsController<TType>).GetMethod(nameof(GenericVariantsController<TType>.Post)),
            string.Empty
        );

        // Act
        sut.ReturnIs<TType>().IsNotNull();

        //Assert
        sut.Validate();
    }

    [Fact]
    public async Task ValidatingPut_WhenCheckingForValidation_ItFindsIt()
    {
        // Arrange
        var sut = await SUT(
            typeof(GenericVariantsController<TType>).GetMethod(nameof(GenericVariantsController<TType>.Put)),
            string.Empty
        );

        // Act
        sut.ReturnIs<TType>().IsNotNull();

        //Assert
        sut.Validate();
    }

    [Fact]
    public async Task ValidatingDelete_WhenCheckingForValidation_ItFindsIt()
    {
        // Arrange
        var sut = await SUT(
            typeof(GenericVariantsController<TType>).GetMethod(nameof(GenericVariantsController<TType>.Delete)),
            string.Empty
        );

        // Act
        sut.ReturnIs<TType>().IsNotNull();

        //Assert
        sut.Validate();
    }

    [Fact]
    public async Task ValidatingPatch_WhenCheckingForValidation_ItFindsIt()
    {
        // Arrange
        var sut = await SUT(
            typeof(GenericVariantsController<TType>).GetMethod(nameof(GenericVariantsController<TType>.Patch)),
            string.Empty
        );

        // Act
        sut.ReturnIs<TType>().IsNotNull();

        //Assert
        sut.Validate();
    }

    [Fact]
    public async Task ValidatingHead_WhenCheckingForValidation_ItFindsIt()
    {
        // Arrange
        var sut = await SUT(
            typeof(GenericVariantsController<TType>).GetMethod(nameof(GenericVariantsController<TType>.Head)),
            string.Empty
        );

        // Act
        sut.ReturnIs<TType>().IsNotNull();

        //Assert
        sut.Validate();
    }

    [Fact]
    public async Task ValidatingOptions_WhenCheckingForValidation_ItFindsIt()
    {
        // Arrange
        var sut = await SUT(
            typeof(GenericVariantsController<TType>).GetMethod(nameof(GenericVariantsController<TType>.Options)),
            string.Empty
        );

        // Act
        sut.ReturnIs<TType>().IsNotNull();

        //Assert
        sut.Validate();
    }
}