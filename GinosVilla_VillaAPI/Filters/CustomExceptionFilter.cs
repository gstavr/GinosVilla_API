using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GinosVilla_VillaAPI.Filters
{
    public class CustomExceptionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if(context.Exception is FileNotFoundException fileNotFoundException)
            {
                context.Result = new ObjectResult("File not found bud handled in filter")
                {
                    StatusCode = 503,
                };

                context.ExceptionHandled = true; // Do not move to other handlers that handle this exception
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
