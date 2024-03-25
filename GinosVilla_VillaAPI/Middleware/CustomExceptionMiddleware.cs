using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;

namespace GinosVilla_VillaAPI.Middleware
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public CustomExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await ProcessException(context, ex);

            }
        }

        private async Task ProcessException(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            if (ex is BadImageFormatException badImageFormatException)
            {
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    // If you have custom exception where you are passing status code than you pass here
                    Statuscode = 776,
                    ErrorMessage = "Hello from custom handler! Image format is invalid",

                }));
            }
            else
            {
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    Statuscode = context.Response.StatusCode,
                    ErrorMessage = "Hello From Middlware  - Finale!!!"

                }));
            }
        }
    }
}
