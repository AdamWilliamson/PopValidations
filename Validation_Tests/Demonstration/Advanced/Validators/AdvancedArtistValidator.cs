using PopValidations;

namespace PopValidations_Tests.Demonstration.Advanced.Validators
{
    public class AdvancedArtistValidator : AbstractSubValidator<AdvancedArtist> 
    {
        public AdvancedArtistValidator()
        {
            Describe(x => x.Name).NotNull();
        }
    }
}
