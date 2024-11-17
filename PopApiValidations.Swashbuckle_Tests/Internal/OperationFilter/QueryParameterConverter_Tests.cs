using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using PopApiValidations.Swashbuckle.Internal.OperationFilter;
using PopApiValidations.Swashbuckle_Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopApiValidations.Swashbuckle_Tests.Internal.OperationFilter;
public class Api
{
    public void Function1(int integer) { }
    public void Function2([FromQuery(Name = "renamed")]int renamedInteger) { }
    public void Function3(Request request) { }
}

public class QueryParameterConverter_Tests
{
    [Fact]
    public void GivenAIntegerParameter_ItReturnsTheSameName()
    {
        // Arrange
        // Act
        var results = QueryParameterConverter.GetPropertyNameFromQuery("integer", typeof(Api).GetMethod(nameof(Api.Function1)).GetParameters()[0]);

        // Assert
        results.Should().Be("integer");
    }

    [Fact]
    public void GivenARenamedIntegerParameter_ItReturnsTheOriginalName()
    {
        // Arrange
        // Act
        var results = QueryParameterConverter.GetPropertyNameFromQuery("renamed", typeof(Api).GetMethod(nameof(Api.Function2)).GetParameters()[0]);

        // Assert
        results.Should().Be("renamedInteger");
    }

    [Fact]
    public void GivenAComplexParameterName_ItReturnsTheSameName()
    {
        // Arrange
        // Act
        var results = QueryParameterConverter.GetPropertyNameFromQuery("Integer", typeof(Api).GetMethod(nameof(Api.Function3)).GetParameters()[0]);

        // Assert
        results.Should().Be("Integer");
    }

    [Fact]
    public void GivenARenamedComplexParameterName_ItReturnsTheOriginalName()
    {
        // Arrange
        // Act
        var results = QueryParameterConverter.GetPropertyNameFromQuery("NewName", typeof(Api).GetMethod(nameof(Api.Function3)).GetParameters()[0]);

        // Assert
        results.Should().Be("Renamed");
    }

    [Fact]
    public void GivenAMultiLayerComplexParameterName_ItReturnsTheFullHeirarchy()
    {
        // Arrange
        // Act
        var results = QueryParameterConverter.GetPropertyNameFromQuery("SubRequest.TooDeepInteger", typeof(Api).GetMethod(nameof(Api.Function3)).GetParameters()[0]);

        // Assert
        results.Should().Be("SubRequest.Integer");
    }
}
