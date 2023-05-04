<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  components: {},
  data() {
    return {
    };
  },
});
</script>
<template>
 <v-container fluid bg-color="surface">
    <v-row>
      <v-col>
        <v-card>
          <v-card-title><h3>Is Email</h3></v-card-title>
          <v-card-text>Is Email works on any string. It will report an error if the field is not in a valid email format.
            <br><br> 
            Is email validator uses the MailAddress class to verify if the string is in a correct email format.
            Please note that it is next to impossible to create a perfect email validator.
             As such it is recommended to verify an address by actually sending an email.
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <PanelsOrTabs>
      <template #code>
        <CodeWindow
              language="csharp"
              source='
public class ArtistMainValidator : AbstractValidator<Artist>
{
    public ArtistMainValidator()
    {
        Describe(x => x.Email).IsEmail();
    }
}'
            ></CodeWindow>
      </template>
      <template #errorreport>
        <CodeWindow
              language="json"
              source='{
    "errors": {
        "email": [
            "Is not a valid email"
        ]
    }
}'
            ></CodeWindow>
      </template>
      <template #openapi>
        <CodeWindow
              language="json"
              source='{
    "Artist": {
        "type": "object",
        "properties": {
            "name": {
                "type": "string",
                "nullable": true
            },
            "email": {
                "type": "string",
                "nullable": true
            }
        },
        "additionalProperties": false,
        "x-validation": {
            "email": [
                "Must be a valid email"
            ]
        }
    }
}'
            ></CodeWindow>
      </template>
    </PanelsOrTabs>
    
    <v-row>
      <v-col>
        <v-card>
          <v-card-text>Contributed by Andrew Williamson. <v-btn href="https://github.com/AWilliamson88" flat link><v-icon icon="mdi-github"></v-icon></v-btn></v-card-text>
        </v-card>
      </v-col>
    </v-row>
    
  </v-container>
</template>
