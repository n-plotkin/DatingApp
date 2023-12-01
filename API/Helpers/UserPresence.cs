using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Helpers
{
    public class UserPresence
    {
        public List<string> ConnectionIds { get; set; } = new List<string>();
        public AppUser User { get; set; } // Optional SpotifyData

    }
}