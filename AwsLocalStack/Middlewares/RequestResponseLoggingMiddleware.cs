using AwsLocalStack.Helpers;
using System.Text;

namespace AwsLocalStack.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly IConfiguration _configuration;
        private RequestDelegate _next;

        public RequestResponseLoggingMiddleware(IConfiguration configuration, RequestDelegate next)
        {
            _configuration = configuration;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            //Read and log request body data
            string requestBodyPayload = await ReadRequestBody(context.Request);
            LogHelper.RequestPayload = requestBodyPayload;

            //read and log response body data
            //copy a pointer to the original response body stream
            var originalResponseBodyStream = context.Response.Body;

            //create a new memory stream
            using var responseBody = new MemoryStream();
            //.. and use that body for temporary response body
            context.Response.Body = responseBody;

            //continue doen the middleware pipeline, eventually returning to this class
            await _next(context);

            // Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
            await responseBody.CopyToAsync(originalResponseBodyStream);
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            var requestString = string.Empty;
            using (var bodyReader = new StreamReader(request.Body))
            {
                string body = await bodyReader.ReadToEndAsync();
                request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
                requestString = body;
            }

            return requestString;
        }
    }
}
