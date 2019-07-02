using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.AspNetCore.Http;

namespace Inshapardaz.Functions.Tests
{
    public class TestHelpers
    {
        public static HttpRequest CreateGetRequest(string hostName = null, Dictionary<string, string> queryString = null)
        {
            return null;
            /*var requestPath = string.IsNullOrWhiteSpace(hostName) ? "https://localhost" : hostName;
            requestPath += queryString == null ? string.Empty : $"?{string.Join("&", queryString.Select(kvp => $"{kvp.Key}={kvp.Value}"))}";
            var request = new HttpRequest
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(requestPath)
            };

            //request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            return request;*/
        }

        public static HttpRequestMessage CreatePostRequest<T>(T obj, string hostName = null)
        {
            var requestPath = string.IsNullOrWhiteSpace(hostName) ? "https://localhost" : hostName;
            var json = JsonConvert.SerializeObject(obj);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(requestPath),
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            

            //request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            return request;
        }
    }
}