using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.Controllers
{
    public class SpotifyController : BaseApiController
    {
        private readonly ISpotifyAccountService _spotifyAuth;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISpotifyService _spotifyService;

        public SpotifyController(ISpotifyAccountService spotifyAuth, IMapper mapper, IUnitOfWork unitOfWork, ISpotifyService spotifyService)
        {
            _spotifyService = spotifyService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _spotifyAuth = spotifyAuth;
        }


        [Authorize]
        [HttpPost("auth")] // POST: api/account/register?username=dave&password=
        public async Task<ActionResult> Authorize(CodeDto codeDto)
        {

            var username = User.GetUsername();
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);


            var response = await _spotifyAuth.GetTokens(username, codeDto.Code);

            return Ok();

            //var spotifydata = _mapper.Map<SpotifyData>(response);
        }

        [HttpGet("me")] // POST: api/spotify/me
        public async Task<ActionResult<String[]>> GetCurrentSpotifyData()
        {

            var username = User.GetUsername();
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

            var response = new string[] {};
            if (user.UserSpotifyData != null)
            {
                response = new string[] 
                    {
                        user.UserSpotifyData.TopArtist,
                        user.UserSpotifyData.TopArtistImageUri,
                        user.UserSpotifyData.CurrentArtists,
                        user.UserSpotifyData.CurrentArtistsUris, 
                        user.UserSpotifyData.CurrentSong, 
                        user.UserSpotifyData.CurrentSongUri, 
                    };
            }

            return Ok(response);

            //var spotifydata = _mapper.Map<SpotifyData>(response);


        }

    }
}