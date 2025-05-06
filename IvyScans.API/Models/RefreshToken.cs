using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IvyScans.API.Models
{
    public class RefreshToken
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string UserId { get; set; }

        public User User { get; set; }
    }
}