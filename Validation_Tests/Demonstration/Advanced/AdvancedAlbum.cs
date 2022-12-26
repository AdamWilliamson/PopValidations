using System;
using System.Collections.Generic;

namespace PopValidations_Tests.Demonstration.Advanced;

public enum AlbumType
{
    SingleArtist,
    Collaboration,
    Mix
}

public record AdvancedAlbum(    
    AlbumType Type,
    string Artist, 
    string CoverImageUrl,
    DateTime Created,
    List<AdvancedSong?> Songs
);