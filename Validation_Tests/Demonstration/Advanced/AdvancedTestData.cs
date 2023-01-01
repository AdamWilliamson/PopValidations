using System;

namespace PopValidations_Tests.Demonstration.Advanced
{
    internal static class AdvancedTestData
    {
        public static AdvancedAlbum SingleArtist()
        {
            return new AdvancedAlbum(
                "Down with the Sickness",
                AlbumType.SingleArtist,
                "Disturbed",
                "Disturbed/AlbumOne.jpg",
                new DateTime(1998, 10, 10),
                new()
                {
                    new AdvancedSong(
                        "Disturbed",
                        null,
                        "Down With The Sickness",
                        2.4,
                        string.Empty
                    ),
                    new AdvancedSong(
                        "Disturbed",
                        1,
                        "Down With The Sickness",
                        6,
                        string.Empty
                    )
                },
                new()
                {
                    1998,
                    1999,
                    2000
                }
            );
        }

        internal static AdvancedAlbum Collaboration()
        {
            return new AdvancedAlbum(
                "Top 10 Hits of 1999",
                AlbumType.Collaboration,
                "Various",
                "Collaboration/Top10HitsOf1999.jpg",
                new DateTime(1998, 10, 10),
                new()
                {
                    new AdvancedSong(
                        "Disturbed",
                        1,
                        "Down With The Sickness",
                        2.4,
                        "Metal"
                    ),
                    new AdvancedSong(
                        "Evanescence",
                        2,
                        "Bring Me To Life",
                        2.4,
                        "Punk Rock"
                    )
                },
                new()
                {
                    1998,
                    1999,
                    2000
                }
           );
        }
    }
}
