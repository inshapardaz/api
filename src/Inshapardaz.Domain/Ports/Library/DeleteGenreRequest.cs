using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class DeleteGenreRequest : RequestBase
    {
        public DeleteGenreRequest(int genreId)
        {
            GenreIdId = genreId;
        }

        public int GenreIdId { get; }
    }

    public class DeleteGenreRequestHandler : RequestHandlerAsync<DeleteGenreRequest>
    {
        private readonly IGenreRepository _genreRepository;

        public DeleteGenreRequestHandler(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public override async Task<DeleteGenreRequest> HandleAsync(DeleteGenreRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _genreRepository.GetGenreById(command.GenreIdId, cancellationToken);

            if (result == null)
            {
                throw new NotFoundException();
            }

            await _genreRepository.DeleteGenre(command.GenreIdId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}