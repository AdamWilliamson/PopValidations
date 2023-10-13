using PopValidations;
using PopValidations.Validations.Base;
using System.Threading.Tasks;

namespace PopValidations_Tests.DepthTests;

internal class FieldObjectValidator : AbstractSubValidator<FieldObject>
{
    public FieldObjectValidator(IScopedData<FieldObject> scopedData)
    {
        Describe(x => x.Is)
            .Is(
                "Is Description", 
                "Is Failed", 
                scopedData.To(nameof(FieldObject.Is), (int x, FieldObject? scoped) => x == (scoped?.Is ?? x + 1))
            );
        Describe(x => x.IsEmpty).IsEmpty();
        Describe(x => x.IsEnum).IsEnum(typeof(FieldObjectEnum));
        Describe(x => x.IsEqualTo).IsEqualTo(scopedData.To(nameof(FieldObject.IsEqualTo), x => x.IsEqualTo));
        Describe(x => x.IsGreaterThan).IsGreaterThan(scopedData.To(nameof(FieldObject.IsGreaterThan), x => x.IsGreaterThan));
        Describe(x => x.IsGreaterThanOrEqualTo).IsGreaterThanOrEqualTo(scopedData.To(nameof(FieldObject.IsGreaterThanOrEqualTo), x => x.IsGreaterThanOrEqualTo));
        Describe(x => x.IsLengthExclusivelyBetween).IsLengthExclusivelyBetween(
            scopedData.To<int?>(nameof(FieldObject.IsLengthExclusivelyBetween), x => x.IsLengthExclusivelyBetween.Length - 1),
            scopedData.To<int?>(nameof(FieldObject.IsLengthExclusivelyBetween), x => x.IsLengthExclusivelyBetween.Length +  1)
        );
        Describe(x => x.IsLengthInclusivelyBetween).IsLengthInclusivelyBetween(
            scopedData.To<int?>(nameof(FieldObject.IsLengthInclusivelyBetween), x => x.IsLengthInclusivelyBetween.Length),
            scopedData.To<int?>(nameof(FieldObject.IsLengthInclusivelyBetween), x => x.IsLengthInclusivelyBetween.Length + 1)
        );
        Describe(x => x.IsLessThan).IsLessThan(scopedData.To(nameof(FieldObject.IsLessThan), x => x.IsLessThan));
        Describe(x => x.IsLessThanOrEqualTo).IsLessThanOrEqualTo(scopedData.To(nameof(FieldObject.IsLessThanOrEqualTo), x => x.IsLessThanOrEqualTo));
        Describe(x => x.IsNotEmpty).IsNotEmpty();
        Describe(x => x.IsNotNull).IsNotNull();
        Describe(x => x.IsNull).IsNull();
    }
}

internal class Level1Validator : AbstractValidator<Level1>
{
    public Level1Validator()
    {
        Scope(
            "Getting Values of Things", 
            (validationObject) => Task.FromResult(FieldObjectBuilder.CreateValidObject(validationObject.GetType().Name)), 
            (scopedData) =>
            {
                Include(new Level1InclusionValidator(scopedData));
            });
    }
}

internal class Level1InclusionValidator : AbstractSubValidator<Level1>
{
    public Level1InclusionValidator(IScopedData<FieldObject> scopedData)
    {
        Describe(x => x.TestItem).SetValidator(new FieldObjectValidator(scopedData));

        ScopeWhen(
            "When ListOfLevel2 is Not Null",
            (x) => Task.FromResult(x.ListOfLevel2 is not null),
            "Getting Values of Things",
            (validationObject) => Task.FromResult(FieldObjectBuilder.CreateValidObject(validationObject.GetType().Name)),
            (inclusionScopedData) =>
            {
                Describe(x => x.TestItem).SetValidator(new FieldObjectValidator(inclusionScopedData));

                DescribeEnumerable(x => x.ListOfLevel2)
                    .Vitally().IsNotNull()
                    .ForEach(x => x.Vitally().IsNotNull().SetValidator(new Level2Validator()));
            });
    }
}

internal class Level2Validator : AbstractSubValidator<Level2>
{
    public Level2Validator()
    {
        Scope(
            "Getting Values of Things",
            (validationObject) => Task.FromResult(FieldObjectBuilder.CreateValidObject(validationObject.GetType().Name)),
            (scopedData) =>
            {
                Include(new Level2InclusionValidator(scopedData));
            });
    }
}

internal class Level2InclusionValidator : AbstractSubValidator<Level2>
{
    public Level2InclusionValidator(IScopedData<FieldObject> scopedData)
    {
        Describe(x => x.TestItem).SetValidator(new FieldObjectValidator(scopedData));

        ScopeWhen(
            "When ListOfLevel2 is Not Null",
            (x) => Task.FromResult(x.ListOfLevel3 is not null),
            "Getting Values of Things",
            (validationObject) => Task.FromResult(FieldObjectBuilder.CreateValidObject(validationObject.GetType().Name)),
            (inclusionScopedData) =>
            {
                Describe(x => x.TestItem).SetValidator(new FieldObjectValidator(inclusionScopedData));

                DescribeEnumerable(x => x.ListOfLevel3)
                    .Vitally().IsNotNull()
                    .ForEach(x => x.Vitally().IsNotNull().SetValidator(new Level3Validator()));
            });
    }
}


internal class Level3Validator : AbstractSubValidator<Level3>
{
    public Level3Validator()
    {
        Scope(
            "Getting Values of Things",
            (validationObject) => Task.FromResult(FieldObjectBuilder.CreateValidObject(validationObject.GetType().Name)),
            (scopedData) =>
            {
                Include(new Level3InclusionValidator(scopedData));
            });
    }
}

internal class Level3InclusionValidator : AbstractSubValidator<Level3>
{
    public Level3InclusionValidator(IScopedData<FieldObject> scopedData)
    {
        Describe(x => x.TestItem).SetValidator(new FieldObjectValidator(scopedData));
    }
}