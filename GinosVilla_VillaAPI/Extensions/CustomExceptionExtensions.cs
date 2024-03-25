using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;

namespace GinosVilla_VillaAPI.Extensions
{
    public static class CustomExceptionExtensions
    {
        public static void HandlerError(this IApplicationBuilder app, bool isDevelopment)
        {
            app.UseExceptionHandler(error =>
            {
                error.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    var feature = context.Features.Get<IExceptionHandlerFeature>();

                    if (feature != null)
                    {
                        if (isDevelopment)
                        {
                            if(feature.Error is BadImageFormatException badImageFormatException)
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
                                    ErrorMessage = feature.Error.Message,
                                    StackTrace = feature.Error.StackTrace,
                                }));
                            }

                            
                        }
                        else
                        {
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                            {
                                Statuscode = context.Response.StatusCode,
                                ErrorMessage = "Hello from Program.cs exception handler"
                            }));
                        }
                    }
                });
            });
        }
    }
}
