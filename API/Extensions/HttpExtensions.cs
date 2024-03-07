
using System.Text.Json;
using API.Helpers;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, PaginationHeader header)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            response.Headers.Append("Pagination", JsonSerializer.Serialize(header, jsonOptions));
            // because this custom header 
            response.Headers.Append(Microsoft.Net.Http.Headers.HeaderNames.AccessControlExposeHeaders, "Pagination");

            // response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}