using Microsoft.AspNetCore.Http;

namespace SharedLibrary
{
    public class RestrictAccessMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var referrer = context.Request.Headers["Referrer"].FirstOrDefault();
            if (string.IsNullOrEmpty(referrer))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Hmmm, je n'arrive pas à accéder à cette page");
                return;
            }
            else
            {
                await next(context);
            }
        }
    }
}
