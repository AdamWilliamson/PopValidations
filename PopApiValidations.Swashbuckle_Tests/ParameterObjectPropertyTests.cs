using ApiValidations;
using ApprovalTests;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PopApiValidations.Swashbuckle_Tests.Helpers;
using PopValidations;
using System.Reflection;

namespace PopApiValidations.Swashbuckle_Tests;

public class ParameterObjectPropertyTests
{
    [Theory]
    [MemberData(nameof(ParamAndChildObjectSetups))]
    public async Task Param_AndChildObject_Validation(
        string description,
        string route,
        string methodName,
        Func<MethodInfo, bool> validateEndpointFunc,
        Func<TestControllerValidation> createValidation,
        Action<OpenApiHelper> validationBuilder
        )
    {
        // Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = validateEndpointFunc;

        // Act
        var validator = createValidation.Invoke();

        var helper = await controllerTester.GetHelper<ActionResult<Response>>(
            config,
            methodName,
            route,
            validator
        );

        validationBuilder.Invoke(helper);

        //Assert
        Assert.NotEmpty(description);
        Approvals.AssertEquals(helper.CleanContent.ToString(Formatting.Indented), helper.ParsedContent.ToString(Formatting.Indented));
    }

    public static IEnumerable<object[]> ParamAndChildObjectSetups()
    {
        yield return new object[] {
            "Create Post Request.IntegerField are not null",
            "/api/Test",
            nameof(TestController.Create),
            (MethodInfo m) => m == typeof(TestController).GetMethod(nameof(TestController.Create)),
            () =>
            {
                var subValidator = new TestSubValidation<Request>();
                subValidator.Describe(x => x.IntegerField).IsNotNull();

                var validator = new TestControllerValidation();
                validator.DescribeFunc(x => x.Create(validator.Param.Is<Request>().IsNotNull().SetValidator(subValidator)));
                return validator;
            },
            (OpenApiHelper helper) =>
            {
                helper.Builder.ParamIs<Request>(["request"]).IsNotNull();
                helper.Builder.ParamIs<Request>(["request", "integerField"]).IsNotNull();
            }
        };

        yield return new object[] {
            "Create Post Request.SubRequestField.IntegerField are not null",
            "/api/Test",
            nameof(TestController.Create),
            (MethodInfo m) => m == typeof(TestController).GetMethod(nameof(TestController.Create)),
            () =>
            {
                var subRequestFieldValidator = new TestSubValidation<SubRequest>();
                subRequestFieldValidator.Describe(x => x.IntegerField).IsNotNull();

                var subValidator = new TestSubValidation<Request>();
                subValidator.Describe(x => x.SubRequestField).IsNotNull().SetValidator(subRequestFieldValidator);

                var validator = new TestControllerValidation();
                validator.DescribeFunc(x => x.Create(validator.Param.Is<Request>().IsNotNull().SetValidator(subValidator)));
                return validator;
            },
            (OpenApiHelper helper) =>
            {
                helper.Builder.ParamIs<Request>(["request"]).IsNotNull();
                helper.Builder.ParamIs<Request>(["request", "subRequestField"]).IsNotNull();
                helper.Builder.ParamIs<Request>(["request", "subRequestField", "integerField"]).IsNotNull();
            }
        };

        yield return new object[] {
            "Create Post Request.SubRequestField.IntegerField.DataItemField are not null",
            "/api/Test",
            nameof(TestController.Create),
            (MethodInfo m) => m == typeof(TestController).GetMethod(nameof(TestController.Create)),
            () =>
            {
                var subRequestDataItemValidator = new TestSubValidation<RequestDataItem>();
                subRequestDataItemValidator.Describe(x => x.Identifier).IsNotNull();

                var subRequestFieldValidator = new TestSubValidation<SubRequest>();
                subRequestFieldValidator.Describe(x => x.IntegerField).IsNotNull();
                subRequestFieldValidator.Describe(x => x.DataItemField)
                    .IsNotNull()
                    .SetValidator(subRequestDataItemValidator)
                    ;

                var requestDataItemValidator = new TestSubValidation<RequestDataItem>();
                requestDataItemValidator.Describe(x => x.Identifier).IsNotNull();

                var subValidator = new TestSubValidation<Request>();
                subValidator.Describe(x => x.IntegerField).IsNotNull();
                subValidator.Describe(x => x.DataItemField)
                    .IsNotNull()
                    .SetValidator(requestDataItemValidator)
                ;
                subValidator.Describe(x => x.SubRequestField)
                    .IsNotNull()
                    .SetValidator(subRequestFieldValidator);

                var validator = new TestControllerValidation();
                validator.DescribeFunc(x => x.Create(validator.Param.Is<Request>()
                    .IsNotNull()
                    .SetValidator(subValidator)));
                return validator;
            },
            (OpenApiHelper helper) =>
            {
                helper.Builder.ParamIs<Request>(["request"]).IsNotNull();
                helper.Builder.ParamIs<Request>(["request", "subRequestField"]).IsNotNull();
                helper.Builder.ParamIs<Request>(["request", "integerField"]).IsNotNull();
                helper.Builder.ParamIs<Request>(["request", "dataItemField"]).IsNotNull();
                helper.Builder.ParamIs<Request>(["request", "dataItemField", "identifier"]).IsNotNull();

                
                helper.Builder.ParamIs<Request>(["request", "subRequestField", "integerField"]).IsNotNull();

                helper.Builder.ParamIs<Request>(["request", "subRequestField", "dataItemField"]).IsNotNull();
                helper.Builder.ParamIs<Request>(["request", "subRequestField", "dataItemField", "identifier"]).IsNotNull();
            }
        };
        

        // TODO:   Array time bitches?
    }
}
