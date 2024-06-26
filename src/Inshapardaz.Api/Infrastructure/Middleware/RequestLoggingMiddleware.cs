﻿using System.Diagnostics;

namespace Inshapardaz.Api.Infrastructure.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<StatusCodeMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<StatusCodeMiddleware> logger)
    {
        _next = next;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Invoke(HttpContext context)
    {
        Stopwatch sw = new Stopwatch();
        try
        {
            sw.Start();
            _logger.LogInformation($"BGN : {context.Request.Method} {context.Request.Path} ");
            await _next.Invoke(context);
            _logger.LogInformation($"FIN : {context.Request.Method} {context.Request.Path} - {context.Response.StatusCode}. TimeElapsed : {sw.ElapsedMilliseconds} ms");
        }
        catch
        {
            _logger.LogInformation($"Err : {context.Request.Method} {context.Request.Path} - {context.Response.StatusCode}. TimeElapsed : {sw.ElapsedMilliseconds} ms");
        }
    }
}

public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestLoggingMiddleware>();
        return app;
    }
}
