<template>
  <v-container fluid bg-color="surface">
    <v-row>
      <v-col>
        <v-card>
          <v-card-title><h3>MediatR</h3></v-card-title>
          <v-card-text>MediatR is a great quick way to connect peices of code and execute them. One of it's benefits, is it allows you to run modifier code before the
          main code.  These things are called Behaviours, and PopValidations allows you to take advantage of that. <br /> 
          With the extensions provided, a behaviour is added that will run the validations against a MediatR's inputs, and cancel execution if any of the validations fail.<br />
          This way you can incorporate Validation restrictions to execution in your code, with little effort.
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <CodeWindow
          language="csharp"
          source='using PopValidations;
using PopValidations.MediatR;
          
builder.Services.AddMediatR(
    cfg => cfg
        .RegisterServicesFromAssemblyContaining<HomeController>()
        // Pop Validation Extension, that adds a MediatR Behaviour to validate all objects before executing the handlers.
        .AddPopValidations()
);

// PopValidations Extensions Function for Registering The Validation Runner
builder.Services.RegisterRunner()
    // And this extension and all the Validators in the same assembly as "AlbumValidator"
    .RegisterAllMainValidators(typeof(AlbumValidator).Assembly);
'
        ></CodeWindow>
      </v-col>
    </v-row>
  </v-container>
</template>