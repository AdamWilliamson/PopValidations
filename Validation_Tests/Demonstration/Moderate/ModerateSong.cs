namespace Validations_Tests.Demonstration.Moderate;

public record ModerateSong(
    string Artist,
    int? TrackNumber,
    string TrackName,
    double Duration,
    string Genre
);