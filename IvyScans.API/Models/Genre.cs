using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IvyScans.API.Models
{
    public class Genre
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public ICollection<ComicGenre> ComicGenres { get; set; }
    }
}