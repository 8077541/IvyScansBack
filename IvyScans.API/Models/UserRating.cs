using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IvyScans.API.Models
{
    public class UserRating
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ComicId { get; set; }
        public int Rating { get; set; }

        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User User { get; set; }
        public Comic Comic { get; set; }
    }
}