using DatingApp.Services.Pagination;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace DatingApp.Services.Extensions
{
    public static class HttpExtensions
    {
        /// <summary>
        /// We are extending the HttpResponse class to include the PaginationHeader information
        /// </summary>
        /// <param name="response"></param>
        /// <param name="header"></param>
        public static void AddPaginationHeader(this HttpResponse response, PaginationHeader header)
        {
            // We need to convert this to json so it can go back with the header
            // Note: if we don't specify camel case, the json will be returned as pascal case instead
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            response.Headers.Add("Pagination", JsonSerializer.Serialize(header,jsonOptions));

            // Adding cors policy so the client can get the pagination header options
            // we need to expose the pagination header
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");

        }
    }
}
