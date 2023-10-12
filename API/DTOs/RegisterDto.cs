using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        //even though we call it Username, the API controller understands the JSON is lowercase and connects it
        [Required]
        public string Username  {get; set; }
        [Required]
        [StringLength(8, MinimumLength = 4)]
        public string Password { get; set;}

    }
}