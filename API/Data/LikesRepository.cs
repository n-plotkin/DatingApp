using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public LikesRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
        {
            //we pass them in together as the primary key values. together they serve as primary
            return await _context.Likes.FindAsync(sourceUserId, targetUserId);    
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var predicate = likesParams.Predicate;
            var userId = likesParams.UserId;

            //as queryable means we don't execute anything yet, we store as LINQ queryable.
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if (predicate == "liked")
            {

                //First, we filter the likes collection based on a condition (using Where).
                //Then, we transform the filtered collection into a new form (using Select)
                //In this case, we transform likes into a collection of Users.
                likes = likes.Where(like => like.SourceUserId == userId);
                //select just the target AppUsers
                users = likes.Select(like => like.TargetUser);
            }
            if (predicate == "likedBy")
            {

                likes = likes.Where(like => like.TargetUserId == userId);
                //select just the target AppUsers
                users = likes.Select(like => like.SourceUser);
            }

            var LikedUsers = users.ProjectTo<LikeDto>(_mapper.ConfigurationProvider);

            return await PagedList<LikeDto>.CreateAsync(LikedUsers, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
            .Include(u => u.LikedUsers)
            .Where(u => u.Id == userId).FirstOrDefaultAsync();
        }


    }
}