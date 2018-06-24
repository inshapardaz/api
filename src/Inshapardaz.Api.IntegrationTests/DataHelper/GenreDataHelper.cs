using System.Threading;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;

namespace Inshapardaz.Api.IntegrationTests.DataHelper
{
    public class GenreDataHelper
    {
        private readonly IGenreRepository _genreRepository;

        public GenreDataHelper(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public Genre Create(string genre)
        {
            return _genreRepository.AddGenre(new Genre { Name = genre} , CancellationToken.None).Result;
        }

        public Genre Get(int genreId)
        {
            return _genreRepository.GetGenreById(genreId, CancellationToken.None).Result;
        }

        public void Delete(int genreId)
        {
            _genreRepository.DeleteGenre(genreId, CancellationToken.None).Wait();
        }
    }
}