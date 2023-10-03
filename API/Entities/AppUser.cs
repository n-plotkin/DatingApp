namespace API.Entities
{

public class AppUser
    {
        public int Id { get; set; } 
        //entity framework is convention based
        //entity framework will recognize this as primary key.
        //To make something ELSE the primary key, use [Key] above the property.
        public string UserName { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }
    }
}