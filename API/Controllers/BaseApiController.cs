using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    //has everything that gets reused in our controllers
    [ApiController]
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")] // /api/users

    public class BaseApiController : ControllerBase
    {
        
    }
}