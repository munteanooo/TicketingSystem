using System.Net;
using System.Text.Json;
using TicketingSystem.Application.Contracts.Exceptions;

namespace TicketingSystem.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger; // Logger adăugat

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Logăm eroarea cu detalii (stack trace) pentru noi, programatorii
                _logger.LogError(ex, "O excepție a fost prinsă de Middleware: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                ValidationException => HttpStatusCode.BadRequest,
                NotFoundException => HttpStatusCode.NotFound,
                ForbiddenException => HttpStatusCode.Forbidden,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new
            {
                status = context.Response.StatusCode,
                error = exception.Message,
                type = exception.GetType().Name
                // Opțional: poți adăuga și stackTrace doar dacă ești în Development
            });

            return context.Response.WriteAsync(result);
        }
    }
}