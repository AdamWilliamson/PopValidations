using ApprovalTests;
using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json.Linq;
using PopValidations.Swashbuckle_Tests.Helpers;
using PopValidations_Tests.TestHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations.Demonstration_Tests.Examples.Advanced;

public class AdvancedDemonstrationTests
{
    [Fact]
    public async Task Validation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(
            new AdvancedDemonstration.AlbumSubmissionValidator(
                new AdvancedDemonstration.AlbumVerificationService()
            )
        );
        var albumSubmission = new AdvancedDemonstration.AlbumSubmission(
            new() {
                new AdvancedDemonstration.Album(
                    "Down with the Sickness",
                     AdvancedDemonstration.AlbumType.SingleArtist,
                    new (){ new AdvancedDemonstration.Artist("") },
                    "https://en.wikipedia.org/wiki/File:TheSickness.jpg",
                    new System.DateTime(2000, 03, 07),
                    new()
                    {
                        new AdvancedDemonstration.Song(
                            new(){ new AdvancedDemonstration.Artist("Disturbed") },
                            4,
                            "Down With The Sickness",
                            4.38,
                            "Nu Metal"
                        ),
                        new AdvancedDemonstration.Song(
                            new() { new AdvancedDemonstration.Artist("Muse") },
                            1,
                            "Supremecy",
                            4.51,
                            "Progressive Rock"
                        ),
                        null
                    },
                    new (){ "Rock", "Nu Metal" }
                ),
                new AdvancedDemonstration.Album(
                    "",
                     AdvancedDemonstration.AlbumType.Collaboration,
                    new() { new AdvancedDemonstration.Artist("Disturbed") },
                    null,
                    null,
                    new()
                    {
                        new AdvancedDemonstration.Song(
                            null,
                            null,
                            "",
                            -1,
                            "Pop"
                            ),
                        null
                    },
                    new() { "Rock", "Nu Metal" }
                ),
                new AdvancedDemonstration.Album(
                    "",
                     AdvancedDemonstration.AlbumType.Compilation,
                    new() { new AdvancedDemonstration.Artist("Disturbed") },
                    null,
                    null,
                    new()
                    {
                        new AdvancedDemonstration.Song(
                            new() { new AdvancedDemonstration.Artist("Disturbed") },
                            null,
                            "",
                            -1,
                            "Rock"
                        ),
                        new AdvancedDemonstration.Song(
                            new() { new AdvancedDemonstration.Artist("Disturbed") },
                            null,
                            "",
                            -1,
                            "Country"
                        ),
                    },
                    new() { "Pop" }
                ),
                new AdvancedDemonstration.Album(
                    "",
                     AdvancedDemonstration.AlbumType.Single,
                    new() { new AdvancedDemonstration.Artist("Disturbed") },
                    null,
                    null, 
                    Enumerable.Repeat(0, 8).Select<int, AdvancedDemonstration.Song?>(i => 
                        new AdvancedDemonstration.Song(
                            null,
                            null,
                            "",
                            -1,
                            "Rock"
                        )
                    ).ToList(),
                    new() { "Pop" }
                ),
                new AdvancedDemonstration.Album(
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null
                ),
                null
            }
        );

        // Act
        var results = await runner.Validate(albumSubmission);
        var json = JsonConverter.ToJson(results);

        // Assert
        Approvals.VerifyJson(json);

        void AssertType(List<string> parentNames, Type t, List<string> ignore)
        {
            if (t.IsGenericType && typeof(List<>) == t.GetGenericTypeDefinition()) 
            { 
                t = t.GetGenericArguments()[0];
            }

            foreach (var prop in t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                if (
                    prop.PropertyType.DeclaringType?.Name == nameof(AdvancedDemonstration)
                    || (
                        prop.PropertyType.IsGenericType
                        && prop.PropertyType.GetGenericArguments()[0].DeclaringType?.Name == nameof(AdvancedDemonstration)
                    )
                )
                {
                    var newListOfParents = new List<string>(parentNames);
                    newListOfParents.Add(prop.Name);
                    if (!prop.PropertyType.IsEnum && (!Nullable.GetUnderlyingType(prop.PropertyType)?.IsEnum ?? true))
                    {
                        AssertType(newListOfParents, prop.PropertyType, ignore);
                    }
                    results.GetErrorsForField(newListOfParents).Should().HaveCountGreaterThan(0, string.Join('.', newListOfParents));
                }
            }
        }

        using (new AssertionScope()) 
        {
            AssertType(new() { }, typeof(AdvancedDemonstration.AlbumSubmission), new() { "Albums" });
        }
    }

    [Fact]
    public void Description()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(
            new AdvancedDemonstration.AlbumSubmissionValidator(new AdvancedDemonstration.AlbumVerificationService())
        );

        // Act
        var results = runner.Describe();
        var json = JsonConverter.ToJson(results);

        // Assert
        Approvals.VerifyJson(json);
    }

    [Fact]
    public async Task OpenApi()
    {
        // Arrange
        var config = new WebApiConfig();
        var setup = new TestSetup<
               AdvancedDemonstration.TestController,
               AdvancedDemonstration.AlbumSubmissionValidator,
               AdvancedDemonstration.AlbumSubmission
           >(new AdvancedDemonstration.AlbumSubmissionValidator(new AdvancedDemonstration.AlbumVerificationService()));
        setup.Register(typeof(AdvancedDemonstration.AlbumVerificationService), new AdvancedDemonstration.AlbumVerificationService());
        // Act
        var helper = await setup.GetHelper(config);
        JObject json = JObject.Parse(helper.Content!);
        var match = json["components"]!["schemas"]!;//.FirstOrDefault();

        // Assert
        Approvals.VerifyJson(match?.ToString(Newtonsoft.Json.Formatting.None));
    }
}

