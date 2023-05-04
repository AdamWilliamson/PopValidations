using PopValidations;

namespace PopValidations_Tests.Demonstration.Basic
{
    public class BasicArtistValidator : AbstractValidator<BasicArtist>
    {
        public BasicArtistValidator()
        {
            Describe(x => x.Email).IsEmail();
        }
    }
}
