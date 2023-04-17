using DatingApp.Services.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingApp.Services.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next">This will be the next middleware in the http pipeline</param>
        /// <param name="logger">Used to log information regarding exceptions</param>
        /// <param name="env">Used to help us determine if we are running in production mode or development mode</param>
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        /// <summary>
        /// This method must be called InvokeAsync. The framework is expecting the name InvokeAsync. It uses this name to determine what will happen next
        /// </summary>
        /// <param name="context">This gives us access to the http request being passed into the middleware</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // The try catch block will be used to handle the exception
            // This is the only place it needs to be to allow us to handle our middleware

            try
            {
                // We await the next http request in the pipeline
                await _next(context);
            }
            catch(Exception ex)
            {
                // Logging the error
                _logger.LogError(ex, ex.Message);
                // Specifying the content we are returning to the client
                context.Response.ContentType = "application/json";
                // Give the response a 500 error code
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;


                // If we are in development mode we return the first part. Else we return the second part.
                // Note: This is a ternary operator
                var response = _env.IsDevelopment()
                    ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new ApiException(context.Response.StatusCode, ex.Message, "Internal Server Error");

                var options = new JsonSerializerOptions
                {
                    // Our API Controller use this by default but since we are not in the controller, we specify this here
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);

            }
        }


    }
}
