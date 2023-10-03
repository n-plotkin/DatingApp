using API.Entities;

namespace API.Interfaces
{
    public interface ITokenService
    {
        //Naturally, any class which implements this contract must implement CreateToken
        string CreateToken(AppUser user);
    }
}