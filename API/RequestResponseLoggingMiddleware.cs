// using System.Text;

// public class RequestResponseLoggingMiddleware
// {
//     private readonly RequestDelegate _next;
//     private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

//     public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
//     {
//         _next = next;
//         _logger = logger;
//     }

//     public async Task InvokeAsync(HttpContext context)
//     {
//         // Log the request
//         context.Request.EnableBuffering();
//         var requestBody = await ReadRequestBodyAsync(context.Request);
//         _logger.LogInformation("Incoming request: {Method} {Path} {Body}", context.Request.Method, context.Request.Path, requestBody);

//         // Capture the response
//         var originalBodyStream = context.Response.Body;
//         using var responseBodyStream = new MemoryStream();
//         context.Response.Body = responseBodyStream;

//         await _next(context);

//         // Log the response
//         context.Response.Body.Seek(0, SeekOrigin.Begin);
//         var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
//         context.Response.Body.Seek(0, SeekOrigin.Begin);
//         _logger.LogInformation("Outgoing response: {StatusCode} {Body}", context.Response.StatusCode, responseBody);

//         await responseBodyStream.CopyToAsync(originalBodyStream);
//     }

//     private async Task<string> ReadRequestBodyAsync(HttpRequest request)
//     {
//         using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
//         var body = await reader.ReadToEndAsync();
//         request.Body.Position = 0; // Reset the stream position
//         return body;
//     }
// }
