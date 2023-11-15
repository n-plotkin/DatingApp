using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class AppUserRole : IdentityUserRole<int>
{
        //represents join table between AppUser and AppRole

    public AppUser User { get; set; }
    public AppRole Role { get; set; }
}