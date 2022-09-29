using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        // This will tell us what environment we are in
        private readonly IHostEnvironment _env;

        // RequestDelegate handles is coming up next in the middleware pipeline
        // ILogger is used to write more detailed log data
        // IHostEnvironment is what will determine the type of environment (could be development or production)
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            this._env = env;
            this._logger = logger;
            this._next = next;
        }
        // At middleware we have access to the HTTP request
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Get our context and pass it onto the next piece of middleware
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                // This will be a 500 response
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

                // If we are in development mode we do the following, else we are in production and do the latter
                var response = _env.IsDevelopment()
                    ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new ApiException(context.Response.StatusCode, "Internal Server Error");
                // We need to serialize the response into a JSON response
                // This will ensure out response is a normal JSON response in camel case
                var options = new JsonSerializerOptions{ PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}