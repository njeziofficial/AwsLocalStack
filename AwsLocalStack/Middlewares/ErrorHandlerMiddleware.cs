using AwsLocalStack.Wrappers;
using System.Net;
using System.Text.Json;

namespace AwsLocalStack.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly ILogger<ErrorHandlerMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(ILogger<ErrorHandlerMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async void Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {

                Dictionary<string, string[]> errors = new()
                {
                    {"Error", new string[]{error.Message } }
                };

                if (error.InnerException != null)
                {
                    _logger.LogError(error.InnerException.Message);
                    errors.Add("InnerException", new string[] { error.InnerException.Message });
                }

                var response = context.Response;

                response.ContentType = "application/json";

                var responseModel = new Response<string>()
                {
                    Succeeded = false,
                    Message = "An error occurred",
                    Errors = errors
                };

                string errorCode = "99";

                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseModel.Code = errorCode;
                _logger.LogError(error.Message);

                var result = JsonSerializer.Serialize(responseModel);
                await response.WriteAsync(result);
            }
        }
    }
}
