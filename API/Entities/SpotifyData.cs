using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class SpotifyData
    {
        public AppUser SpotifyDataUser { get; set; }
        [Key]
        public int AppUserId { get; set; }
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public string Scope { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}