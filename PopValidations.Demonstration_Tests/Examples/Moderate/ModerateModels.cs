using System;
using System.Collections.Generic;

namespace PopValidations.Demonstration_Tests.Examples.Moderate;

public record ModerateSong(
    string Artist,
    int? TrackNumber,
    string TrackName,
    double Duration,
    string Genre
);

public record ModerateAlbum(
    string Artist,
    string CoverImageUrl,
    DateTime Created,
    List<ModerateSong?> Songs
);