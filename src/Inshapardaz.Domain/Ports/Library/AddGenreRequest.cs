using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class AddGenreRequest : RequestBase
    {
        public AddGenreRequest(Genre genre)
        {
            Genre = genre;
        }

        public Genre Genre { get; }
        public Genre Result { get; set; }
    }

    public class AddGenreRequestHandler : RequestHandlerAsync<AddGenreRequest>
    {
        private readonly IGenreRepository _genreRepository;

        public AddGenreRequestHandler(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public override async Task<AddGenreRequest> HandleAsync(AddGenreRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var newGenre = await _genreRepository.AddGenre(command.Genre, cancellationToken);

            command.Result = newGenre;
            return await base.HandleAsync(command, cancellationToken);
        }
    } 
}
