using DatingApp.DAL.Interfaces;
using DatingApp.Services.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace DatingApp.Sercvices.Middleware
{
    public class LogUserActivity : IAsyncActionFilter
    {
        // Note: ActionExecutingContent is used to do something before the request is completed
        // ActionExecutionDelegate is used to do something after the request is completed
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // This will give us back the context after the request is completed
            var resultContext = await next();

            // This is probably not necessary as we will only be accessing the controller through authenticated endpoints
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) { return; }

            var userId = resultContext.HttpContext.User.GetUserId();
            var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

            var user = await repo.GetUserByIdAsync(int.Parse(userId));
              
            // This will allow us to update the user last active property after the request is completed
            user.LastActive = DateTime.UtcNow;

            await repo.SaveAllAsync();

        }
    }
}
