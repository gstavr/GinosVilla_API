using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GinosVilla_VillaAPI.Controllers
{
    [Route("ErrorHandling")]
    [ApiController]
    [AllowAnonymous]
    [ApiVersionNeutral]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorHandlingController : ControllerBase
    {

        [Route("ProcessError")]
        public IActionResult ProcessError([FromServices] IHostEnvironment hostEnvironment){

            if (hostEnvironment.IsDevelopment())
            {
                //Custom Logic
                var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();

                return Problem(
                    detail: feature.Error.InnerException.StackTrace,
                    title: feature.Error.Message,
                    instance: hostEnvironment.EnvironmentName
                    );
            }
            else
            {
                return Problem();
            }
            
        }
    }
}
