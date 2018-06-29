using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class UpdateGenreRequest : RequestBase
    {
        public UpdateGenreRequest(Genre genre)
        {
            Genre = genre;
        }

        public Genre Genre { get; }

        public UpdateGenreResult Result { get; } = new UpdateGenreResult();

        public class UpdateGenreResult
        {
            public bool HasAddedNew { get; set; }

            public Genre Genre { get; set; }
        }
    }

    public class UpdateGenreRequestHandler : RequestHandlerAsync<UpdateGenreRequest>
    {
        private readonly IGenreRepository _genreRepository;

        public UpdateGenreRequestHandler(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public override async Task<UpdateGenreRequest> HandleAsync(UpdateGenreRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _genreRepository.GetGenreById(command.Genre.Id, cancellationToken);

            if (result == null)
            {
                command.Genre.Id = default(int);
                var newGenre = await _genreRepository.AddGenre(command.Genre, cancellationToken);
                command.Result.HasAddedNew = true;
                command.Result.Genre = newGenre;
            }
            else
            {
                await _genreRepository.UpdateGenre(command.Genre, cancellationToken);
                command.Result.Genre = command.Genre;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}