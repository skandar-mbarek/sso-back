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
        context.Response.ContentType="application/json";
        context.Response.StatusCode = e switch
        {
            ArgumentException => (int)HttpStatusCode.NotFound,
            BadHttpRequestException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };
        var response = new
        {
            StatusCode = 500,
            Message = e.Message,
            Methode = context.Request.Method,
            Path = context.Request.Path.Value,
        };
        var json = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(json);
    }

}