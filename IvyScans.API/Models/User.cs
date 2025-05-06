using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IvyScans.API.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime JoinDate { get; set; }
        public string Avatar { get; set; }

        public ICollection<UserBookmark> Bookmarks { get; set; }
        public ICollection<UserRating> Ratings { get; set; }
        public ICollection<ReadingHistory> ReadingHistories { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}