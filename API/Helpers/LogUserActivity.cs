
using API.Extensions;
using API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context,
         ActionExecutionDelegate next)
        // we can do something after or before (next)
        {
            var resultContext = await next(); // after completing api action
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;
            var userId = resultContext.HttpContext.User.GetUserId();
            var repository = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            var user = await repository.GetUserByIdAsync(int.Parse(userId));

            // to update user last active
            user.LastActive = DateTime.UtcNow;
            await repository.SaveAllAsync();
        }
    }
}