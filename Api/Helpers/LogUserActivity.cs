using Api.Extensions;
using Api.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //Before execute action

            var resultContext = await next();

            // After action has been executed

            if (context.HttpContext.User.Identity?.IsAuthenticated != true) return;

            var userId = resultContext.HttpContext.User.GetUserId();

            var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            var user = await repo.GetUserByIdAsync(userId);
            if (user == null) return;
            user.LastActive = DateTime.UtcNow;
            await repo.SaveAllSync();
        }
    }
}