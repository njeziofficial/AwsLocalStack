using AwsLocalStack.Helpers;
using AwsLocalStack.Middlewares;
using Serilog;

namespace AwsLocalStack.Extensions
{
    public static class BuilderExtensions
    {
        public static void ExtendBuilder(this IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseSerilogRequestLogging(opts => opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest);
            app.UseMiddleware<ErrorHandlerMiddleware>();
            loggerFactory.AddSerilog();
        }
    }
}
