using AwsLocalStack.Models;
using Newtonsoft.Json;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;

namespace AwsLocalStack.Helpers
{
    public class LogHelper
    {
        public static string RequestPayload = string.Empty;
        public static string RequestId = string.Empty;
        public static string ProductId = string.Empty;

        public static async void EnrichFromRequest(IDiagnosticContext diagnosticContext,HttpContext httpContext)
        {
            var request = httpContext.Request;
            var response = httpContext.Response;

            diagnosticContext.Set("ResponseBody", RequestPayload);
            diagnosticContext.Set("RequestId", RequestId);
            diagnosticContext.Set("ProductId", ProductId);

            string responseBodyPayload  = await ReadResponseBody(response);

            diagnosticContext.Set("ResponseBody", responseBodyPayload);

            ApiResponse responseBody = (ApiResponse)JsonConvert.DeserializeObject(responseBodyPayload)!;

            if (responseBody != null)
            {
                diagnosticContext.Set("ResponseCode", responseBody.Code);
                diagnosticContext.Set("Description", responseBody.Description);
            }

            //set all the common properties available for every request
            string clientId = request.Headers["client_id"];
            //string correlationId = request.Headers["x-correction-id"];

            diagnosticContext.Set("Host", request.Host);
            diagnosticContext.Set("Protocol", request.Protocol);
            diagnosticContext.Set("Scheme", request.Host);
            diagnosticContext.Set("Host", request.Scheme);
            diagnosticContext.Set("ClientId", clientId);

            //only set if available, no sensitive data to be sent
            if (request.QueryString.HasValue)
            {
                diagnosticContext.Set("QueryString", request.QueryString.Value);
            }

            //set the content-type of the response at this point
            diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

            //retrieve the IEndpointFeature selected for the request
            var endpoint = httpContext.GetEndpoint();
            if (endpoint is object) // endpoint != null
            {
                diagnosticContext.Set("EndpointName", endpoint.DisplayName);
            }
        }

        private static async Task<string> ReadResponseBody(HttpResponse response)
        {
            bool isConsumed = IsResponseBodyConsumed(response);

            if (!isConsumed)
            {
                response.Body.Seek(0, SeekOrigin.Begin);
                string responseBody = await new StreamReader(response.Body, encoding: Encoding.Default).ReadToEndAsync();
                response.Body.Seek(0, SeekOrigin.Begin);

                return $"{responseBody}";
            }

            return null;
        }

        private static bool IsResponseBodyConsumed(HttpResponse response)
        {
            bool isSeekableStream = response.Body.CanSeek;
            bool canReadStream = response.Body.CanRead;

            return !isSeekableStream || !canReadStream;
        }
    }
}
