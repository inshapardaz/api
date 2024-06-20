using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class AuthorAssert
    {
        private AuthorView _author;
        private int _libraryId;

        public HttpResponseMessage _response;
        private readonly IAuthorTestRepository _authorRepository;
        private readonly IBookTestRepository _bookRepository;
        private readonly IArticleTestRepository _articleRepository;
        private readonly FakeFileStorage _fileStorage;
        private readonly IFileTestRepository _fileRepository;

        public AuthorAssert(IAuthorTestRepository authorRepository, 
            IBookTestRepository bookRepository, 
            IArticleTestRepository articleRepository, 
            FakeFileStorage fileStorage, 
            IFileTestRepository fileRepository)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
            _articleRepository = articleRepository;
            _fileStorage = fileStorage;
            _fileRepository = fileRepository;
        }

        public AuthorAssert ForResponse(HttpResponseMessage response)
        {
            _response = response;
            _author = response.GetContent<AuthorView>().Result;
            return this;
        }

        public AuthorAssert ForView(AuthorView view)
        {
            _author = view;
            return this;
        }

        public AuthorAssert ForLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public AuthorAssert ShouldHaveSelfLink()
        {
            _author.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/authors/{_author.Id}");

            return this;
        }

        public AuthorAssert ShouldHaveBooksLink()
        {
            _author.Link("books")
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/books")
                  .ShouldHaveQueryParameter("authorId", _author.Id);

            return this;
        }

        public AuthorAssert ShouldHaveUpdateLink()
        {
            _author.UpdateLink()
                  .ShouldBePut()
                  .EndingWith($"libraries/{_libraryId}/authors/{_author.Id}");

            return this;
        }

        public AuthorAssert ShouldNotHaveUpdateLink()
        {
            _author.UpdateLink().Should().BeNull();
            return this;
        }

        public AuthorAssert ShouldHaveCorrectImageLocationHeader(int authorId)
        {
            _response.Headers.Location.Should().NotBeNull();
            return this;
        }

        public AuthorAssert ShouldNotHaveImageUpdateLink()
        {
            _author.Link("image-upload").Should().BeNull();
            return this;
        }

        public AuthorAssert ShouldHaveImageUpdateLink()
        {
            _author.Link("image-upload")
                   .ShouldBePut()
                   .EndingWith($"libraries/{_libraryId}/authors/{_author.Id}/image");
            return this;
        }

        public AuthorAssert ShouldHavePublicImageLink()
        {
            _author.Link("image")
                .ShouldBeGet()
                .Href.Should()
                .NotBeNullOrEmpty();
            //.StartWith(Settings.CDNAddress);

            return this;
        }

        public AuthorAssert ShouldNotHaveImageLink()
        {
            _author.Link("image").Should().BeNull();
            return this;
        }

        public AuthorAssert ShouldHaveDeleteLink()
        {
            _author.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"libraries/{_libraryId}/authors/{_author.Id}");
            return this;
        }

        public AuthorAssert WithBookCount(int count)
        {
            _author.BookCount.Should().Be(count);
            return this;
        }

        public AuthorAssert ShouldBeSameAs(AuthorDto expected)
        {
            _author.Should().NotBeNull();
            _author.Name.Should().Be(expected.Name);
            return this;
        }

        public AuthorAssert WithReadOnlyLinks()
        {
            ShouldHaveSelfLink();
            ShouldHaveBooksLink();
            ShouldNotHaveUpdateLink();
            ShouldNotHaveDeleteLink();
            ShouldNotHaveImageUpdateLink();

            return this;
        }

        public AuthorAssert WithEditableLinks(Role role)
        {
            ShouldHaveSelfLink();
            ShouldHaveBooksLink();
            ShouldHaveUpdateLink();
            if (role == Role.Admin || role == Role.LibraryAdmin)
            {
                ShouldHaveDeleteLink();
            }
            else
            {
                ShouldNotHaveDeleteLink();
            }
            ShouldHaveImageUpdateLink();

            return this;
        }



        public AuthorAssert ShouldHaveUpdatedAuthorImage(int authorId, byte[] newImage)
        {
            var imageUrl = _authorRepository.GetAuthorImageUrl(authorId);
            imageUrl.Should().NotBeNull();
            var image = _fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNull().And.Equal(newImage);
            return this;
        }

        public AuthorAssert ShouldHavePublicImage(int authorId)
        {
            var image = _authorRepository.GetAuthorImage(authorId);
            image.Should().NotBeNull();
            image.IsPublic.Should().BeTrue();
            return this;
        }

        public AuthorAssert ShouldNotHaveUpdatedAuthorImage(int authorId, byte[] newImage)
        {
            var imageUrl = _authorRepository.GetAuthorImageUrl(authorId);
            imageUrl.Should().NotBeNull();
            var image = _fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotEqual(newImage);
            return this;
        }

        public AuthorAssert ShouldHaveAddedAuthorImage(int authorId, byte[] newImage)
        {
            var author = _authorRepository.GetAuthorById(authorId);
            author.ImageId.Should().NotBeNull();
            var file = _fileRepository.GetFileById(author.ImageId.Value);
            file.FilePath.Should().Be($"authors/{author.Id}/image{Path.GetExtension(file.FileName)}");
            var image = _fileStorage.GetFile(file.FilePath, CancellationToken.None).Result;
            image.Should().NotBeNullOrEmpty().And.Equal(newImage);
            return this;
        }

        public AuthorAssert ShouldHaveDeletedAuthorImage(int authorId, long imageId, string filePath)
        {
            var image = _authorRepository.GetAuthorImage(authorId);
            image.Should().BeNull();
            var file = _fileRepository.GetFileById(imageId);
            file.Should().BeNull();
            _fileStorage.DoesFileExists(filePath).Should().BeFalse();
            return this;
        }

        public AuthorAssert ShouldHaveDeletedAuthor(int authorId)
        {
            var author = _authorRepository.GetAuthorById(authorId);
            author.Should().BeNull();
            return this;
        }

        public AuthorAssert ShouldNotHaveDeletedAuthor(int authorId)
        {
            var author = _authorRepository.GetAuthorById(authorId);
            author.Should().NotBeNull();
            return this;
        }

        public AuthorAssert ShouldNotHaveDeleteLink()
        {
            _author.DeleteLink().Should().BeNull();

            return this;
        }

        public AuthorAssert ShouldHaveImageUploadLink()
        {
            _author.Link("image-upload")
                  .ShouldBePut()
                  .EndingWith($"libraries/{_libraryId}/authors/{_author.Id}/image");

            return this;
        }

        public AuthorAssert ShouldNotHaveImageUploadLink()
        {
            _author.Link("image-upload").Should().BeNull();

            return this;
        }

        public AuthorAssert ShouldHaveCorrectLocationHeader()
        {
            var location = _response.Headers.Location;
            location.Should().NotBeNull();
            location.AbsoluteUri.Should().EndWith($"libraries/{_libraryId}/authors/{_author.Id}");
            return this;
        }

        public AuthorAssert ShouldHaveSavedAuthor()
        {
            var dbAuthor = _authorRepository.GetAuthorById(_author.Id);
            dbAuthor.Should().NotBeNull();
            _author.Name.Should().Be(dbAuthor.Name);
            return this;
        }

        public AuthorAssert ShouldHaveCorrectAuthorRetunred(AuthorDto author)
        {
            _author.Should().NotBeNull();
            _author.Id.Should().Be(author.Id);
            _author.Name.Should().Be(author.Name);
            _author.BookCount.Should().Be(_bookRepository.GetBookCountByAuthor(_author.Id));
            _author.ArticleCount.Should().Be(_articleRepository.GetArticleCountByAuthor(_author.Id));
            return this;
        }
    }
}
