using PopApiValidations.Swashbuckle_Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using ApprovalTests;

namespace PopApiValidations.Swashbuckle_Tests.ValidationModificationTests;

public abstract class ValidationConverterTestBase
{
    [Fact]
    public async virtual Task ParameterValidated()
    {
        // Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(nameof(TestController.GetById));

        // Act
        var validator = new TestControllerValidation();
        ParameterValidated_AddValidations(validator);

        var result = await controllerTester.GetHelper<ActionResult<Response>>(
            config,
            nameof(TestController.GetById),
            "/api/Test",
            validator
        );

        // Assert
        Approvals.VerifyJson(JsonCompare.FindDiffString(result.ParsedContent!, result.CleanContent!));
    }
    protected abstract void ParameterValidated_AddValidations(TestControllerValidation validator);

    [Fact]
    public async virtual Task ParameterListObjectValidated()
    {
        // Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = 
            (m) => m == typeof(TestController).GetMethod(nameof(TestController.GetByIds));

        // Act
        var validator = new TestControllerValidation();
        ParameterListObjectValidated_AddValidations(validator);

        var result = await controllerTester.GetHelper<ActionResult<IEnumerable<Response>>>(
            config,
            nameof(TestController.GetByIds),
            "/api/Test/GetByIds",
            validator
        );

        // Assert
        Approvals.VerifyJson(JsonCompare.FindDiffString(result.ParsedContent!, result.CleanContent!));
    }
    protected abstract void ParameterListObjectValidated_AddValidations(TestControllerValidation validator);

    [Fact]
    public async virtual Task ParameterDeeperObjectValidated()
    {
        // Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(nameof(TestController.CreateByQuery));

        // Act
        var validator = new TestControllerValidation();
        ParameterDeeperObjectValidated_AddValidations(validator);

        var result = await controllerTester.GetHelper<ActionResult<Response>>(
            config,
            nameof(TestController.CreateByQuery),
            "/api/Test/CreateByQuery",
            validator
        );

        // Assert
        Approvals.VerifyJson(JsonCompare.FindDiffString(result.ParsedContent!, result.CleanContent!));
    }
    protected abstract void ParameterDeeperObjectValidated_AddValidations(TestControllerValidation validator);

    [Fact]
    public async virtual Task ParameterDeeperListObjectValidated()
    {
        // Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(nameof(TestController.CreateByQueryMultiple));

        // Act
        var validator = new TestControllerValidation();
        ParameterDeeperListObjectValidated_AddValidations(validator);

        var result = await controllerTester.GetHelper<ActionResult<Response>>(
            config,
            nameof(TestController.CreateByQueryMultiple),
            "/api/Test/CreateByQueryMultiple",
            validator
        );

        // Assert
        Approvals.VerifyJson(JsonCompare.FindDiffString(result.ParsedContent!, result.CleanContent!));
    }
    protected abstract void ParameterDeeperListObjectValidated_AddValidations(TestControllerValidation validator);

    [Fact]
    public async virtual Task RequestBodyValidated()
    {
        // Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(nameof(TestController.Create));

        // Act
        var validator = new TestControllerValidation();
        RequestBodyValidated_AddValidations(validator);

        var result = await controllerTester.GetHelper<ActionResult<Response>>(
            config,
            nameof(TestController.Create),
            "/api/Test",
            validator
        );

        // Assert
        Approvals.VerifyJson(JsonCompare.FindDiffString(result.ParsedContent!, result.CleanContent!));
    }
    protected abstract void RequestBodyValidated_AddValidations(TestControllerValidation validator);

    [Fact]
    public async virtual Task RequestBodyListValidated()
    {
        // Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(nameof(TestController.CreateMultiple));

        // Act
        var validator = new TestControllerValidation();
        RequestBodyListValidated_AddValidations(validator);

        var result = await controllerTester.GetHelper<ActionResult<Response>>(
            config,
            nameof(TestController.CreateMultiple),
            "/api/Test/CreateMultiple",
            validator
        );

        // Assert
        Approvals.VerifyJson(JsonCompare.FindDiffString(result.ParsedContent!, result.CleanContent!));
    }
    protected abstract void RequestBodyListValidated_AddValidations(TestControllerValidation validator);

    [Fact]
    public async virtual Task RequestBodyDeeperObjectValidated()
    {
        // Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(nameof(TestController.Create));

        // Act
        var validator = new TestControllerValidation();
        RequestBodyDeeperObjectValidated_AddValidations(validator);

        var result = await controllerTester.GetHelper<ActionResult<Response>>(
            config,
            nameof(TestController.Create),
            "/api/Test",
            validator
        );

        // Assert
        Approvals.VerifyJson(JsonCompare.FindDiffString(result.ParsedContent!, result.CleanContent!));
    }
    protected abstract void RequestBodyDeeperObjectValidated_AddValidations(TestControllerValidation validator);

    [Fact]
    public async virtual Task RequestBodyDeeperListObjectValidated()
    {
        // Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(nameof(TestController.CreateMultiple));

        // Act
        var validator = new TestControllerValidation();
        RequestBodyDeeperListObjectValidated_AddValidations(validator);

        var result = await controllerTester.GetHelper<ActionResult<Response>>(
            config,
            nameof(TestController.CreateMultiple),
            "/api/Test/CreateMultiple",
            validator
        );

        // Assert
        Approvals.VerifyJson(JsonCompare.FindDiffString(result.ParsedContent!, result.CleanContent!));
    }
    protected abstract void RequestBodyDeeperListObjectValidated_AddValidations(TestControllerValidation validator);

