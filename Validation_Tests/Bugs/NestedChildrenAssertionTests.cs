using ApprovalTests;
using FluentAssertions;
using Newtonsoft.Json;
using PopValidations;
using PopValidations_Tests.TestHelpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations_Tests.Bugs;

public record Level5(string Name);

public record Level4(string Name, List<Level5> Level4ArrayPropertyForLevel5);

public record Level3(string Name, Level4 Level3PropertyForLevel4);

public record Level2(
    string Name,
    Level3? Level2PropertyForLevel3,
    List<Level3>? Level2ArrayPropertyForLevel3
);

public record Level1(
    string Name,
    Level2? Level1PropertyForLevel2,
    List<Level2>? Level1ArrayPropertyForLevel2
);

public class Level1Validator : AbstractValidator<Level1>
{
    public Level1Validator()
    {
        //Describe(x => x.Name).IsEqualTo("Level1_Name");
        Describe(x => x.Level1PropertyForLevel2)
            //.Vitally()
            //.NotNull()
            .SetValidator(new Level2Validator());
        DescribeEnumerable(x => x.Level1ArrayPropertyForLevel2)
            //.Vitally()
            //.NotNull()
            .ForEach(x => x.SetValidator(new Level2Validator_InsideArray()));
    }
}

public class Level1Validator_InsideArray : AbstractValidator<Level1>
{
    public Level1Validator_InsideArray()
    {
        //Describe(x => x.Name).IsEqualTo("INSIDEARRAY");
        //Describe(x => x.Level1PropertyForLevel2)
            //.Vitally()
            //.NotNull()
            //.SetValidator(new Level2Validator());
        DescribeEnumerable(x => x.Level1ArrayPropertyForLevel2)
            .ForEach(x => x.SetValidator(new Level2Validator_InsideArray()));
    }
}

public class Level2Validator : AbstractSubValidator<Level2>
{
    public Level2Validator()
    {
        //Describe(x => x.Name).IsEqualTo("Level2_Name");
        //Describe(x => x.FurtherChild).NotNull();
        Describe(x => x.Level2PropertyForLevel3)
        //    .Vitally()
        //    .NotNull()
            .SetValidator(new Level3RequestValidator());

        //DescribeEnumerable(x => x.Level2ArrayPropertyForLevel3)
            //.Vitally()
            //.NotNull()
            //.ForEach(x => x.SetValidator(new Level3Validator_InsideArray()))
        //.ForEach(x => x.NotNull())
        ;
    }
}

public class Level2Validator_InsideArray : AbstractSubValidator<Level2>
{
    public Level2Validator_InsideArray()
    {
        //Describe(x => x.Name).IsEqualTo("INSIDEARRAY");
        //Describe(x => x.FurtherChild).NotNull();
        //Describe(x => x.Level2PropertyForLevel3)
            //.NotNull()
            //.SetValidator(new Level3RequestValidator());

        DescribeEnumerable(x => x.Level2ArrayPropertyForLevel3)
            //.Vitally().NotNull()
            //.IsNull()
            .ForEach(x => x.SetValidator(new Level3Validator_InsideArray()))
        //.ForEach(x => x.NotNull())
        ;
    }
}

public class Level3RequestValidator : AbstractSubValidator<Level3>
{
    public Level3RequestValidator()
    {
        Describe(x => x.Name).IsEqualTo("Level3Name");
        //Describe(x => x.Level3PropertyForLevel4).SetValidator(new Level4RequestValidator());
    }
}

public class Level3Validator_InsideArray : AbstractSubValidator<Level3>
{
    public Level3Validator_InsideArray()
    {
        //Describe(x => x.Name).IsEqualTo("INSIDE ARRAY");
        Describe(x => x.Level3PropertyForLevel4).SetValidator(new Level4RequestValidator());        
    }
}

public class Level4RequestValidator : AbstractSubValidator<Level4>
{
    public Level4RequestValidator()
    {
        //Describe(x => x.Name).IsEqualTo("NOT ARRAY");
        DescribeEnumerable(x => x.Level4ArrayPropertyForLevel5)
            .ForEach(x => x.SetValidator(new Level5RequestValidator()));
    }
}

public class Level5RequestValidator : AbstractSubValidator<Level5>
{
    public Level5RequestValidator()
    {
        Describe(x => x.Name).IsEqualTo("LEVEL 5 NAME");
    }
}

public class NestedChildrenAssertionTests
{
    public Level1 CreateMoreBorkedSubject()
    {
        return new Level1(
            Name: "Level1Name_Bad",
            //Child: null,
            Level1PropertyForLevel2: new Level2(
                Name: "Level2Name_Bad",
                //  FurtherChild: null,
                Level2PropertyForLevel3: new Level3(
                    "Level3Name",
                    new Level4("Level3Name_Bad", new() { new Level5("LEVEL 5 NAME") })
                ),
                //Level2ArrayPropertyForLevel3: null
                Level2ArrayPropertyForLevel3: new()
                {
                    new Level3(
                        "Deeper Name[0]",
                        new Level4("Level3Name_Bad", new() { new Level5("LEVEL 5 NAME") })
                    )
                } // WORKS
            ),
            //ChildArray: null
            Level1ArrayPropertyForLevel2: new()
            {
                new Level2(
                    Name: "Level2ArrayItemName_Bad",
                    //FurtherChild: null,
                    Level2PropertyForLevel3: new Level3(
                        "Level3InsideLevel2ArrayItemName_Bad",
                        new Level4("Level3Name_Bad", new() { new Level5("LEVEL 5 NAME") })
                    ), // Works
                    //FurtherChildArray: null
                    Level2ArrayPropertyForLevel3: new()
                    {
                        new Level3(
                            Name: "Level3InsideArrayInsideArray_Bad",
                            Level3PropertyForLevel4: new Level4("Level3Name_Bad", new() { new Level5("LEVEL 5 NAME_Bad") })
                        )
                    }
                ),
            }
        );
    }

    [Fact]
    public async Task GivenAValidator_WithNoErrors_ThenNoErrorsAreProduced3()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Level1Validator());

        // Act
        var validationResult = await runner.Validate(CreateMoreBorkedSubject());

        // Assert
        Approvals.VerifyJson(JsonConvert.SerializeObject(validationResult.Errors));
        validationResult.Errors.Should().BeEmpty();
    }
}
