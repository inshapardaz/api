using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Adapters;

public interface ISendEmail
{
    void Send(string to, string subject, string html, string from = null);

    Task SendAsync(string to, string subject, string html, string from = null, CancellationToken cancellationToken = default(CancellationToken));
}
