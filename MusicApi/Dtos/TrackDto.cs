﻿namespace MusicApi.Dtos
{
    public class TrackDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TrackPath { get; set; } = string.Empty;
        public int LengthInSeconds { get; set; }
        public DateTime ReleaseDate { get; set; }
        // Genre refrencep
        public IEnumerable<GenreDto> Genres { get; set; } = new List<GenreDto>();
    }
}
