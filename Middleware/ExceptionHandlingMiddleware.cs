using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AuthenticationTask.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;   
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
                //if (ex.InnerException!=null)
                //{
                //    _logger.LogError("{ExceptionType}{ExceptionMessage}",
                //                        ex.InnerException.GetType().ToString(),
                //                        ex.InnerException.Message);
                //}
                //else
                //{
                //    _logger.LogError("{ExceptionType}{ExceptionMessage}",
                //                                           ex.GetType().ToString(),
                //                                           ex.Message);
                //}
                //httpContext.Response.StatusCode = 500;
                //await httpContext.Response.WriteAsync("Error occurred");
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            // Customize the response here
            return context.Response.WriteAsync(new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal server error",
                Detailed = exception.Message // Optionally include exception details
            }.ToString());
        }
    }
}
