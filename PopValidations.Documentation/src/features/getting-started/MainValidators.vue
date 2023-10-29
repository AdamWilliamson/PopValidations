<template>
  <v-container fluid bg-color="surface">
    <v-row>
      <v-col>
        <v-card>
          <v-card-title><h3>Main Validators</h3></v-card-title>
          <v-card-text>
            PopValidation seperates types of validators into those that run against the core objects that input into the system, and the sub objects that compose them.<br/>
            The AbstractValidator class wraps validations around the main input class.<br/>
            While AbstractSubValidator class wraps secondary validations around the main input class, and all component classes of the main input class.<br/>
            The ValidationRunner class, takes in all the MainValidators of the Input class, to validate it's values.
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <CodeWindow
          language="csharp"
          source='// A Main Validator, needed to validate input objects
public class AlbumValidator : AbstractValidator<Album>
{
    public AlbumValidator()
    {
        Describe(x => x.Title)
            .IsNotEmpty();

        Describe(x => x.Artist).SetValidator(new ArtistValidator());
    }
}

// A Sub Validator, needed to validate sub objects
public class ArtistValidator : AbstractSubValidator<Artist>
{
    public ArtistValidator()
    {
        Describe(x => x.SongTitle)
            .IsNotEmpty();
    }
}
'
        ></CodeWindow>
      </v-col>
    </v-row>
  </v-container>
</template>