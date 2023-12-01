using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;
using API.Helpers.Artist;
using API.Helpers.Song;

namespace API.Interfaces
{
    public interface ISpotifyService
    {
        Task<API.Helpers.Artist.Artist> GetTopArtist(string accessToken, string term);
        Task<API.Helpers.Song.Song> GetCurrentlyPlayingTrack(string accessToken);

    }
}