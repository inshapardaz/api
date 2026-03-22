namespace Inshapardaz.Domain.Adapters;

public interface IProvideOcr
{
    Task<string> PerformOcr(byte[] imageData, string apiKey, CancellationToken cancellationToken);
}
