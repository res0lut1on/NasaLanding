using BackendTestTask.Common.CustomExceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace BackendTestTask.AspNetExtensions.Filters
{
    public class ExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<ExceptionHandlerAttribute> _logger;
        public ExceptionHandlerAttribute(ILogger<ExceptionHandlerAttribute> logger)
        {
            _logger = logger;
        }

        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "Error");

            var response = context.HttpContext.Response;

            if (!response.HasStarted)
            {
                object result;
                int statusCode;

                switch (context.Exception)
                {
                    case NotFoundException:
                        statusCode = StatusCodes.Status404NotFound;
                        result = new { message = "Not found" };
                        break;

                    case ManualException:
                        statusCode = StatusCodes.Status400BadRequest;
                        result = new { message = "Bad Request" };
                        break;

                    default:
                        statusCode = StatusCodes.Status500InternalServerError;
                        result = new { message = "Internal Server Error" };
                        break;
                }

                response.StatusCode = statusCode;
                context.Result = new JsonResult(result);
            }

            await base.OnExceptionAsync(context);
        }

    }
}
