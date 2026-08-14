using System.Net;
using System.Text.Json;

namespace ezgi_mobilya.Web.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Beklenmeyen bir hata oluştu: {Message}", ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var acceptHeader = context.Request.Headers.Accept.ToString();
            var wantsHtml = acceptHeader.Contains("text/html", StringComparison.OrdinalIgnoreCase);

            if (wantsHtml && !context.Response.HasStarted)
            {
                var redirectPath = context.Request.Path.StartsWithSegments("/Home/SubmitContactMessage")
                    ? "/#contact"
                    : "/Home/Error";
                context.Response.Redirect(redirectPath);
                return;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new Dictionary<string, object?>
            {
                ["StatusCode"] = context.Response.StatusCode,
                ["Message"] = "Sunucu tarafında bir hata oluştu."
            };

            if (_environment.IsDevelopment())
            {
                response["Detailed"] = exception.Message;
            }

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
