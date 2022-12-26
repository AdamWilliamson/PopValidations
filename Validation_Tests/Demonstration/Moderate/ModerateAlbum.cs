using System;
using System.Collections.Generic;

namespace Validations_Tests.Demonstration.Moderate;

public record ModerateAlbum(
    string Artist, 
    string CoverImageUrl,
    DateTime Created,
    List<ModerateSong> Songs
);