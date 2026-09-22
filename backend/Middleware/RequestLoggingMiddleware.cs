using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Middleware;

public class RequestLoggingMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<RequestLoggingMiddleware> _logger;

  public RequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestLoggingMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    var stopwatch = Stopwatch.StartNew();

    _logger.LogInformation(
    "Request: {Method} {Path}{QueryString}",
    context.Request.Method,
    context.Request.Path,
    context.Request.QueryString);

    try
    {
      await _next(context);
    }
    catch (Exception ex)
    {
      _logger.LogError(
        ex,
        "Request failed: {Method} {Path}{QueryString}",
        context.Request.Method,
        context.Request.Path,
        context.Request.QueryString);

      throw;
    }

    stopwatch.Stop();

    _logger.LogInformation(
      "Response: {StatusCode} ({ElapsedMs} ms)",
      context.Response.StatusCode,
      stopwatch.ElapsedMilliseconds);
}
}
