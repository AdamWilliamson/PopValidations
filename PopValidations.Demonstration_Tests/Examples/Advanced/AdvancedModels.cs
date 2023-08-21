using System;
using System.Collections.Generic;

namespace PopValidations.Demonstration_Tests.Examples.Advanced;

public record AdvancedSong(
    AdvancedArtist? Artist,
    int? TrackNumber,
    string TrackName,
    double Duration,
    string Genre
);

public enum AlbumType
{
    SingleArtist,
    Collaboration
}

public record AdvancedAlbum(
    string? Title,
    AlbumType? Type,
    AdvancedArtist? Artist,
    string? CoverImageUrl,
    DateTime? Created,
    List<AdvancedSong?>? Songs,
    List<int>? PublishedDates
);