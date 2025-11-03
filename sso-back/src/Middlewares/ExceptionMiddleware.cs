using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace sso_back.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            _logger.LogError($"Execption : {e.Message},StackTrace:{e.StackTrace}");
            await HandleExceptionAsync(context, e);
        }
    }

private Task HandleExceptionAsync(HttpContext context, Exception e)
{
    context.Response.ContentType = "application/json";
    context.Response.StatusCode = e switch
    {
        UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
        System.Security.SecurityException => (int)HttpStatusCode.Forbidden,
        
        ArgumentNullException => (int)HttpStatusCode.BadRequest,
        ArgumentException => (int)HttpStatusCode.BadRequest,
        
        KeyNotFoundException => (int)HttpStatusCode.NotFound,
        FileNotFoundException => (int)HttpStatusCode.NotFound,
        DirectoryNotFoundException => (int)HttpStatusCode.NotFound,
        
        
        InvalidOperationException => (int)HttpStatusCode.Conflict,
        NotImplementedException => (int)HttpStatusCode.NotImplemented,
        TimeoutException => (int)HttpStatusCode.RequestTimeout,
        BadHttpRequestException => (int)HttpStatusCode.BadRequest,
        
        // Default
        _ => (int)HttpStatusCode.InternalServerError
    };

    // For security reasons, you might want to hide internal exception details in production
    var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
    var message = isDevelopment ? e.Message : "An error occurred. Please try again later.";

    var response = new
    {
        StatusCode = context.Response.StatusCode,
        Message = message,
        DetailedMessage = isDevelopment ? e.Message : null,
        ExceptionType = isDevelopment ? e.GetType().Name : null,
        Method = context.Request.Method,
        Path = context.Request.Path.Value,
        Timestamp = DateTime.UtcNow
    };

    var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = isDevelopment
    });
    
    return context.Response.WriteAsync(json);
}
}