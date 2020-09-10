using Newtonsoft.Json;
using System.Net.Http;
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
    }
}
