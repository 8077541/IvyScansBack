using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IvyScans.API.Models
{
    public class ReadingHistory
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ComicId { get; set; }
        public string ChapterId { get; set; }

        public int ChapterNumber { get; set; }
        public DateTime LastReadAt { get; set; }

        public User User { get; set; }
        public Comic Comic { get; set; }
        public Chapter Chapter { get; set; }
    }
}