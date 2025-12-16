using ApiValidations;
using DjvuNet.Tests.Xunit;
using Microsoft.AspNetCore.Mvc;
using PopApiValidations.Swashbuckle_Tests.Helpers;
using PopValidations;
using System.Reflection;

namespace PopApiValidations.Swashbuckle_Tests.Converter_FeatureTests;

public class NotNullOpenApiConverter_Tests
{
    [DjvuTheory]
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
        //Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<TestController, TestControllerValidation>();

        var config = new TestWebApiConfig();
        config.ValidateEndpoint = validateEndpointFunc;

        //Act
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
                builder.ParamIs<Request>(["request","integerField"]).IsNotNull();
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
                builder.ParamIs<Request>(["request"]).IsNotNull();
                builder.ParamIs<Request>(["request", "subRequestField"]).IsNotNull();
                builder.ParamIs<Request>(["request", "subRequestField", "integerField"]).IsNotNull();
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
                builder.ParamIs<Request>(["request"]).IsNotNull();
                builder.ParamIs<Request>(["request", "subRequestField"]).IsNotNull();
                builder.ParamIs<Request>(["request", "integerField"]).IsNotNull();
                builder.ParamIs<Request>(["request", "dataItemField"]).IsNotNull();
                builder.ParamIs<Request>(["request", "dataItemField", "identifier"]).IsNotNull();
                builder.ParamIs<Request>(["request", "subRequestField", "integerField"]).IsNotNull();
                builder.ParamIs<Request>(["request", "subRequestField", "dataItemField"]).IsNotNull();
                builder.ParamIs<Request>(["request", "subRequestField", "dataItemField", "identifier"]).IsNotNull();
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
                //builder.ParamIs<Request>().IsNotNull();  // Query based classes, dont pass validation.
                builder.ParamIs<Request>(ParamType.FromQuery, ["request", "SubRequestField.IntegerField"]).IsNotNull();
                builder.ParamIs<Request>(ParamType.FromQuery, ["request", "IntegerField"]).IsNotNull();
            }
        };

        yield return new object[] {
            "CreateByQuery Post Array Fields are not null",
            "/api/Test/CreateByQuery",
            nameof(TestController.CreateByQuery),
            (MethodInfo m) => m == typeof(TestController).GetMethod(nameof(TestController.CreateByQuery)),
            () =>
            {
                var subRequestFieldValidator = new TestSubValidation<SubRequest>();
                subRequestFieldValidator.DescribeEnumerable(x => x.ListOfStringsField)
                    .IsNotNull()
                    .ForEach(x => x.IsNotNull());

                var subValidator = new TestSubValidation<Request>();
                subValidator.DescribeEnumerable(x => x.ListOfStringsField)
                    .IsNotNull()
                    .ForEach(x => x.IsNotNull());
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
                //builder.ParamIs<Request>().IsNotNull();
                builder.ParamIs<Request>(ParamType.FromQuery, ["request", "SubRequestField.ListOfStringsField"]).IsNotNull();
                builder.ParamIs<Request>(ParamType.FromQuery, ["request", "SubRequestField.ListOfStringsField[n]"]).IsNotNull();
                builder.ParamIs<Request>(ParamType.FromQuery, ["request", "ListOfStringsField"]).IsNotNull();
                builder.ParamIs<Request>(ParamType.FromQuery, ["request", "ListOfStringsField[n]"]).IsNotNull();
            }
       };
    }
}
