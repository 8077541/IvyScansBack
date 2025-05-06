namespace IvyScans.API.Models

{
    public class Comic
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Cover { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Artist { get; set; }
        public DateTime Released { get; set; }
        public string Status { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsFeatured { get; set; }

        public ICollection<ComicGenre> ComicGenres { get; set; }
        public ICollection<Chapter> Chapters { get; set; }
        public ICollection<UserBookmark> Bookmarks { get; set; }
        public ICollection<UserRating> Ratings { get; set; }
        public ICollection<ReadingHistory> ReadingHistories { get; set; }
    }
}