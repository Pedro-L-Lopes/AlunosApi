using System.Data;
using System.Linq.Expressions;
using System.Net;
using System.Text.Json;

namespace AlunosApi.Middlewares
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public GlobalErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            string stackTrace = String.Empty;
            string message;
            var exceptionType = exception.GetType();

            if (exceptionType == typeof(HttpRequestException))
            {
                message = exception.Message;
                status = HttpStatusCode.BadRequest;
                stackTrace = exception.StackTrace;
            }
            else
            {
                message = exception.Message;
                status = HttpStatusCode.InternalServerError;
                stackTrace = exception.StackTrace;
            }

            var result = JsonSerializer.Serialize(new { status, message, stackTrace });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            return context.Response.WriteAsync(result);
        }
    }
}
