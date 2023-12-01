
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;


//This is a service, so we make it available in ApplicationServiceExtensions.cs
namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await _context.Users
                //Automapper handles "eager loading" of entities: 
                //meaning we don't have to specific we need to .include our photos
                .Where(x => x.UserName == username)
                //automapper handles for us, we pass configuration provider so it knows
                //where to find our mapping profiles, which it gets from the service
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task UpdateSpotifyData(SpotifyData spotifyData)
        {
            var user = await _context.Users
                         .Include(u => u.UserSpotifyData)
                         .FirstOrDefaultAsync(u => u.Id == spotifyData.AppUserId);

            if (user == null)
            {
                // Handle the case where the user is not found
                throw new InvalidOperationException("User not found.");
            }

            if (user.UserSpotifyData != null)
            {
                // If the user already has SpotifyData, update it
                _context.Entry(user.UserSpotifyData).CurrentValues.SetValues(spotifyData);
            }
            else
            {
                // If the user does not have SpotifyData, add it
                user.UserSpotifyData = spotifyData;
            }

            // Save changes in the context
            await _context.SaveChangesAsync();

        }
        public async Task UpdateCurrentlyPlaying(string username, CurrentlyPlaying currentlyPlaying)
        {
            var user = await _context.Users
                         .Include(u => u.UserSpotifyData)
                         .FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
            {
                // Handle the case where the user is not found
                throw new InvalidOperationException("User not found.");
            }

            if (user.UserSpotifyData != null)
            {
                user.UserSpotifyData.CurrentSong = currentlyPlaying.CurrentSong;
                user.UserSpotifyData.CurrentSongUri = currentlyPlaying.CurrentSongUri;
                user.UserSpotifyData.CurrentArtists = currentlyPlaying.CurrentArtists;
                user.UserSpotifyData.CurrentArtistsUris = currentlyPlaying.CurrentArtistsUris;
            }

            // Save changes in the context
            await _context.SaveChangesAsync();

        }





        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();

            query = query.Where(u => u.UserName != userParams.CurentUsername);
            query = query.Include(s => s.UserSpotifyData);

            var currentuser = await _context.Users
                .Include(u => u.UserSpotifyData)
                .FirstOrDefaultAsync(u => u.UserName == userParams.CurentUsername);

            if (currentuser?.UserSpotifyData != null)
            {
                if (userParams.TypeOf == "song")
                {
                    query = query.Where(u => u.UserSpotifyData != null &&
                                             u.UserSpotifyData.CurrentSongUri == currentuser.UserSpotifyData.CurrentSongUri);
                }
                else
                {
                    query = query.Where(u => u.UserSpotifyData != null &&
                                             u.UserSpotifyData.TopArtist == currentuser.UserSpotifyData.TopArtist);
                }
            }
            else
            {
                return await PagedList<MemberDto>.CreateAsync(Enumerable.Empty<MemberDto>().AsQueryable(), userParams.PageNumber, userParams.PageSize);
            }


            //var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            //var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));


            //query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            return await PagedList<MemberDto>.CreateAsync(
            query.AsNoTracking().ProjectTo<MemberDto>(_mapper.ConfigurationProvider),
            userParams.PageNumber,
            userParams.PageSize);
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }



        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            //If we want not only user, but also the "related data" such as photos
            //We have to "Eager load" the entity
            //Do this by chaining a .Include

            return await _context.Users
            .Include(p => p.Photos)
            .Include(s => s.UserSpotifyData)
            .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<AppUser> GetUserByAccessToken(string accessToken)
        {
            return await _context.Users
            .Include(s => s.UserSpotifyData)
            .SingleOrDefaultAsync(x => x.UserSpotifyData.AccessToken == accessToken);
        }


        public async Task<string> GetUserGender(string username)
        {
            return await _context.Users.Where(x => x.UserName == username)
                .Select(x => x.Gender).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
            .Include(p => p.Photos)
            .ToListAsync();
        }

        public void Update(AppUser user)
        {
            //informing ef that entity has been updated. Ef automatically does this though.
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}