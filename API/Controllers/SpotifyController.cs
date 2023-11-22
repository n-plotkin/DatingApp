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
    
        public SpotifyController(ISpotifyAccountService spotifyAuth, IMapper mapper)
        {
            _mapper = mapper;
            _spotifyAuth = spotifyAuth;
        }


        [Authorize]
        [HttpPost("auth")] // POST: api/account/register?username=dave&password=
        public async Task<ActionResult> Authorize(CodeDto codeDto)
        {
            
            Console.WriteLine("HELLO");
            var username = User.GetUsername();
            

            var response = await _spotifyAuth.GetTokens(codeDto.Code);

            Console.WriteLine("HELLO2");

            Console.WriteLine(response.ToString());

            return Ok();

            

            //var spotifydata = _mapper.Map<SpotifyData>(response);




        }

    }
}