    [Fact]
    public async virtual Task ReturnValidated()
    {
        // Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(nameof(TestController.Create));

        // Act
        var validator = new TestControllerValidation();
        ReturnValidated_AddValidations(validator);

        var result = await controllerTester.GetHelper<ActionResult<Response>>(
            config,
            nameof(TestController.Create),
            "/api/Test",
            validator
        );

        // Assert
        Approvals.VerifyJson(JsonCompare.FindDiffString(result.ParsedContent!, result.CleanContent!));
    }
    protected abstract void ReturnValidated_AddValidations(TestControllerValidation validator);

    [Fact]
    public async virtual Task ReturnListValidated()
    {
        // Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(nameof(TestController.CreateMultiple));

        // Act
        var validator = new TestControllerValidation();
        ReturnListValidated_AddValidations(validator);

        var result = await controllerTester.GetHelper<ActionResult<Response>>(
            config,
            nameof(TestController.CreateMultiple),
            "/api/Test/CreateMultiple",
            validator
        );

        // Assert
        Approvals.VerifyJson(JsonCompare.FindDiffString(result.ParsedContent!, result.CleanContent!));
    }
    protected abstract void ReturnListValidated_AddValidations(TestControllerValidation validator);

    [Fact]
    public async virtual Task ReturnDeeperObjectValidated()
    {
        // Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(nameof(TestController.Create));

        // Act
        var validator = new TestControllerValidation();
        ReturnDeeperObjectValidated_AddValidations(validator);

        var result = await controllerTester.GetHelper<Response>(
            config,
            nameof(TestController.Create),
            "/api/Test",
            validator
        );

        // Assert
        Approvals.VerifyJson(JsonCompare.FindDiffString(result.ParsedContent!, result.CleanContent!));
    }
    protected abstract void ReturnDeeperObjectValidated_AddValidations(TestControllerValidation validator);

    [Fact]
    public async virtual Task ReturnDeeperListObjectValidated()
    {
        // Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(nameof(TestController.CreateMultiple));

        // Act
        var validator = new TestControllerValidation();
        ReturnDeeperListObjectValidated_AddValidations(validator);

        var result = await controllerTester.GetHelper<ActionResult<Response>>(
            config,
            nameof(TestController.CreateMultiple),
            "/api/Test/CreateMultiple",
            validator
        );

        // Assert
        Approvals.VerifyJson(JsonCompare.FindDiffString(result.ParsedContent!, result.CleanContent!));
    }
    protected abstract void ReturnDeeperListObjectValidated_AddValidations(TestControllerValidation validator);
    //== Return Action Response
    [Fact]
    public async virtual Task ReturnActionResultValidated()
    {
        // Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(nameof(TestController.Update));

        // Act
        var validator = new TestControllerValidation();
        ReturnActionResultValidated_AddValidations(validator);

        var result = await controllerTester.GetHelper<ActionResult<Response>>(
            config,
            nameof(TestController.Update),
            "/api/Test",
            validator
        );

        // Assert
        Approvals.VerifyJson(JsonCompare.FindDiffString(result.ParsedContent!, result.CleanContent!));
    }
    protected abstract void ReturnActionResultValidated_AddValidations(TestControllerValidation validator);

    [Fact]
    public async virtual Task ReturnListActionResultValidated()
    {
        // Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(nameof(TestController.UpdateMultiple));

        // Act
        var validator = new TestControllerValidation();
        ReturnListActionResultValidated_AddValidations(validator);

        var result = await controllerTester.GetHelper<ActionResult<Response>>(
            config,
            nameof(TestController.UpdateMultiple),
            "/api/Test/UpdateMultiple",
            validator
        );

        // Assert
        Approvals.VerifyJson(JsonCompare.FindDiffString(result.ParsedContent!, result.CleanContent!));
    }
    protected abstract void ReturnListActionResultValidated_AddValidations(TestControllerValidation validator);

    [Fact]
    public async virtual Task ReturnDeeperObjectActionResultValidated()
    {
        // Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(nameof(TestController.Update));

        // Act
        var validator = new TestControllerValidation();
        ReturnDeeperObjectActionResultValidated_AddValidations(validator);

        var result = await controllerTester.GetHelper<ActionResult<Response>>(
            config,
            nameof(TestController.Update),
            "/api/Test",
            validator
        );

        // Assert
        Approvals.VerifyJson(JsonCompare.FindDiffString(result.ParsedContent!, result.CleanContent!));
    }
    protected abstract void ReturnDeeperObjectActionResultValidated_AddValidations(TestControllerValidation validator);

    [Fact]
    public async virtual Task ReturnDeeperListObjectActionResultValidated()
    {
        // Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(nameof(TestController.UpdateMultiple));

        // Act
        var validator = new TestControllerValidation();
        ReturnDeeperListObjectActionResultValidated_AddValidations(validator);

        var result = await controllerTester.GetHelper<ActionResult<Response>>(
            config,
            nameof(TestController.UpdateMultiple),
            "/api/Test/UpdateMultiple",
            validator
        );

        // Assert
        Approvals.VerifyJson(JsonCompare.FindDiffString(result.ParsedContent!, result.CleanContent!));
    }
    protected abstract void ReturnDeeperListObjectActionResultValidated_AddValidations(TestControllerValidation validator);
}