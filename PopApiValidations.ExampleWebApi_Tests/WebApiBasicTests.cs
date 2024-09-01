using PopApiValidations.ExampleWebApi.Controllers;
using System.Net;
using FluentAssertions;
using System.Text.Json;
using PopApiValidations.ExampleWebApi;
using System.Text;
using FluentAssertions.Execution;

namespace PopApiValidations.ExampleWebApi_Tests;

public class WebApiBasicTests : WebApiTestBase<AddressOwnershipController>
{
    public WebApiBasicTests(TestingApplicationFactory factory): base(factory) {}

    [Fact]
    public async Task AnEndpointWithNoParameters_DoesntValidated()
    {
        // Arrange
        var client = GetSUT();

        // Act
        var response = await client.GetAsync(GetUrl(nameof(AddressOwnershipController.Get)));

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task ASingleDataTypeParam_IsValidated()
    {
        // Arrange
        var client = GetSUT();

        // Act
        var response = await client.GetAsync(GetUrl(nameof(AddressOwnershipController.GetById) + "/{id}", new {id = -1}));

        // Assert
        response.EnsureValidationErrors();
        var vr = await response.ToValidationResult();
        vr.Errors.Keys.Should().Contain("id");
        vr.Errors["id"].Should().HaveCountGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task MultipleParams_WhereEach_IsCorrect()
    {
        // Arrange
        var client = GetSUT();

        // Act
        var response = await client.PutAsync(
            GetUrl($"10/" + nameof(AddressOwnershipController.InsertOwner), queryObject: 
                new
                {
                    firstName = "Greg",
                    lastName = "Washington",
                    contactType = "Email",
                    contactValue = "TestEmail@Email.com.au"
                }
            ), null
        );

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Theory]
    [InlineData("id")]
    [InlineData("firstName")]
    [InlineData("lastName")]
    [InlineData("contactType")]
    [InlineData("contactValue")]
    public async Task MultipleParams_WhereEach_IsValidated(string paramName)
    {
        // Arrange
        var client = GetSUT();

        // Act
        var response = await client.PutAsync(
            GetUrl($"-1/" + nameof(AddressOwnershipController.InsertOwner), queryObject: new
            {
                firstName = (string?)null,
                lastName = (string?)null,
                contactType = (string?)null,
                contactValue = (string?)null
            }),
            null
        );

        // Assert
        response.EnsureValidationErrors();
        var vr = await response.ToValidationResult();
        vr.Errors.Keys.Should().Contain(paramName);
        vr.Errors[paramName].Should().HaveCountGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task AnObjectAsContent_IsValidated()
    {
        const string paramName = "newAddressOwnership.";
        List<string> objectsAcessors = new()
        {
            "Id",
            "Address.Id",
            "Address.Postcode",
            "Address.StreetName",
            "Address.StreetNumber",
            "Address.Suburb",
            "Owners[0].Id",
            "Owners[0].Age",
            "Owners[0].FirstName",
            "Owners[0].LastName",
            "Owners[0].ContactRecords[0].Id",
            "Owners[0].ContactRecords[0].Value"
        };

        // Arrange
        var client = GetSUT();

        // Act
        var response = await client.PostAsync(
            GetUrl(nameof(AddressOwnershipController.AddAddressOwnership)),
            new StringContent(
                JsonSerializer.Serialize(ObjectMother.AddressOwnershipSetup.FullHeirarchyWithInvalidFields()),
                Encoding.UTF8,
                "application/json"
            )
        );

        // Assert
        response.EnsureValidationErrors();
        var vr = await response.ToValidationResult();

        using (new AssertionScope())
        {
            foreach (var path in objectsAcessors)
            {
                vr.Errors.Keys.Should().Contain(paramName + path);
                vr.Errors[paramName + path].Should().HaveCountGreaterThanOrEqualTo(1);
            }
        }
    }

    [Fact]
    public async Task AnObjectAsContent_IsSuccessful()
    {
        // Arrange
        var client = GetSUT();

        // Act
        var response = await client.PostAsync(
            GetUrl(nameof(AddressOwnershipController.AddAddressOwnership)),
            new StringContent(
                JsonSerializer.Serialize(ObjectMother.AddressOwnershipSetup.ValidNewEntry()),
                Encoding.UTF8,
                "application/json"
            )
        );

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task WhenSendingAnInvalidContentPackage_ItDoesntTryToValidate()
    {
        // Arrange
        var client = GetSUT();

        // Act
        var response = await client.PostAsync(
            GetUrl(nameof(AddressOwnershipController.AddAddressOwnership)),
            new StringContent(
                "{\"id\": \"hg\",\"Address\": {},\"Owners\":[]}",
                Encoding.UTF8,
                "application/json"
            )
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var vr = await response.ToValidationResult();
        vr.Errors.Keys.Any(x => x.StartsWith("newAddressOwnership.")).Should().BeFalse();
    }

    [Fact]
    public async Task WhenUsingTheIgnoreAttribute_NoValidationHappens()
    {
        // Arrange
        var client = GetSUT();

        // Act
        var response = await client.PostAsync(
            GetUrl(nameof(AddressOwnershipController.AddAddressOwnership_Ignore)),
            new StringContent(
                JsonSerializer.Serialize(ObjectMother.AddressOwnershipSetup.FullHeirarchyWithInvalidFields()),
                Encoding.UTF8,
                "application/json"
            )
        );

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GivenAnApiWithRenameAttribute_AllValidationsUseTheNewName()
    {
        const string paramName = "CreationAddress.";
        List<string> objectsAcessors = new()
        {
            "Id",
            "Address.Id",
            "Address.Postcode",
            "Address.StreetName",
            "Address.StreetNumber",
            "Address.Suburb",
            "Owners[0].Id",
            "Owners[0].Age",
            "Owners[0].FirstName",
            "Owners[0].LastName",
            "Owners[0].ContactRecords[0].Id",
            "Owners[0].ContactRecords[0].Value"
        };

        // Arrange
        var client = GetSUT();

        // Act
        var response = await client.PostAsync(
            GetUrl(nameof(AddressOwnershipController.AddAddressOwnership_Rename)),
            new StringContent(
                JsonSerializer.Serialize(ObjectMother.AddressOwnershipSetup.FullHeirarchyWithInvalidFields()),
                Encoding.UTF8,
                "application/json"
            )
        );

        // Assert
        response.EnsureValidationErrors();
        var vr = await response.ToValidationResult();

        using (new AssertionScope())
        {
            foreach (var path in objectsAcessors)
            {
                vr.Errors.Keys.Should().Contain(paramName + path);
                vr.Errors[paramName + path].Should().HaveCountGreaterThanOrEqualTo(1);
            }
        }
    }
}