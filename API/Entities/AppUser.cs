using API.Extensions;
using Microsoft.EntityFrameworkCore.Metadata;

namespace API.Entities
{
    public class AppUser
    {
        //entity framework will recognize this as primary key based on convention
        //To make something ELSE the primary key, use [Key] above the property.

        public int Id { get; set; }
        public string UserName { get; set; }

        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public DateOnly DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public List<Photo> Photos { get; set; } = new();
        
        //automapper knows that GetAge here can map to Age in memberDto
        // public int GetAge()
        // {
        //     return DateOfBirth.CalculateAge();
        // }
    }

}