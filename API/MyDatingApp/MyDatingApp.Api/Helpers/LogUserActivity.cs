using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using MyDatingApp.Api.Data;
using System.Security.Claims;

namespace MyDatingApp.Api.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // we're going to do something after the action inside our controllers has executed the request..
            var resultContext = await next();

            if (context.HttpContext.User.Identity?.IsAuthenticated != true) return;

            var memberId = resultContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var dbContext = resultContext.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();

            await dbContext.Members.Where(x => x.Id == memberId).ExecuteUpdateAsync(setters => setters.SetProperty(x => x.LastActive, DateTime.UtcNow));
        }
    }
}
