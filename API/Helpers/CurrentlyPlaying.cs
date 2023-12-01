using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class CurrentlyPlaying
    {
        public string CurrentSong { get; set; }
        public string CurrentArtists { get; set; }
        public string CurrentSongUri { get; set; }
        public string CurrentArtistsUris { get; set; }

    }
}