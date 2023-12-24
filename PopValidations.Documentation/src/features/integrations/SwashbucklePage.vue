<template>
      <v-container fluid bg-color="surface">
    <v-row>
      <v-col>
        <v-card>
          <v-card-title><h3>Swashbuckle</h3></v-card-title>
          <v-card-text>Pop Validations, using the PopValidations.Swashbuckle library, can convert validators into OpenApi descriptions.
          <br/>
          The Swashbuckle integration, once activated correctly, will automatically add the conversions for any object registered with a AbstractValidator.<br />
          There is a potential thing to watch for, due to OpenApi. It doesn't support very well, describing the same object with different validations dependant on the api end point.
          It is recommended to use this correctly, you ensure you use unique objects for each end point, or that they must share validation.
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
    
    <v-row>
      <v-col>
        <v-card>
          <v-card-text>
            <a href="https://www.nuget.org/packages/PopValidations.Swashbuckle">https://www.nuget.org/packages/PopValidations.Swashbuckle</a><br />
            Or via CLI<br />
            dotnet add package PopValidations.Swashbuckle --version 1.0.0
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <CodeWindow
          language="csharp"
          source='using PopValidations;
using PopValidations.Swashbuckle;
//...
// When Adding Controllers, You can convert exceptions caused by Validations, into WebApi Validation Errors.
builder.Services.AddControllers(
    // This Filter provided by PopValidations converts exceptions into 422 Validation Errors for WebApi.
    options => options.Filters.Add<PopValidationExceptionFilter>()
);

// PopValidations Extensions Function for Registering The Validation Runner
builder.Services.RegisterRunner()
    // And this extension and all the Validators in the same assembly as "AlbumValidator"
    .RegisterAllMainValidators(typeof(AlbumValidator).Assembly);

// Register a Pop Validation Config that describes the configuration for describing the validations within OpenApi
builder.Services.RegisterPopValidationsOpenApiDefaults(new WebApiConfig());

// Inside the Swagger generator, you now just need to Add the PopValidation Filter, that will modify the OpenApi Schema
builder.Services.AddSwaggerGen(
    options =>
    {
        // Register PopValidations Custom API decorations
        options.RegisterOpenApiModificationFilter();
    });'
        ></CodeWindow>
      </v-col>
    </v-row>
  </v-container>
</template>