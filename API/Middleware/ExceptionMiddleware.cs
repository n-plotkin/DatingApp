using System.Net;
using System.Text.Json;
using API.Errors;
using Microsoft.AspNetCore.Mvc;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;
        public ExceptionMiddleware(RequestDelegate next, 
            ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _next = next;
            
        }

        //Must be called InvokeAsync, framework expects to see this from our middleware
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                //try to pass it along harmlessly through tot the next middleware
                await _next(context);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, ex.Message);

                //configure our http context to the format we want, give an error code
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                //create a response, grab exception error message, include stacktrace if in dev mode
                var response = _env.IsDevelopment()
                    ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new ApiException(context.Response.StatusCode, ex.Message, "Internal Server Error.");

                //configure our JsonSerializerOptions such that we're following CamelCase
                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
                //make a serializer with our response and options
                var json = JsonSerializer.Serialize(response, options);
                
                await context.Response.WriteAsync(json);
            }   


        }
    }
}