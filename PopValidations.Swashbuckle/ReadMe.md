PopValidation's Swashbuckle integration allows you to include PopValidations validation descriptions within the OpenApi specifications.

### Usage

Pop Validations Swashbuckle Integration, is usable for both public and corporate use, without need for compensation, or mention.
Feature or bugfix PR's, including appropriate tests are more than welcome.

### Developers

The core developer is Adam Williamson(https://github.com/AdamWilliamson).

### Example

PopValidation provides the ability to Describe the validations input configured for your objects.
These descriptions can be imported into your OpenApi spec, using this package, with multiple configuration options.
The OpenApi specification can be modified in 2 ways, by including the validation descriptions in an attribute, 
as well as, or instead of, modifying the OpenApi in-built validation options.

An example
```c#
using PopValidation;

public record Song(string? Title, double Duration);

public class SongValidator: AbstractValidator<Song> 
{
    public SongValidator() 
    {
        RuleFor(x => x.Title).Vitally().NotEmpty();
        RuleFor(x => x.Duration)
            .IsNotNull()
            .IsLessThan(10.0, options => options.WithErrorMessage("Songs must be less than 10 minutes long."));
    }
}

// Instantiate the Validation Runner.
var runner = new ValidationRunner<Song>(
    new () { new SongValidator() },
    new MessageProcessor()
);

// Run the Validation System.
var results = await runner.Validate(new Song("A Song", 5.5));

// Determine failures;
var isFailure = results.Errors.Any();
```

### Full Documentation

Full documentation can be found at
[https://adamwilliamson.github.io/popvalidations/](https://adamwilliamson.github.io/popvalidations/)

