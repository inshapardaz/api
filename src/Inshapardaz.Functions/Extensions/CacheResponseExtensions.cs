using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Inshapardaz.Functions.Extensions
{
    public static class CacheResponseExtensions
    {
        private static TimeSpan defaultTimeSpan = TimeSpan.FromDays(1);

        public static HttpResponseMessage CreateCachedResponse<T>(this HttpRequestMessage request, HttpStatusCode statusCode, T value, TimeSpan? maxAge = null)
        {
            HttpResponseMessage responseMessage = request.CreateResponse<T>(statusCode, value);
            responseMessage.Headers.CacheControl = new CacheControlHeaderValue()
            {
                Public = true,
                MaxAge = maxAge ?? defaultTimeSpan
            };
            return responseMessage;
        }
    }
}
