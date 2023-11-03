PopValidation's MedaitR integration allows you to run PopValidations before the MediatR Handlers, and stop execution on failure.

### Usage

Pop Validations MediatR Integration, is usable for both public and corporate use, without need for compensation, or mention.
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
using PopValidations.MediatR;

builder.Services.AddMediatR(
    cfg => cfg
        .RegisterServicesFromAssemblyContaining<BasicObjectController>()
        // Pop Validation Extension, that adds a MediatR Behaviour to validate all objects before executing the handlers.
        .AddPopValidations()
);


// PopValidations Extensions Function for Registering The Validation Runner
builder.Services.RegisterRunner()
    // And this extension and all the Validators in the same assembly as "SongValidator"
    .RegisterAllMainValidators(typeof(BasicObjectController).Assembly);

```

### Full Documentation

Full documentation can be found at
[https://adamwilliamson.github.io/popvalidations/](https://adamwilliamson.github.io/popvalidations/)

