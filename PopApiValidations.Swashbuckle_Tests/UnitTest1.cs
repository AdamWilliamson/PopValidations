using ApiValidations;
using ApprovalTests;
using Microsoft.AspNetCore.Mvc;
using PopApiValidations.Swashbuckle_Tests.Helpers;
using PopValidations;
using Formatting = Newtonsoft.Json.Formatting;

namespace PopApiValidations.Swashbuckle_Tests;

public class UnitTest1
{
    [Fact]
    public async Task Create_Basic_Validation()
    {
        // Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(nameof(TestController.Create));

        // Act
        var validator = new TestControllerValidation();
        validator.DescribeFunc(x => x.Create(validator.Param.Is<Request>().IsNotNull()));

        var helper = await controllerTester.GetHelper<ActionResult<Response>>(
            config,
            nameof(TestController.Create),
            "/api/Test",
            validator
        );

        helper.Builder.ParamIs<Request>("request").IsNotNull();

        //Assert
        Approvals.AssertEquals(helper.CleanContent.ToString(Formatting.Indented), helper.ParsedContent.ToString(Formatting.Indented));
    }

    [Fact]
    public async Task GetById_Basic_Validation()
    {
        // Arrange
        string methodName = nameof(TestController.GetById);
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(methodName);

        // Act
        var validator = new TestControllerValidation();
        validator.DescribeFunc(x => x.GetById(validator.Param.Is<int?>().IsNotNull()));

        var helper = await controllerTester.GetHelper<ActionResult<Response>>(
            config,
            nameof(TestController.GetById),
            "/api/Test/{id}",
            validator
        );

        helper.Builder.ParamIs<Request>("id").IsNotNull();

        //Assert
        Approvals.AssertEquals(helper.CleanContent.ToString(Formatting.Indented), helper.ParsedContent.ToString(Formatting.Indented));
    }

    [Fact]
    public async Task CreateByUrl_Basic_Validation()
    {
        // Arrange
        string methodName = nameof(TestController.CreateByUrl);
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(methodName);

        // Act
        var validator = new TestControllerValidation();
        validator.DescribeFunc(x => x.CreateByUrl(
            validator.Param.Is<int?>().IsNotNull(),
            validator.Param.Is<string>().IsNotNull(),
            validator.Param.Is<List<int>>().IsNotNull()
        ));

        var helper = await controllerTester.GetHelper<ActionResult<Response>>(
            config,
            nameof(TestController.CreateByUrl),
            "/api/Test/CreateByUrl/{id}/{stringField}/{listOfIntField}",
            validator
        );

        helper.Builder.ParamIs<Request>("id").IsNotNull();
        helper.Builder.ParamIs<Request>("stringField").IsNotNull();
        helper.Builder.ParamIs<Request>("listOfIntField").IsNotNull();

        //Assert
        Approvals.AssertEquals(helper.CleanContent.ToString(Formatting.Indented), helper.ParsedContent.ToString(Formatting.Indented));
    }

    [Fact]
    public async Task CreateByQuery_Basic_Validation()
    {
        // Arrange
        string methodName = nameof(TestController.CreateByQuery);
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(methodName);

        // Act
        var validator = new TestControllerValidation();
        validator.DescribeFunc(x => x.CreateByQuery(validator.Param.Is<Request>().IsNotNull()));

        var helper = await controllerTester.GetHelper<ActionResult<Response>>(
            config,
            nameof(TestController.CreateByQuery),
            "/api/Test/CreateByQuery",
            validator
        );

        helper.Builder.ParamIs<Request>("request").IsNotNull();

        //Assert
        Approvals.AssertEquals(helper.CleanContent.ToString(Formatting.Indented), helper.ParsedContent.ToString(Formatting.Indented));
    }

    [Fact]
    public async Task CreateByBody_Basic_Validation()
    {
        // Arrange
        string methodName = nameof(TestController.CreateByBody);
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(methodName);

        // Act       
        var validator = new TestControllerValidation();
        validator.DescribeFunc(x => x.CreateByBody(validator.Param.Is<Request>().IsNotNull()));

        var helper = await controllerTester.GetHelper<ActionResult<Response>>(
            config,
            nameof(TestController.CreateByBody),
            "/api/Test/CreateByBody",
            validator
        );

        helper.Builder.ParamIs<Request>("request").IsNotNull();

        //Assert
        Approvals.AssertEquals(helper.CleanContent.ToString(Formatting.Indented), helper.ParsedContent.ToString(Formatting.Indented));
    }

    [Fact]
    public async Task Update_Basic_Validation()
    {
        // Arrange
        string methodName = nameof(TestController.Update);
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = (m) => m == typeof(TestController).GetMethod(methodName);

        // Act
        var validator = new TestControllerValidation();
        validator.DescribeFunc(x => x.Update(validator.Param.Is<Request>().IsNotNull()));

        var helper = await controllerTester.GetHelper<ActionResult<Response>>(
            config,
            nameof(TestController.Update),
            "/api/Test",
            validator
        );

        helper.Builder.ParamIs<Request>("request").IsNotNull();

        //Assert
        Approvals.AssertEquals(helper.CleanContent.ToString(Formatting.Indented), helper.ParsedContent.ToString(Formatting.Indented));
    }
}