using AutoFixture;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Helpers
{
    public static class HttpExtensions
    {
        public static async Task<T> GetContent<T>(this HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(body);
        }

        public static async Task<HttpResponseMessage> GetAsync(this HttpClient client, string url, string language, string mimetype)
        {
            if (!string.IsNullOrWhiteSpace(language))
            {
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(language));
            }

            if (!string.IsNullOrWhiteSpace(mimetype))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mimetype));
            }

            return await client.GetAsync(url);
        }

        public static async Task<HttpResponseMessage> PostObject<T>(this HttpClient client, string url, T payload)
        {
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.Default, "application/json");
            return await client.PostAsync(url, content);
        }

        public static async Task<HttpResponseMessage> PostContent(this HttpClient client, string url, byte[] payload, string language, string mimetype)
        {
            if (!string.IsNullOrEmpty(language))
            {
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(language));
            }

            using (var stream = new MemoryStream(payload))
            using (var content = new StreamContent(stream))
            using (var formData = new MultipartFormDataContent())
            {
                content.Headers.ContentType = new MediaTypeHeaderValue(mimetype);
                formData.Add(content, "file", $"{Random.Name}.jpg");

                return await client.PostAsync(url, formData);
            }
        }

        public static async Task<HttpResponseMessage> PutObject<T>(this HttpClient client, string url, T payload)
        {
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            return await client.PutAsync(url, content);
        }

        public static async Task<HttpResponseMessage> PutFile(this HttpClient client, string url, byte[] payload, string fileName = "image.jpg")
        {
            using (var stream = new MemoryStream(payload))
            using (var content = new StreamContent(stream))
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(content, "file", $"{Random.Name}.jpg");

                return await client.PutAsync(url, formData);
            }
        }

        public static async Task<HttpResponseMessage> PostObjectWithFile<T>(this HttpClient client, string url, T data, byte[] payload, string fileName = "image.jpg")
        {
            using (var stream = new MemoryStream(payload))
            using (var content = new StreamContent(stream))
            using (var formData = new MultipartFormDataContent())
            {
                var jsonContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                formData.Add(jsonContent, "json");
                formData.Add(content, "image", $"{Random.Name}.jpg");

                return await client.PostAsync(url, formData);
            }
        }

        public static async Task<HttpResponseMessage> PutFile(this HttpClient client, string url, byte[] payload, string language, string mimeType, string fileName = "image.jpg")
        {
            client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(language));

            using (var stream = new MemoryStream(payload))
            using (var content = new StreamContent(stream))
            using (var formData = new MultipartFormDataContent())
            {
                content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
                formData.Add(content, "file", $"{Random.Name}.jpg");

                return await client.PutAsync(url, formData);
            }
        }

        public static async Task<HttpResponseMessage> PutContent(this HttpClient client, string url, byte[] payload, string language, string mimetype)
        {
            if (!string.IsNullOrEmpty(language))
            {
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(language));
            }

            using (var stream = new MemoryStream(payload))
            using (var content = new StreamContent(stream))
            using (var formData = new MultipartFormDataContent())
            {
                content.Headers.ContentType = new MediaTypeHeaderValue(mimetype);
                formData.Add(content, "file", $"{Random.Name}.jpg");

                return await client.PutAsync(url, formData);
            }
        }

        public static async Task<HttpResponseMessage> DeleteAsync(this HttpClient client, string url, string language, string mimetype)
        {
            if (!string.IsNullOrWhiteSpace(language))
            {
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(language));
            }

            if (!string.IsNullOrWhiteSpace(mimetype))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mimetype));
            }

            return await client.DeleteAsync(url);
        }
    }
}
