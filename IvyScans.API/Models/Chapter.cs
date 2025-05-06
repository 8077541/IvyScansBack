using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IvyScans.API.Models
{
    public class Chapter
    {
        public string Id { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string ComicId { get; set; }

        public Comic Comic { get; set; }
        public ICollection<ChapterImage> Images { get; set; }
        public ICollection<ReadingHistory> ReadingHistories { get; set; }
    }
}