using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class SpotifyAccountService : ISpotifyAccountService
    {
        private readonly HttpClient _httpClient;
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string callBack;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISpotifyService _spotifyService;

        public SpotifyAccountService(HttpClient httpClient, IOptions<SpotifySettings> config,
            IServiceProvider serviceProvider, ISpotifyService spotifyService)
        {
            _spotifyService = spotifyService;
            _serviceProvider = serviceProvider;

            clientId = config.Value.ClientId;
            clientSecret = config.Value.ClientSecret;
            callBack = config.Value.CallbackUrl;

            _httpClient = httpClient;
        }

        public async Task<SpotifyAuthResult> GetTokens(string username, string code)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "token");

            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"))
            );

            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "authorization_code"},
                {"code", string.Format(code)},
                {"redirect_uri", callBack}
            });

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            var authResult = await JsonSerializer.DeserializeAsync<SpotifyAuthResult>(responseStream);
            authResult.expires_at = DateTime.UtcNow.AddSeconds(authResult.expires_in);


            Console.WriteLine(response.ToString());
            var user = new AppUser();
            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                user = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            }


            var spotifyData = new SpotifyData
            {
                SpotifyDataUser = user,
                AppUserId = user.Id,
                AppUserUserName = user.UserName,
                AccessToken = authResult.access_token,
                TokenType = authResult.token_type,
                RefreshToken = authResult.refresh_token,
                ExpiresAt = authResult.expires_at
            };

            var artist = await _spotifyService.GetTopArtist(spotifyData.AccessToken, "short_term");

            spotifyData.TopArtist = artist.items.FirstOrDefault().name;
            spotifyData.TopArtistImageUri = artist.items.FirstOrDefault().images.FirstOrDefault().url;


            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                await unitOfWork.UserRepository.UpdateSpotifyData(spotifyData);

                if (unitOfWork.HasChanges()) await unitOfWork.Complete();
            }


            return authResult;
        }


        public async Task Refresh(string refreshOn, string refreshType)
        {

            // Retrieve the user's SpotifyData
            AppUser user = null;
            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                if (refreshType == "token")
                {
                    user = await unitOfWork.UserRepository.GetUserByAccessToken(refreshOn);
                }
                else
                {
                    user = await unitOfWork.UserRepository.GetUserByUsernameAsync(refreshOn);
                }
            }


            if (user != null)
            {
                var spotifyData = user.UserSpotifyData;

                if (spotifyData == null || DateTime.UtcNow <= spotifyData.ExpiresAt)
                {
                    return;
                }

                var request = new HttpRequestMessage(HttpMethod.Post, "token");

                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"))
                );

                request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                    {"grant_type", "refresh_token"},
                    {"refresh_token", spotifyData.RefreshToken}
                    });

                var response = await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                using var responseStream = await response.Content.ReadAsStreamAsync();
                var authResult = await JsonSerializer.DeserializeAsync<SpotifyAuthResult>(responseStream);

                // Update SpotifyData with the new access token and expiration time
                spotifyData.AccessToken = authResult.access_token;
                spotifyData.ExpiresAt = DateTime.UtcNow.AddSeconds(authResult.expires_in);

                var artist = await _spotifyService.GetTopArtist(spotifyData.AccessToken, "short_term");

                spotifyData.TopArtist = artist.items.FirstOrDefault().name;
                spotifyData.TopArtistImageUri = artist.items.FirstOrDefault().images.FirstOrDefault().url;

                var currentlyPlaying = await _spotifyService.GetCurrentlyPlayingTrack(spotifyData.AccessToken);
                if (currentlyPlaying != null)
                {
                    spotifyData.CurrentArtists = JsonSerializer.Serialize(currentlyPlaying.item.artists.Select(artist => artist.name).ToList());
                    spotifyData.CurrentArtistsUris = JsonSerializer.Serialize(currentlyPlaying.item.artists.Select(artist => artist.uri).ToList());
                    spotifyData.CurrentSong = currentlyPlaying.item.name;
                    spotifyData.CurrentSongUri = currentlyPlaying.item.uri;
                }



                // Save the updated SpotifyData (e.g., to the database)

                using (var scope = _serviceProvider.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    await unitOfWork.UserRepository.UpdateSpotifyData(spotifyData);
                    if (unitOfWork.HasChanges()) await unitOfWork.Complete();
                }


                return;
            }
        }



    }
}