using System;
using System.Collections.Generic;

namespace PopValidations_Tests.Demonstration.Moderate;

public record ModerateAlbum(
    string Artist, 
    string CoverImageUrl,
    DateTime Created,
    List<ModerateSong?> Songs
);