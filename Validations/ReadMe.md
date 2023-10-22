PopValidation uses a fluent based style, to bring effective validation, with strong typing, using lambda expressions and generics.

### Usage

Pop Validations, is usable for both public and corporate use, without need for compensation, or mention.
Feature or bugfix PR's, including appropriate tests are more than welcome.

### Developers

The core developer is Adam Williamson(https://github.com/AdamWilliamson), with help from Andrew Williamson (https://github.com/AWilliamson88)

### Example

PopValidation  provides a class "AbstractValidator", that you can inherit which provides the functions
"Describe" and "DescribeEnumerable", which form the basis for all of the valiations, allowing you to
select a field and describe the validations for that field.

An example
```c#
using PopValidation;

public record Song(string? Title, double? Duration);

public class SongValidator: AbstractValidator<Song> 
{
    public SongValidator() 
    {
        RuleFor(x => x.Title).Vitally().NotEmpty();
        RuleFor(x => x.Duration).Vitally().IsNotNull().IsLessThan(10.0, options => options.WithErrorMessage("Songs must be less than 10 minutes long."));
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

