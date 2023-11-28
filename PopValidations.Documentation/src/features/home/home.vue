<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  components: {},
  data() {
    return {
      tab: "one",
    };
  },
});
</script>

<template>
  <v-container fluid>
    <v-card>
      <v-card-title>
        <h2>Pop Validations</h2>
      </v-card-title>
    </v-card>

    <v-card>
      <v-card-text>
        <p>
          Pop Validations helps users describe their objects, with enforceable rules, for .Net.
        </p>
        <p>With automated validation, compatible with ASP.Net, Swashbuckle and MediatR</p>
      </v-card-text>
    </v-card>

    <v-row>
      <v-col>
        <v-tabs v-model="tab">
          <v-tab value="one">Validator</v-tab>
          <v-tab value="two">Error Report</v-tab>
          <v-tab value="three">OpenApi Schema</v-tab>
        </v-tabs>

        <v-window v-model="tab">
          <!-- bg-blue-grey-lighten-4 -->
          <v-window-item value="one" class="bg-darkgrey">
            <CodeWindow
              language="csharp"
              source='
public class BasicSongValidator : AbstractValidator
{
    public BasicSongValidator()
    {
        Describe(x => x.TrackNumber).NotNull();
        Describe(x => x.TrackName).IsEqualTo("Definitely Not The Correct Song Name.");
        Describe(x => x.Duration).IsEqualTo(-1);
        Describe(x => x.Genre).Vitally().NotEmpty().HasLengthBetween(20, 400);
    }
}'
            ></CodeWindow>
          </v-window-item>

          <v-window-item value="two">
            <CodeWindow
              language="json"
              source='
              {
    "errors": {
        "trackNumber": [
            "Is null."
        ],
        "trackName": [
            "Is not equal to &quot;Definitely Not The Correct Song Name.&quot;"
        ],
        "duration": [
            "Song must have a negative duration."
        ]
    }
}'
            ></CodeWindow>
          </v-window-item>

          <v-window-item value="three">
            <CodeWindow
              language="json"
              source='
              {
    "results": [
        {
            "property": "TrackNumber",
            "outcomes": [
                {
                    "validator": "NotNullValidation",
                    "message": "Must not be null."
                }
            ]
        },
        {
            "property": "TrackName",
            "outcomes": [
                {
                    "validator": "IsEqualToValidation",
                    "message": "Must equal to &quot;Definitely Not The Correct Song Name.&quot;",
                    "values": [
                        {
                            "key": "value",
                            "value": "Definitely Not The Correct Song Name."
                        }
                    ]
                }
            ]
        },
        {
            "property": "Duration",
            "outcomes": [
                {
                    "validator": "IsEqualToValidation",
                    "message": "Songs must force you to travel slowly backwards in time to listen to.",
                    "values": [
                        {
                            "key": "value",
                            "value": "-1"
                        }
                    ]
                }
            ]
        }
    ]
}'
            ></CodeWindow>
          </v-window-item>
        </v-window>
      </v-col>
    </v-row>

    <v-card>
      <v-card-title>
        <h3>Why does Pop Validations exist?</h3>
      </v-card-title>
      <v-card-text>
        <p>
          There are other validators out there, a personal favorite of mine has
          been FluentValidation, I've used it for years. But I kept repeatedly
          hitting 2 important issues for me. 
          Describing the validation for external deliverables like OpenApi, or a data dictionary, 
          needed to be seperate from the implementation of the rules.  
          And integrating external data sources into the validation, to check existing values, is extremely difficult.
        </p>
        <p>
          This was designed to solve both of those issues. It is capable of
          bringing in Database and other external data, without immediate or multiple 
          execution. And describe the validations that have would be executed, without executing them,
          and when. Allowing for a rich integration with OpenApi, and an easy
          interaction with external sources for validating data, without querying those external data sources.
        </p>
      </v-card-text>
    </v-card>
  </v-container>
</template>
