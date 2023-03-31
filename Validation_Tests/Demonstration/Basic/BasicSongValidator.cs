using PopValidations;

namespace PopValidations_Tests.Demonstration.Basic;

public class BasicSongValidator : AbstractValidator<BasicSong>
{
    public BasicSongValidator()
    {
        #region HomeDemoCode
        //Describe(x => x.TrackNumber)
        //    .NotNull();

        //Describe(x => x.TrackName)
        //    .IsEqualTo("Definitely Not The Correct Song Name.");

        //Describe(x => x.Duration)
        //    .IsEqualTo(
        //        -1,
        //        o => o
        //            .WithErrorMessage("Song must have a negative duration.")
        //            .WithDescription("Songs must force you to travel slowly backwards in time to listen to.")
        //    );

        //Describe(x => x.Genre)
        //    //.Vitally().NotEmpty();
        //    //.HasLengthBetween(20, 400)
        //    ;
        #endregion

        #region demoCode
        #region IsNull
        //Describe(x => x.TrackName).IsNull();
        //Describe(x => x.TrackNumber).IsNull();
        //Describe(x => x.Duration).IsNull();
        //Describe(x => x.Genre)
        //  .Vitally().IsNull(options =>
        //    options
        //      .WithErrorMessage("We don't like values")
        //      .WithDescription("Values are bad.")
        //  );
        #endregion
        #region NotNull
        //Describe(x => x.TrackName).NotNull();
        //Describe(x => x.TrackNumber).NotNull();
        //Describe(x => x.Duration).NotNull();
        //Describe(x => x.Genre)
        //  .Vitally().NotNull(options =>
        //    options
        //      .WithErrorMessage("Null is Invalid")
        //      .WithDescription("Nulls are bad.")
        //  );
        #endregion
        #region IsEmpty
        //Describe(x => x.TrackName).IsEmpty();
        //Describe(x => x.TrackNumber).IsEmpty();
        //Describe(x => x.Genre)
        //  .Vitally().IsEmpty(options =>
        //    options
        //      .WithErrorMessage("Non Empty Fields are Invalid.")
        //      .WithDescription("This Field must be Empty.")
        //  );
        #endregion
        #region IsNotEmpty
        //Describe(x => x.TrackName).IsNotEmpty();
        //Describe(x => x.TrackNumber).IsNotEmpty();
        //Describe(x => x.Genre)
        //  .Vitally().IsNotEmpty(options =>
        //    options
        //      .WithErrorMessage("Empty Fields are Invalid.")
        //      .WithDescription("This Field must not be Empty.")
        //  );
        #endregion
        #region IsGreaterThan 
        //Describe(x => x.TrackName).IsGreaterThan("Down With The Sickness");
        //Describe(x => x.TrackNumber).IsGreaterThan(new ScopedData<double>(double.MaxValue));
        //Describe(x => x.Duration).IsGreaterThan(new ScopedData<double>(2));
        #endregion
        #region IsGreaterThanOrEqualTo
        //Describe(x => x.TrackName).IsGreaterThanOrEqualTo("Down With The Sickness");
        //Describe(x => x.TrackNumber).IsGreaterThanOrEqualTo(new ScopedData<double>(double.MaxValue));
        //Describe(x => x.Duration).IsGreaterThanOrEqualTo(new ScopedData<double>(2));
        #endregion
        #region IsLessThan
        //Describe(x => x.TrackName).IsLessThan("Down With The Sickness");
        //Describe(x => x.TrackNumber).IsLessThan(new ScopedData<double>(double.MaxValue));
        //Describe(x => x.Duration).IsLessThan(new ScopedData<double>(2));
        #endregion
        #region IsLessThanOrEqualTo
        //Describe(x => x.TrackName).IsLessThanOrEqualTo("Down With The Sickness");
        //Describe(x => x.TrackNumber).IsLessThanOrEqualTo(new ScopedData<double>(double.MaxValue));
        //Describe(x => x.Duration).IsLessThanOrEqualTo(new ScopedData<double>(2));
        #endregion
        #region IsLengthInclusivelyBetween
        Describe(x => x.TrackNumber).IsLengthInclusivelyBetween(5, 10);
        Describe(x => x.TrackNumber).IsLengthInclusivelyBetween(2, 5);
        #endregion
        #endregion

    }
}