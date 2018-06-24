using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetGenreByIdRequest : RequestBase
    {
        public GetGenreByIdRequest(int authorId)
        {
            GenreId = authorId;
        }

        public Genre Result { get; set; }
        public int GenreId { get; }
    }

    public class GetGenreByIdRequestHandler : RequestHandlerAsync<GetGenreByIdRequest>
    {
        private readonly IGenreRepository _genreRepository;

        public GetGenreByIdRequestHandler(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public override async Task<GetGenreByIdRequest> HandleAsync(GetGenreByIdRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var genre = await _genreRepository.GetGenreById(command.GenreId, cancellationToken);
            command.Result = genre;
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}

