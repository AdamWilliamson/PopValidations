namespace PopValidations_Tests.Demonstration.Advanced;

public record AdvancedSong(
    AdvancedArtist? Artist,
    int? TrackNumber,
    string TrackName,
    double Duration,
    string Genre
);