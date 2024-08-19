using BackEndProducts.Common;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using static BackEndProducts.Common.Enum;

namespace BackEndProducts.Application.Exceptions.Handler;
public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        //logger.LogError("Error Message: {exceptionMessage}, Time of occurrence {time}",            exception.Message, DateTime.UtcNow);
        ServiceLog.Write(LogType.WebSite, exception, nameof(TryHandleAsync), "Error");

        (string Detail, string Title, int StatusCode) details = exception switch
        {
            InternalServerException =>
            (
                exception.Message,
                exception.GetType().Name,
                context.Response.StatusCode = StatusCodes.Status500InternalServerError
            ),
            ValidationException =>
            (
                exception.Message,
                exception.GetType().Name,
                context.Response.StatusCode = StatusCodes.Status400BadRequest
            ),
            BadRequestException =>
            (
                exception.Message,
                exception.GetType().Name,
                context.Response.StatusCode = StatusCodes.Status400BadRequest
            ),
            NotFoundException =>
            (
                exception.Message,
                exception.GetType().Name,
                context.Response.StatusCode = StatusCodes.Status404NotFound
            ),
            _ =>
            (
                exception.Message,
                exception.GetType().Name,
                context.Response.StatusCode = StatusCodes.Status500InternalServerError
            )
        };

        var problemDetails = new ProblemDetails
        {
            Title = details.Title,
            Detail = details.Detail,
            Status = details.StatusCode,
            Instance = context.Request.Path
        };

        problemDetails.Extensions.Add("traceId", context.TraceIdentifier);

        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions.Add("ValidationErrors", validationException.Errors);
        }

        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
        return true;
    }

    //public sealed class ValidationExceptionHandlingMiddleware
    //{
    //    private readonly RequestDelegate _next;
    //    private readonly ILogger<CustomExceptionHandler> _logger;

    //    public ValidationExceptionHandlingMiddleware(RequestDelegate next, ILogger<CustomExceptionHandler> logger)
    //    {
    //        _next = next;
    //        _logger = logger;
    //    }

    //    public async Task InvokeAsync(HttpContext context)
    //    {
    //        try
    //        {
    //            await _next(context);
    //        }
    //        catch (FluentValidation.ValidationException exception)
    //        {
    //            var problemDetails = new ProblemDetails
    //            {
    //                Status = StatusCodes.Status400BadRequest,
    //                Type = "ValidationFailure",
    //                Title = "Validation error",
    //                Detail = "One or more validation errors has occurred"
    //            };

    //            if (exception.Errors is not null)
    //            {
    //                problemDetails.Extensions["errors"] = exception.Errors;
    //            }

    //            context.Response.StatusCode = StatusCodes.Status400BadRequest;

    //            await context.Response.WriteAsJsonAsync(problemDetails);
    //        }
    //    }
    //}
}
