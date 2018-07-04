using System.Threading;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;

namespace Inshapardaz.Api.IntegrationTests.DataHelper
{
    public class AuthorDataHelper
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorDataHelper(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public Author Create(Author author)
        {
            return _authorRepository.AddAuthor(author , CancellationToken.None).Result;
        }

        public Author Get(int authorId)
        {
            return _authorRepository.GetAuthorById(authorId, CancellationToken.None).Result;
        }

        public void Delete(int authorId)
        {
            _authorRepository.DeleteAuthor(authorId, CancellationToken.None).Wait();
        }
    }
}