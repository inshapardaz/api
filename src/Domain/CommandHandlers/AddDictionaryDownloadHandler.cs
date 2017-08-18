using Hangfire;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Jobs;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddDictionaryDownloadHandler : RequestHandler<AddDictionaryDownloadCommand>
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public AddDictionaryDownloadHandler(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }
        public override AddDictionaryDownloadCommand Handle(AddDictionaryDownloadCommand command)
        {

            var jobId = _backgroundJobClient.Enqueue<SqliteExport>(x => x.ExportDictionary(command.DitionarayId));
            command.JobId = jobId;
            return base.Handle(command);
        }
    }
}
