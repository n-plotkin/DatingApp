
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using API.Data;
using API.Helpers;
using API.Helpers.Artist;
using API.Helpers.Song;
using API.Interfaces;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class SpotifyService : ISpotifyService
    {

        private readonly HttpClient _httpClient;
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string callBack;

        public SpotifyService(HttpClient httpClient, IOptions<SpotifySettings> config)
        {

            clientId = config.Value.ClientId;
            clientSecret = config.Value.ClientSecret;
            _httpClient = httpClient;
        }

        public async Task<Helpers.Artist.Artist> GetTopArtist(string accessToken, string term)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync($"me/top/artists?time_range={term}&limit=1");
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();

            var responseObject = await JsonSerializer.DeserializeAsync<Helpers.Artist.Artist>(responseStream);

            Console.WriteLine(responseObject.items.FirstOrDefault().name);

            return responseObject;
        }

        public async Task<Helpers.Song.Song> GetCurrentlyPlayingTrack(string accessToken)
        {

            var request = new HttpRequestMessage(HttpMethod.Get, "me/player/currently-playing?market=ES");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                Console.WriteLine("No currently playing track.");
                return null; // Or however you wish to handle this case
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Console.WriteLine("Unauthorized access. Check the access token.");
                // Handle the unauthorized access as per your application's logic
                return null;
            }



            response.EnsureSuccessStatusCode();


            using var responseStream = await response.Content.ReadAsStreamAsync();
            var responseObject = await JsonSerializer.DeserializeAsync<Song>(responseStream);

            return responseObject;
        }
    }
}