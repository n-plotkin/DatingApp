using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();


            //resultcontext lets us get User, userRepository. This service is outside the main program some ways.
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

             var userId = resultContext.HttpContext.User.GetUserId();

             var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
             var user = await repo.GetUserByIdAsync(int.Parse(userId));
             user.LastActive = DateTime.UtcNow;
             await repo.SaveAllAsync();

        }
    }
}