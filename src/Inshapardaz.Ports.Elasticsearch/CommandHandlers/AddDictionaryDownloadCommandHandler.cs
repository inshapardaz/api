using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Ports.Elasticsearch.CommandHandlers
{
    public class AddDictionaryDownloadCommandHandler : RequestHandlerAsync<AddDictionaryDownloadCommand>
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public AddDictionaryDownloadCommandHandler(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public override async Task<AddDictionaryDownloadCommand> HandleAsync(AddDictionaryDownloadCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            var jobId = _backgroundJobClient.Enqueue<SqliteExport>(x => x.ExportDictionary(command.DitionarayId));
            command.JobId = jobId;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
