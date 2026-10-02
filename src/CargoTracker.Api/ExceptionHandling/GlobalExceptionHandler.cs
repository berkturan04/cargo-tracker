using CargoTracker.Application.Exceptions;
using CargoTracker.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CargoTracker.Api.ExceptionHandling;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService, IHostEnvironment environment)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, isExpected) = MapException(exception);

        if (isExpected)
            _logger.LogWarning(exception, "İstenen işlem reddedildi: {Message}", exception.Message);
        else
            _logger.LogError(exception, "Beklenmeyen hata. TraceId: {TraceId}", httpContext.TraceIdentifier);

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = isExpected ? exception.Message : "Beklenmeyen bir hata oluştu.",
            Detail = isExpected || _environment.IsDevelopment() ? exception.Message : null,
            Instance = httpContext.Request.Path
        };

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }

    private static (int StatusCode, bool IsExpected) MapException(Exception exception) => exception switch
    {
        EmailAlreadyInUseException => (StatusCodes.Status409Conflict, true),
        InvalidCredentialsException => (StatusCodes.Status401Unauthorized, true),
        InvalidShipmentStatusTransitionException => (StatusCodes.Status400BadRequest, true),
        InvalidShipmentOperationException => (StatusCodes.Status400BadRequest, true),
        UnauthorizedAccessException => (StatusCodes.Status403Forbidden, true),
        ArgumentException => (StatusCodes.Status400BadRequest, true),
        _ => (StatusCodes.Status500InternalServerError, false)
    };
}
