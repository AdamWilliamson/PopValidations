using ApiValidations;
using DjvuNet.Tests.Xunit;
using Microsoft.AspNetCore.Mvc;
using PopApiValidations.Swashbuckle_Tests.Helpers;
using PopValidations;
using System.Reflection;

namespace PopApiValidations.Swashbuckle_Tests.Converter_FeatureTests;

public class IsNullOpenApiConverter_Tests
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
            "Create Post RequestBody.IntegerField is null",
            "/api/Test",
            nameof(TestController.Create),
            (MethodInfo m) => m == typeof(TestController).GetMethod(nameof(TestController.Create)),
            () =>
            {
                var subValidator = new TestSubValidation<Request>();
                subValidator.Describe(x => x.IntegerField).IsNull();

                var validator = new TestControllerValidation();
                validator.DescribeFunc(x => x.Create(validator.Param.Is<Request>().SetValidator(subValidator)));
                return validator;
            },
            (ApiValidationBuilder builder) =>
            {
                builder.ParamIs<Request>(["integerField"]).IsNull();
            }
        };

        yield return new object[] {
            "Create Post RequestBody.SubRequestField.IntegerField is null",
            "/api/Test",
            nameof(TestController.Create),
            (MethodInfo m) => m == typeof(TestController).GetMethod(nameof(TestController.Create)),
            () =>
            {
                var subRequestFieldValidator = new TestSubValidation<SubRequest>();
                subRequestFieldValidator.Describe(x => x.IntegerField).IsNull();

                var subValidator = new TestSubValidation<Request>();
                subValidator.Describe(x => x.SubRequestField).IsNull().SetValidator(subRequestFieldValidator);

                var validator = new TestControllerValidation();
                validator.DescribeFunc(x => x.Create(validator.Param.Is<Request>().IsNull().SetValidator(subValidator)));
                return validator;
            },
            (ApiValidationBuilder builder) =>
            {
                builder.ParamIs<Request>(["subRequestField"]).IsNull();
                builder.ParamIs<Request>(["subRequestField", "integerField"]).IsNull();
            }
        };

        yield return new object[] {
            "Create Post RequestBody.SubRequestField.IntegerField.DataItemField are not null",
            "/api/Test",
            nameof(TestController.Create),
            (MethodInfo m) => m == typeof(TestController).GetMethod(nameof(TestController.Create)),
            () =>
            {
                var subRequestDataItemValidator = new TestSubValidation<RequestDataItem>();
                subRequestDataItemValidator.Describe(x => x.Identifier).IsNull();

                var subRequestFieldValidator = new TestSubValidation<SubRequest>();
                subRequestFieldValidator.Describe(x => x.IntegerField).IsNull();
                subRequestFieldValidator.Describe(x => x.DataItemField)
                    .IsNull()
                    .SetValidator(subRequestDataItemValidator)
                    ;

                var requestDataItemValidator = new TestSubValidation<RequestDataItem>();
                requestDataItemValidator.Describe(x => x.Identifier).IsNull();

                var subValidator = new TestSubValidation<Request>();
                subValidator.Describe(x => x.IntegerField).IsNull();
                subValidator.Describe(x => x.DataItemField)
                    .IsNull()
                    .SetValidator(requestDataItemValidator)
                ;
                subValidator.Describe(x => x.SubRequestField)
                    .IsNull()
                    .SetValidator(subRequestFieldValidator);

                var validator = new TestControllerValidation();
                validator.DescribeFunc(x => x.Create(validator.Param.Is<Request>()
                    .IsNull()
                    .SetValidator(subValidator)));
                return validator;
            },
            (ApiValidationBuilder builder) =>
            {
                builder.ParamIs<Request>(["subRequestField"]).IsNull();
                builder.ParamIs<Request>(["integerField"]).IsNull();
                builder.ParamIs<Request>(["dataItemField"]).IsNull();
                builder.ParamIs<Request>(["dataItemField", "identifier"]).IsNull();
                builder.ParamIs<Request>(["subRequestField", "integerField"]).IsNull();
                builder.ParamIs<Request>(["subRequestField", "dataItemField"]).IsNull();
                builder.ParamIs<Request>(["subRequestField", "dataItemField", "identifier"]).IsNull();
            }
        };

        yield return new object[] {
            "CreateByQuery Post RequestBody.SubRequestField.IntegerField are not null",
            "/api/Test/CreateByQuery",
            nameof(TestController.CreateByQuery),
            (MethodInfo m) => m == typeof(TestController).GetMethod(nameof(TestController.CreateByQuery)),
            () =>
            {
                var subRequestFieldValidator = new TestSubValidation<SubRequest>();
                subRequestFieldValidator.Describe(x => x.IntegerField).IsNull();

                var subValidator = new TestSubValidation<Request>();
                subValidator.Describe(x => x.IntegerField).IsNull();
                subValidator.Describe(x => x.SubRequestField)
                    .SetValidator(subRequestFieldValidator);

                var validator = new TestControllerValidation();
                validator.DescribeFunc(x => x.CreateByQuery(validator.Param.Is<Request>()
                    .IsNull()
                    .SetValidator(subValidator)));

                return validator;
            },
            (ApiValidationBuilder builder) =>
            {
                builder.ParamIs<Request>(ParamType.FromQuery, ["SubRequestField.IntegerField"]).IsNull();
                builder.ParamIs<Request>(ParamType.FromQuery, ["IntegerField"]).IsNull();
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
                    .IsNull()
                    .ForEach(x => x.IsNull());

                var subValidator = new TestSubValidation<Request>();
                subValidator.DescribeEnumerable(x => x.ListOfStringsField)
                    .IsNull()
                    .ForEach(x => x.IsNull());
                subValidator.Describe(x => x.SubRequestField)
                    .SetValidator(subRequestFieldValidator);

                var validator = new TestControllerValidation();
                validator.DescribeFunc(x => x.CreateByQuery(validator.Param.Is<Request>()
                    .IsNull()
                    .SetValidator(subValidator)));

                return validator;
            },
            (ApiValidationBuilder builder) =>
            {
                builder.ParamIs<Request>(ParamType.FromQuery, ["SubRequestField.ListOfStringsField"]).IsNull();
                builder.ParamIs<Request>(ParamType.FromQuery, ["SubRequestField.ListOfStringsField[n]"]).IsNull();
                builder.ParamIs<Request>(ParamType.FromQuery, ["ListOfStringsField"]).IsNull();
                builder.ParamIs<Request>(ParamType.FromQuery, ["ListOfStringsField[n]"]).IsNull();
            }
       };
    }
}
