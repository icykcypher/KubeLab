using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace OrchestrationService.Services.GlobalExceptionHandler;

public class GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
{
        public async Task InvokeAsync(HttpContext context)
        {
                try
                {
                        await next(context);
                }
                catch (Exception exception)
                {
                        var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;

                        logger.LogCritical(exception, "Exception occurred. TraceId: {TraceId}", traceId);

                        var status = exception switch
                        {
                                ArgumentException => StatusCodes.Status400BadRequest,
                                KeyNotFoundException => StatusCodes.Status404NotFound,
                                _ => StatusCodes.Status500InternalServerError
                        };

                        var problemDetails = new ProblemDetails
                        {
                                Status = status,
                                Title = "Server Error"
                        };

                        problemDetails.Extensions["traceId"] = traceId;

                        context.Response.ContentType = "application/problem+json";
                        context.Response.StatusCode = status;

                        await context.Response.WriteAsJsonAsync(problemDetails);
                }
        }
}