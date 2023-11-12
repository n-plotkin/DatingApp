
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        //pass in the properties that make up the primary key
        Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);
        Task<AppUser> GetUserWithLikes(int userId);
        //predicate is: do we want the users they liked, or the users they are liked by?
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }
}