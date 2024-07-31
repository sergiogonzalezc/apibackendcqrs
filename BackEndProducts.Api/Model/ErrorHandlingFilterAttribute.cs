using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace BackEndProducts.Api.Model
{
    public class ErrorHandlingFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<ErrorHandlingFilterAttribute> _logger;

        public ErrorHandlingFilterAttribute(ILogger<ErrorHandlingFilterAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            var problemDetails = new ProblemDetails
            {
                    //context.Exception.Message, // Or a different generic message
                    //context.Exception.Source,
                    //ExceptionType = context.Exception.GetType().FullName,

                Title = "Unhandled error",
                Status = (int)HttpStatusCode.InternalServerError
            };

            // Log the exception
            _logger.LogError("Unhandled exception occurred while executing request: {ex}", context.Exception);

            context.Result = new ObjectResult(problemDetails);

            context.ExceptionHandled = true;

            base.OnException(context);
        }
    }
}
