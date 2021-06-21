using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers
{
    public class AutoCorrectTextCommand : RequestBase
    {
        public string Language { get; set; }

        public string TextToCorrect { get; set; }

        public string Result { get; set; }
    }

    public class AutoCorrectTextCommandHandeler : RequestHandlerAsync<AutoCorrectTextCommand>
    {
        private readonly ICorrectionRepository _correctionRepository;

        public AutoCorrectTextCommandHandeler(ICorrectionRepository correctionRepository)
        {
            _correctionRepository = correctionRepository;
        }

        public override async Task<AutoCorrectTextCommand> HandleAsync(AutoCorrectTextCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            var corrections = await _correctionRepository.GetCorrectionForLanguage(command.Language, cancellationToken);

            StringBuilder sb = new StringBuilder(command.TextToCorrect, command.TextToCorrect.Length * 2);
            foreach (var correction in corrections)
            {
                sb.Replace(correction.Phrase, correction.Replacement);
            }

            command.Result = sb.ToString();

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
