using Google.Cloud.Vision.V1;
using Inshapardaz.Domain.Adapters;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Adapter.Ocr.Google
{
    public class GoogleOcr : IProvideOcr
    {
        public async Task<string> PerformOcr(byte[] imageData, string apiKey, CancellationToken cancellationToken)
        {
            var image = Image.FromBytes(imageData);
            var client = CreateClient(apiKey);
            var response = await client.DetectDocumentTextAsync(image);
            if (response == null || response.Text == null)
            {
                return null;
            }

            return response.Text;
        }

        private ImageAnnotatorClient CreateClient(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return ImageAnnotatorClient.Create();
            }

            var builder = new ImageAnnotatorClientBuilder();
            builder.JsonCredentials = apiKey;
            return builder.Build();
        }
    }
}
