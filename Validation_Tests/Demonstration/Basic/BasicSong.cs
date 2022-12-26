namespace Validations_Tests.Demonstration.Basic;

public record BasicSong(
    string Artist,
    int? TrackNumber,
    string TrackName,
    double Duration,
    string Genre
);