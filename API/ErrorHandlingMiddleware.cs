using System.Net;
using System.Text;
using System.Text.Json;

public class ErrorHandlingMiddleware : IMiddleware
{
    // private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    // public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    // {
    //     _next = next;
    //     _logger = logger;
    // }
    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {

            var userAgent = context.Request.Headers["User-Agent"].ToString();
            _logger.LogInformation($"API called from source: {userAgent}");
            // Log the request
            context.Request.EnableBuffering();
            var requestBody = await ReadRequestBodyAsync(context.Request);
            _logger.LogInformation("Incoming request: {Method} {Path} {Body}", context.Request.Method, context.Request.Path, requestBody);

            // Capture the response
            var originalBodyStream = context.Response.Body;
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            await next(context);


            // Log the response
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            _logger.LogInformation("Outgoing response: {StatusCode} {Body}", context.Response.StatusCode, responseBody);

            await responseBodyStream.CopyToAsync(originalBodyStream);

            // var userAgent = context.Request.Headers["User-Agent"].ToString();
            // _logger.LogInformation($"API called from source: {userAgent}");
            // Log the request 
            // var request = await FormatRequest(context.Request);
            // _logger.LogInformation($"Request: {request}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred.");
            await HandleExceptionAsync(context, ex);
            //  _logger.LogError($"An error occurred:{result}");
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // context.Response.ContentType = "application/json";
        // context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // var response = new { message = "Internal Server Error", details = exception.Message };
        // return context.Response.WriteAsJsonAsync(response);
        var response = context.Response;
        response.ContentType = "application/json";

        switch (exception)
        {
            case AppException e:
                // custom application error
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            case KeyNotFoundException e:
                // not found error
                response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
            default:
                // unhandled error
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        var result = JsonSerializer.Serialize(new { message = exception?.Message });
        return response.WriteAsync(result);
    }

    // private async Task<string> FormatRequest(HttpRequest request)
    // {
    //     request.EnableBuffering(); // Enable buffering to allow multiple reads

    //     using (var reader = new StreamReader(request.Body, leaveOpen: true))
    //     {
    //         var bodyAsText = await reader.ReadToEndAsync(); // Read the body as a string
    //         request.Body.Position = 0; // Reset the stream position to the beginning
    //         return $"{request.Method} {request.Path}{request.QueryString} {bodyAsText}";
    //     }
    // }

    // public Task InvokeAsync(HttpContext context, RequestDelegate next)
    // {
    //     throw new NotImplementedException();
    // }
    private async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0; // Reset the stream position
        return body;
    }
    public class AppException : Exception
    {
        public AppException(string message) : base(message) { }
    }
}