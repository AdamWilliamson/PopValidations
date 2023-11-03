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
using PopValidations.Swashbuckle;

// PopValidations Extensions Function for Registering The Validation Runner
builder.Services.RegisterRunner()
    // And this extension and all the Validators in the same assembly as "SongValidator"
    .RegisterAllMainValidators(typeof(BasicObjectController).Assembly);

// Register a Pop Validation Config that describes the configuration for describing the validations within OpenApi
builder.Services.RegisterPopValidationsOpenApiDefaults(new WebApiConfig());

// Inside the Swagger generator, you now just need to Add the PopValidation Filter, that will modify the OpenApi Schema
builder.Services.AddSwaggerGen(
    options =>
    {
        // Register PopValidation's Custom API decorations
        options.RegisterOpenApiModificationFilter();
    });
```

### Full Documentation

Full documentation can be found at
[https://adamwilliamson.github.io/popvalidations/](https://adamwilliamson.github.io/popvalidations/)

