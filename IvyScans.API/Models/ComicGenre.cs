using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IvyScans.API.Models
{
    public class ComicGenre
    {
        public string ComicId { get; set; }
        public string GenreId { get; set; }

        public Comic Comic { get; set; }
        public Genre Genre { get; set; }
    }
}