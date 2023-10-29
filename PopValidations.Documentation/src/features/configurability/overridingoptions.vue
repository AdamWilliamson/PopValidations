<template>
  <v-container fluid bg-color="surface">
    <v-row>
      <v-col>
        <v-card>
          <v-card-title><h3>Overriding Options</h3></v-card-title>
          <v-card-text>
            All validations allow for updating the error message, or
            description. Though some validations allow for tweaking additional
            internal settings. For example, the length validators, allow you to
            change out the length comparitor, so that you can check the length
            of custom objects.
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <CodeWindow
          language="csharp"
          source='
  public class BasicSongValidator : AbstractValidator<InputObject>
  {
      public BasicSongValidator()
      {
          Describe(x => x.Genre)
            .IsEmpty(options => 
              options
                .SetErrorMessage("Non Empty Fields are Invalid.")
                .SetDescription("This Field must be Empty.")
            );
      }
  }'
        ></CodeWindow>
      </v-col>
    </v-row>
  </v-container>
</template>
