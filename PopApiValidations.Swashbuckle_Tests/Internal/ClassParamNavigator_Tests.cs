using ApiValidations;
using DjvuNet.Tests.Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using PopApiValidations.Swashbuckle.Internal;
using PopApiValidations.Swashbuckle.Internal.OperationFilter;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace PopApiValidations.Swashbuckle_Tests.Internal;

public class SubRequest
{
    [JsonProperty(PropertyName = "TooDeepInteger")]
    public int Integer { get; set; }

    public List<int> Integers { get; set; } = new();
}

public class Request
{
    public int Integer { get; set; }

    [JsonProperty(PropertyName = "NewName")]
    public string Renamed { get; set; }

    public SubRequest SubRequest { get; set; }

    [JsonProperty(PropertyName = "RenamedSubRequest")]
    public SubRequest SubRequest2 { get; set; }

    public List<SubRequest> SubRequests { get; set; }
}

[ApiController]
public class Test_Api : Controller
{
    public void Function1(Request request) { }

    public void QueryFunction([FromQuery]Request request) { }
    public void QueryListFunction([FromQuery] List<Request> requests) { }
    public void NamedQueryFunction([FromQuery(Name ="NewRequest")] Request request) { }

    public void PostFunction([FromBody]Request request) { }
    
    public void FormFunction([FromForm] Request request) { }
    public void NamedFormFunction([FromForm(Name = "NewFormName")] Request request) { }

    public void BasicRouteFunction([FromRoute]int id) { }
    public void NamedBasicRouteFunction([FromRoute(Name = "NewId")] int id) { }
    public void ObjectRouteFunction([FromRoute(Name = "NewUrlRequest")] Request request) { }

    public void HeaderFunction([FromQuery] Request request) { }
    public void NamedHeaderFunction([FromQuery(Name = "NewHeaderName")] Request request) { }

    public void RouteQueryAndBodyFunction([FromRoute]int id, [FromQuery(Name = "NewHeaderName")] Request request, [FromBody]Request body) { }
}

public class Test_ApiValidation : ApiValidator<Test_Api> { }

public class TestScenario
{
    public MethodInfo MethodInfo { get; init; }
    public string OpenApiPropertyName { get; init; }
    public ParameterLocation? Location { get; init; }
    public Type ObjectType { get; init; }

    public new string ToString()
    {
        return $"{MethodInfo.Name} - {OpenApiPropertyName} - {ObjectType.Name}";
    }

    public static implicit operator object?[](TestScenario d) => new object?[] { d };
}

public class ClassParamNavigator_Tests
{
    public static IEnumerable<object?[]> RecurseParams(MethodInfo methodInfo, PropertyInfo property, ParameterLocation? location, string parentName)
    {
        var prefix = (string.IsNullOrWhiteSpace(parentName))
            ? RouteHelpers.GetAlternateName(property) 
            : parentName + "." + RouteHelpers.GetAlternateName(property);

        if (
            (location == ParameterLocation.Query || location == ParameterLocation.Header || location == ParameterLocation.Path)
            && RouteHelpers.IsComplexType(property.PropertyType)
        )
        {
            foreach (var prop in property.PropertyType.GetProperties())
            {
                foreach( var item in RecurseParams(methodInfo, prop, location, prefix))
                {
                    yield return item;
                }
            }
        }
        else
        {
            yield return new TestScenario
            {
                MethodInfo = methodInfo,
                OpenApiPropertyName = prefix,
                Location = location,
                ObjectType = property.PropertyType
            };
        }
    }

    public static IEnumerable<object?[]> DataSource()
    {
        foreach (var method in typeof(Test_Api).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            foreach (var param in method.GetParameters())
            {
                var location = RouteHelpers.GetParameterLocation(param);

                if (
                    (location == ParameterLocation.Query || location == ParameterLocation.Header || location == ParameterLocation.Path)
                    && RouteHelpers.IsComplexType(param.ParameterType)
                )
                {
                    foreach (var prop in param.ParameterType.GetProperties())
                    {
                        foreach (var item in RecurseParams(method, prop, location, string.Empty))
                        {
                            yield return item;
                        }
                    }
                }
                else
                {
                    yield return new TestScenario
                    {
                        MethodInfo = method,
                        OpenApiPropertyName = RouteHelpers.GetAlternateName(param),
                        Location = RouteHelpers.GetParameterLocation(param),
                        ObjectType = param.ParameterType
                    };
                }
            }
        }
    }

    [DjvuTheory]
    [MemberData(nameof(DataSource))]
    public void Test(TestScenario scenario)
    {
        // Arrange
        var controllerTester = new PopApiControllerValidationTestBuilder<Test_Api, Test_ApiValidation>();
        var schemaRepository = new SchemaRepository();
        var operation = OpenApiOperationBuilder.CreateFromAction(scenario.MethodInfo, schemaRepository, true);

        var navigator = new OpenApiOperationNavigator(
            new TestWebApiConfig(),
            schemaRepository, 
            operation,
            scenario.MethodInfo
        );

        // Act
        var childNav = navigator.GetOpenApiParamNavigators().FirstOrDefault(x => x.OpenApiParameterName == scenario.OpenApiPropertyName);

        // Assert
        childNav.OpenApiParameterName.Should().Be(scenario.OpenApiPropertyName);
        childNav.ObjectType.Should().Be(scenario.ObjectType);
    }
}
