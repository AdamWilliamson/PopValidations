using System;
using System.Collections.Generic;

namespace PopValidations_Tests.Demonstration.Advanced;

public enum AlbumType
{
    SingleArtist,
    Collaboration
}

public record AdvancedAlbum(
    string? Title,
    AlbumType? Type,
    string? Artist,
    string? CoverImageUrl,
    DateTime? Created,
    List<AdvancedSong?>? Songs,
    List<int>? PublishedDates
);