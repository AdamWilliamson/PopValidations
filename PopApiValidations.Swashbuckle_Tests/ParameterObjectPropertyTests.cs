using ApiValidations;
using ApprovalTests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
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
        Action<ApiValidationBuilder> validationBuilder
        )
    {
        // Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = validateEndpointFunc;

        // Act
        var validator = createValidation.Invoke();

        var builder = await controllerTester.GetBuilder<ActionResult<Response>>(
            config,
            methodName,
            route,
            validator
        );

        validationBuilder.Invoke(builder);

        //Assert
        Assert.NotEmpty(description);
        builder.Validate();

        //Approvals.AssertEquals(
        //    helper.CleanContent.ToString(Formatting.Indented), 
        //    helper.ParsedContent.ToString(Formatting.Indented)
        //);
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
                validator.DescribeFunc(x => x.Create(validator.Param.Is<Request>().SetValidator(subValidator)));
                return validator;
            },
            (ApiValidationBuilder builder) =>
            {
                builder.ParamIs<Request>(["integerField"]).IsNotNull2();
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
            (ApiValidationBuilder builder) =>
            {
                builder.ParamIs<Request>(["subRequestField"]).IsNotNull2();
                builder.ParamIs<Request>(["subRequestField", "integerField"]).IsNotNull2();
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
            (ApiValidationBuilder builder) =>
            {
                builder.ParamIs<Request>(["subRequestField"]).IsNotNull2();
                builder.ParamIs<Request>(["integerField"]).IsNotNull2();
                builder.ParamIs<Request>(["dataItemField"]).IsNotNull2();
                builder.ParamIs<Request>(["dataItemField", "identifier"]).IsNotNull2();
                builder.ParamIs<Request>(["subRequestField", "integerField"]).IsNotNull2();
                builder.ParamIs<Request>(["subRequestField", "dataItemField"]).IsNotNull2();
                builder.ParamIs<Request>(["subRequestField", "dataItemField", "identifier"]).IsNotNull2();
            }
        };

        yield return new object[] {
            "CreateByQuery Post Request.SubRequestField.IntegerField are not null",
            "/api/Test/CreateByQuery",
            nameof(TestController.CreateByQuery),
            (MethodInfo m) => m == typeof(TestController).GetMethod(nameof(TestController.CreateByQuery)),
            () =>
            {
                var subRequestFieldValidator = new TestSubValidation<SubRequest>();
                subRequestFieldValidator.Describe(x => x.IntegerField).IsNotNull();

                var subValidator = new TestSubValidation<Request>();
                subValidator.Describe(x => x.IntegerField).IsNotNull();
                subValidator.Describe(x => x.SubRequestField)
                    .SetValidator(subRequestFieldValidator);

                var validator = new TestControllerValidation();
                validator.DescribeFunc(x => x.CreateByQuery(validator.Param.Is<Request>()
                    .IsNotNull()
                    .SetValidator(subValidator)));

                return validator;
            },
            (ApiValidationBuilder builder) =>
            {
                builder.ParamIs<Request>(ParamType.FromQuery, ["SubRequestField.IntegerField"]).IsNotNull2();
                builder.ParamIs<Request>(ParamType.FromQuery, ["IntegerField"]).IsNotNull2();
            }
        };

        // TODO:   Array time bitches?
        yield return new object[] {
            "CreateByQuery Post Request.SubRequestField.IntegerField are not null",
            "/api/Test/CreateByQuery",
            nameof(TestController.CreateByQuery),
            (MethodInfo m) => m == typeof(TestController).GetMethod(nameof(TestController.CreateByQuery)),
            () =>
            {
                var subRequestFieldValidator = new TestSubValidation<SubRequest>();
                subRequestFieldValidator.Describe(x => x.ListOfStringsField).IsNotNull();

                var subValidator = new TestSubValidation<Request>();
                subValidator.Describe(x => x.ListOfStringsField).IsNotNull();
                subValidator.Describe(x => x.SubRequestField)
                    .SetValidator(subRequestFieldValidator);

                var validator = new TestControllerValidation();
                validator.DescribeFunc(x => x.CreateByQuery(validator.Param.Is<Request>()
                    .IsNotNull()
                    .SetValidator(subValidator)));

                return validator;
            },
            (ApiValidationBuilder builder) =>
            {
                builder.ParamIs<Request>(ParamType.FromQuery, ["SubRequestField.ListOfStringsField"]).IsNotNull2();
                builder.ParamIs<Request>(ParamType.FromQuery, ["ListOfStringsField"]).IsNotNull2();
            }
        };
    }
}
