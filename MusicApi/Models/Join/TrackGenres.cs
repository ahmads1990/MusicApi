﻿namespace MusicApi.Models.Join
{
    public class TrackGenres
    {
        public int TrackId { get; set; }
        public Track Track { get; set; } = default!;

        public int GenreId { get; set; }
        public Genre Genre { get; set; } = default!;
    }
}
