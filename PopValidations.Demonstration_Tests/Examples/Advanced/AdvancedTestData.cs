//using System;

//namespace PopValidations.Demonstration_Tests.Examples.Advanced;

//internal static class AdvancedTestData
//{
//    public static AdvancedAlbum ValidSingleArtist()
//    {
//        return new AdvancedAlbum(
//            "Down with the Sickness",
//            AlbumType.SingleArtist,
//            new AdvancedArtist("Disturbed"),
//            "Disturbed/AlbumOne.jpg",
//            new DateTime(1998, 10, 10),
//            new()
//            {
//                new AdvancedSong(
//                    null,
//                    null,
//                    "Down With The Sickness",
//                    2.4,
//                    string.Empty
//                ),
//                new AdvancedSong(
//                    new AdvancedArtist("Disturbed"),
//                    1,
//                    "Down With The Sickness",
//                    6,
//                    string.Empty
//                ),
//                new AdvancedSong(
//                    null,
//                    1,
//                    "Down With The Sickness",
//                    6,
//                    string.Empty
//                ),
//                null
//            },
//            new()
//            {
//                1998,
//                1999,
//                2000
//            }
//        );
//    }

//    public static AdvancedAlbum InValidSingleArtist()
//    {
//        return new AdvancedAlbum(
//            "Down with the Sickness",
//            AlbumType.SingleArtist,
//            null,
//            "Disturbed/AlbumOne.jpg",
//            new DateTime(1998, 10, 10),
//            new()
//            {
//                new AdvancedSong(
//                    new AdvancedArtist("Disturbed"),
//                    null,
//                    "Down With The Sickness",
//                    2.4,
//                    string.Empty
//                ),
//                new AdvancedSong(
//                    null,
//                    1,
//                    "Down With The Sickness",
//                    6,
//                    string.Empty
//                ),
//                new AdvancedSong(
//                    new AdvancedArtist(null),
//                    1,
//                    "Down With The Sickness",
//                    6,
//                    string.Empty
//                ),
//                null
//            },
//            new()
//            {
//                1998,
//                1999,
//                2000
//            }
//        );
//    }

//    internal static AdvancedAlbum ValidCollaboration()
//    {
//        return new AdvancedAlbum(
//            "Top 10 Hits of 1999",
//            AlbumType.Collaboration,
//            null,
//            "Collaboration/Top10HitsOf1999.jpg",
//            new DateTime(1998, 10, 10),
//            new()
//            {
//                new AdvancedSong(
//                    new AdvancedArtist("Disturbed"),
//                    1,
//                    "Down With The Sickness",
//                    2.4,
//                    "Metal"
//                ),
//                new AdvancedSong(
//                    new AdvancedArtist("Evanescence"),
//                    2,
//                    "Bring Me To Life",
//                    2.4,
//                    "Punk Rock"
//                ),
//                new AdvancedSong(
//                    new AdvancedArtist(null),
//                    1,
//                    "Down With The Sickness",
//                    6,
//                    string.Empty
//                ),
//                new AdvancedSong(
//                    null,
//                    1,
//                    "Down With The Sickness",
//                    6,
//                    string.Empty
//                ),
//                null
//            },
//            new()
//            {
//                1998,
//                1999,
//                2000
//            }
//        );
//    }

//    internal static AdvancedAlbum InValidCollaboration()
//    {
//        return new AdvancedAlbum(
//            "Top 10 Hits of 1999",
//            AlbumType.Collaboration,
//            new AdvancedArtist("Disturbed"),
//            "Collaboration/Top10HitsOf1999.jpg",
//            new DateTime(1998, 10, 10),
//            new()
//            {
//                new AdvancedSong(
//                    null,
//                    1,
//                    "Down With The Sickness",
//                    2.4,
//                    "Metal"
//                ),
//                new AdvancedSong(
//                    null,
//                    2,
//                    "Bring Me To Life",
//                    2.4,
//                    "Punk Rock"
//                ),
//                new AdvancedSong(
//                    new AdvancedArtist(null),
//                    1,
//                    "Down With The Sickness",
//                    6,
//                    string.Empty
//                ),
//                new AdvancedSong(
//                    null,
//                    1,
//                    "Down With The Sickness",
//                    6,
//                    string.Empty
//                ),
//                null
//            },
//            new()
//            {
//                1998,
//                1999,
//                2000
//            }
//        );
//    }
//}