using System.Text.Json;

namespace AwsLocalStack.Wrappers
{
    public class Response<T>
    {
        public Response()
        {

        }
        public Response(T? data) { }
        public Response(T data, string message, string code)
        {
            
        }

        public Response(string message)
        {

        }

        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public IDictionary<string, string[]> Errors { get; set; }
        public T? Data { get; set; }
        public string Code { get; set; }
        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
