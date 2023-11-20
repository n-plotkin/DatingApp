using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly IUnitOfWork _uow;

        public UsersController(IUnitOfWork uow, IMapper mapper,
        IPhotoService photoService)
        //also creates a DataContext, which is a session with our database
        {
            _uow = uow;
            _photoService = photoService;
            _mapper = mapper;

        }

        [HttpGet] //http get + route is our request
        //route is http:url./users/
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            var gender = await _uow.UserRepository.GetUserGender(User.GetUsername());

            userParams.CurentUsername = User.GetUsername();

            userParams.Gender = gender == "male" ? "female" : "male";

            var users = await _uow.UserRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, 
                users.TotalCount, users.TotalPages));
            return Ok(users);
        }

        [Authorize(Roles = "Member")]
        [HttpGet("{username}")] //put username in curly brackets to idicate flexibility,
        // route is route + {username}
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _uow.UserRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
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

            //any updates to this user will be tracked by ef
            var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user == null) return NotFound();

            //maps our memberUpdateDto to our particular user
            _mapper.Map(memberUpdateDto, user);

            if (await _uow.Complete()) return NoContent();

            return BadRequest("Failed to update user");
        }
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0) photo.IsMain = true;

            user.Photos.Add(photo);

            if (await _uow.Complete())
                return CreatedAtAction(nameof(GetUser), new { username = user.UserName },
                    _mapper.Map<PhotoDto>(photo));

            return BadRequest("Problem adding photo");
        }


        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("You cannot delete your main photo.");

            //there are seeded photos which don't have PublicIds and aren't in cloudinary
            if (photo.PublicId != null)
            {
                //this deletes from cloudinary
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }
            //this removes from ef
            user.Photos.Remove(photo);

            if (await _uow.Complete()) return Ok();

            return BadRequest("Problem saving or deleting photo.");

        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            //again, ef is tracking the changes to the photo entities, so now we just save
            if (await _uow.Complete()) return NoContent();
            return BadRequest("Problem setting the main photo.");
 
        }

    }
}