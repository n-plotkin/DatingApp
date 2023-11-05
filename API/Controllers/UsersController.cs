using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{    
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        //also creates a DataContext, which is a session with our database
        {
            _mapper = mapper;
            _userRepository = userRepository;

        }

        [HttpGet] //http get + route is our request
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _userRepository.GetMembersAsync();
            return Ok(users);
        }

        [HttpGet("{username}")] //put username in curly brackets to idicate flexibility,
        // route is route + {username}
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto){
            /*
            When a request is made to the UsersController 
            the [Authorize] attribute on the controller class 
            instructs ASP.NET Core's authentication middleware to authenticate the user. 
            If the user is authenticated successfully, 
            the middleware constructs a ClaimsPrincipal object called User
            that represents the authenticated user. 
            This object contains all the claims about the 
            user that were established during the authentication process.
            */

            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //any updates to this user will be tracked by ef
            var user = await _userRepository.GetUserByUsernameAsync(username);

            if (user == null) return NotFound();

            //maps our memberUpdateDto to our particular user
            _mapper.Map(memberUpdateDto, user);
            
            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }

    }
}