using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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

        public SpotifyAccountService(HttpClient httpClient, IOptions<SpotifySettings> config)
        {
            
            clientId = config.Value.ClientId; 
            clientSecret = config.Value.ClientSecret;
            callBack = config.Value.CallbackUrl;

            _httpClient = httpClient;
        }

        public async Task<SpotifyAuthResult> GetTokens(string code)
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

            return authResult;
        }
    }
}