using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IvyScans.API.Models
{
    public class ChapterImage
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public int Order { get; set; }
        public string ChapterId { get; set; }

        public Chapter Chapter { get; set; }
    }
}