using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        //even though we call it Username, the API controller understands the JSON is lowercase and connects it
        [Required] public string Username  {get; set; }
        [Required] public string KnownAs  {get; set; }
        [Required] public string Gender  {get; set; }
        [Required] public DateOnly? DateOfBirth  {get; set; }
        [Required] public string City  {get; set; }
        [Required] public string Country  {get; set; }
        [StringLength(12, MinimumLength = 6)]
        public string Password { get; set;}

    }
}