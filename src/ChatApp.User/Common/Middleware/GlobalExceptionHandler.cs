using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.User.Common.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService
        )
    {
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken ct
    )
    {
        var (statusCode, message) = exception switch
        {
            DbUpdateException => (
                StatusCodes.Status503ServiceUnavailable,
                "Database operation failed. Please try again later."
            ),
            
            HttpRequestException => (
                StatusCodes.Status503ServiceUnavailable,
                "External service is unavailable. Please try again later."
            ),
            
            TimeoutException => (
                StatusCodes.Status504GatewayTimeout,
                "The operation timed out. Please try again."
            ),
            
            OperationCanceledException => (
                StatusCodes.Status499ClientClosedRequest,
                "Request was cancelled"
            ),
            
            _ => (
                StatusCodes.Status500InternalServerError,
                "An internal error occurred. Please contact support."
            )
        };

        context.Response.StatusCode = statusCode;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = exception.GetType().Name,
                Title = "An error occured",
                Detail = message
            }
        });
    }
}