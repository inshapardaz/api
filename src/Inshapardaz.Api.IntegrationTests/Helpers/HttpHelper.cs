using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Newtonsoft.Json;

namespace Inshapardaz.Api.IntegrationTests.Helpers
{
    public static class HttpHelper
    {
        public static async Task<HttpResponseMessage> PostJson<T>(this HttpClient client, string url, T objectToSend)
        {
            var json = JsonConvert.SerializeObject(objectToSend);
            var httpContent = new StringContent(json);
            httpContent.Headers.ContentType.MediaType = MimeTypes.Json;

            return await client.PostAsync(url, httpContent);
        }

        public static async Task<HttpResponseMessage> PutJson<T>(this HttpClient client, string url, T objectToSend)
        {
            var json = JsonConvert.SerializeObject(objectToSend);
            var httpContent = new StringContent(json);
            httpContent.Headers.ContentType.MediaType = MimeTypes.Json;

            return await client.PutAsync(url, httpContent);
        }
    }
}
