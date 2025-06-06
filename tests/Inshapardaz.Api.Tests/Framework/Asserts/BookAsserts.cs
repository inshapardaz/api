﻿using FluentAssertions;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class BookAssert
    {
        private BookView _book;
        private int _libraryId;
        public HttpResponseMessage _response;
        
        private readonly IBookTestRepository _bookRepository;
        private readonly IAuthorTestRepository _authorRepository;
        private readonly ICategoryTestRepository _categoryRepository;
        private readonly ISeriesTestRepository _seriesRepository;
        private readonly FakeFileStorage _fileStorage;

        public BookAssert(IBookTestRepository bookRepository,
            IAuthorTestRepository authorRepository,
            ICategoryTestRepository categoryRepository,
            FakeFileStorage fileStorage,
            ISeriesTestRepository seriesRepository)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _categoryRepository = categoryRepository;
            _fileStorage = fileStorage;
            _seriesRepository = seriesRepository;
        }

        public BookAssert ForResponse(HttpResponseMessage response)
        {
            _response = response;
            _book = response.GetContent<BookView>().Result;
            return this;
        }

        public BookAssert ForView(BookView view)
        {
            _book = view;
            return this;
        }


        public BookAssert ForLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public BookAssert ShouldHaveSelfLink()
        {
            _book.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/books/{_book.Id}");

            return this;
        }

        public BookAssert ShouldHaveChaptersLink()
        {
            _book.Link("chapters")
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/books/{_book.Id}/chapters");

            return this;
        }

        public BookAssert ShouldHaveAddFavoriteLink()
        {
            _book.Link(RelTypes.CreateFavorite)
                  .ShouldBePost()
                  .EndingWith($"libraries/{_libraryId}/favorites/books/{_book.Id}");

            return this;
        }

        public BookAssert ShouldNotHaveAddFavoriteLink()
        {
            _book.Link(RelTypes.CreateFavorite).Should().BeNull();
            return this;
        }

        public BookAssert ShouldHaveRemoveFavoriteLink()
        {
            _book.Link(RelTypes.RemoveFavorite)
                  .ShouldBeDelete()
                  .EndingWith($"libraries/{_libraryId}/favorites/books/{_book.Id}");

            return this;
        }
        
        public BookAssert ShouldHaveRemoveFromBookShelfImageLink(int bookShelfId)
        {
            _book.Link(RelTypes.RemoveBookFromBookShelf)
                  .ShouldBeDelete()
                  .EndingWith($"libraries/{_libraryId}/bookshelves/{bookShelfId}/books/{_book.Id}");

            return this;
        }
        
        public BookAssert ShouldNotHaveRemoveFromBookShelfImageLink()
        {
            _book.Link(RelTypes.RemoveBookFromBookShelf).Should().BeNull();

            return this;
        }

        public BookAssert ShouldNotHaveRemoveFavoriteLink()
        {
            _book.Link(RelTypes.RemoveFavorite).Should().BeNull();
            return this;
        }

        public BookAssert ShouldHaveContents(bool haveEditableLinks = false)
        {
            var bookContents = _bookRepository.GetBookContents(_book.Id);
            _book.Contents.Should().NotBeEmpty();
            _book.Contents.Should().HaveSameCount(bookContents);

            foreach (var content in bookContents)
            {
                var bookContent = _book.Contents.SingleOrDefault(c => c.Language == content.Language && c.MimeType == content.MimeType);
                bookContent.Should().NotBeNull();
                bookContent.Link("self")
                    .ShouldBeGet()
                    .ShouldHaveAcceptLanguage(content.Language)
                    .ShouldHaveAccept(content.MimeType)
                    .EndingWith($"libraries/{_libraryId}/books/{_book.Id}/contents/{bookContent.Id}");

                bookContent.Link("book")
                    .ShouldBeGet()
                    .EndingWith($"libraries/{_libraryId}/books/{_book.Id}");

                if (haveEditableLinks)
                {
                    bookContent.Link("update")
                                        .ShouldBePut()
                                        .ShouldHaveAcceptLanguage(content.Language)
                                        .ShouldHaveAccept(content.MimeType)
                                        .EndingWith($"libraries/{_libraryId}/books/{_book.Id}/contents/{bookContent.Id}");
                    bookContent.Link("delete")
                                        .ShouldBeDelete()
                                        .ShouldHaveAcceptLanguage(content.Language)
                                        .ShouldHaveAccept(content.MimeType)
                                        .EndingWith($"libraries/{_libraryId}/books/{_book.Id}/contents/{bookContent.Id}");
                }
                else
                {
                    bookContent.Link("update").Should().BeNull();
                    bookContent.Link("delete").Should().BeNull();
                }
            }

            return this;
        }

        public BookAssert ShouldHaveEditLinks()
        {
            return ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink();
        }

        public BookAssert ShouldNotHaveEditLinks()
        {
            return ShouldNotHaveUpdateLink()
                   .ShouldNotHaveDeleteLink();
        }

        public BookAssert ShouldHaveCorrectLinks()
        {
            ShouldHaveSelfLink();
            return this;
        }

        public BookAssert ShouldHaveCorrectImageLocationHeader(int bookId)
        {
            _response.Headers.Location.AbsoluteUri.Should().NotBeEmpty();
            return this;
        }

        public BookAssert ShouldHavePublicImage(int bookId)
        {
            var image = _bookRepository.GetBookImage(bookId);
            image.Should().NotBeNull();
            image.IsPublic.Should().BeTrue();
            return this;
        }

        public BookAssert ShouldHaveSeriesLink()
        {
            _book.Link("series")
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/series/{_book.SeriesId}");

            return this;
        }

        public BookAssert ShouldHavePagesLink()
        {
            _book.Link("pages")
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/books/{_book.Id}/pages");

            return this;
        }

        public BookAssert ShouldHavePagesUploadLink()
        {
            _book.Link("add-pages")
                  .ShouldBePost()
                  .EndingWith($"libraries/{_libraryId}/books/{_book.Id}/pages");

            return this;
        }

        public BookAssert ShouldNotHaveSeriesLink()
        {
            _book.Link("series").Should().BeNull();
            return this;
        }

        public BookAssert ShouldHaveUpdateLink()
        {
            _book.UpdateLink()
                  .ShouldBePut()
                  .EndingWith($"libraries/{_libraryId}/books/{_book.Id}");

            return this;
        }

        public BookAssert ShouldNotHaveUpdateLink()
        {
            _book.UpdateLink().Should().BeNull();
            return this;
        }

        public BookAssert ShouldNotHaveDeleteLink()
        {
            _book.DeleteLink().Should().BeNull();
            return this;
        }

        public BookAssert ShouldNotHaveImageUpdateLink()
        {
            _book.Link("image-upload").Should().BeNull();
            return this;
        }

        public BookAssert ShouldHaveImageUpdateLink()
        {
            _book.Link("image-upload")
                   .ShouldBePut()
                   .EndingWith($"libraries/{_libraryId}/books/{_book.Id}/image");
            return this;
        }

        public BookAssert ShouldHaveDeleteLink()
        {
            _book.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"libraries/{_libraryId}/books/{_book.Id}");
            return this;
        }

        public BookAssert ShouldHaveCreateChaptersLink()
        {
            _book.Link("create-chapter")
                  .ShouldBePost()
                  .EndingWith($"libraries/{_libraryId}/books/{_book.Id}/chapters");
            return this;
        }

        public BookAssert ShouldNotHaveCreateChaptersLink()
        {
            _book.Link("create-chapter").Should().BeNull();
            return this;
        }

        public BookAssert ShouldHaveAddContentLink()
        {
            _book.Link("add-file")
                  .ShouldBePost()
                  .EndingWith($"libraries/{_libraryId}/books/{_book.Id}/contents");
            return this;
        }

        public BookAssert ShouldNotHaveAddContentLink()
        {
            _book.Link("add-file").Should().BeNull();
            return this;
        }

        public BookAssert ShouldHavePublicImageLink()
        {
            _book.Link("image")
                .ShouldBeGet();
            //.Href.Should().StartWith(Settings.CDNAddress);

            return this;
        }

        public BookAssert ShouldHaveCorrectLocationHeader()
        {
            _response.Headers.Location.Should().NotBeNull();
            _response.Headers.Location.AbsoluteUri.Should().EndWith($"libraries/{_libraryId}/books/{_book.Id}");
            return this;
        }

        public BookAssert ShouldHaveSavedBook()
        {
            var dbbook = _bookRepository.GetBookById(_book.Id);
            return ShouldBeSameAs(dbbook);
        }

        public BookAssert ShouldBeAddedToFavorite(int bookId, int accountId)
        {
            _bookRepository.DoesBookExistsInFavorites(bookId, accountId).Should().BeTrue();
            return this;
        }

        public BookAssert ShouldNotBeInFavorites(int bookId, int accountId)
        {
            _bookRepository.DoesBookExistsInFavorites(bookId, accountId).Should().BeFalse();
            return this;
        }

        public BookAssert ShouldHaveDeletedBookFromRecentReads(int bookId)
        {
            _bookRepository.DoesBookExistsInRecent(bookId).Should().BeFalse();
            return this;
        }

        public BookAssert ShouldBeSameAs(BookDto expected)
        {
            _book.Should().NotBeNull();
            _book.Title.Should().Be(expected.Title);
            _book.Description.Should().Be(expected.Description);
            _book.Language.Should().Be(expected.Language);

            _book.IsPublic.Should().Be(expected.IsPublic);
            _book.IsPublished.Should().Be(expected.IsPublished);
            _book.Copyrights.Should().Be(expected.Copyrights.ToDescription());
            _book.DateAdded.Should().BeCloseTo(expected.DateAdded, TimeSpan.FromSeconds(2));
            _book.DateUpdated.Should().BeCloseTo(expected.DateUpdated, TimeSpan.FromSeconds(2));
            _book.Status.Should().Be(expected.Status.ToDescription());
            _book.YearPublished.Should().Be(expected.YearPublished);
            _book.SeriesId.Should().Be(expected.SeriesId);
            _book.Source.Should().Be(expected.Source);
            _book.Publisher.Should().Be(expected.Publisher);
            if (_book.SeriesId.HasValue)
            {
                _book.SeriesName.Should().Be(_seriesRepository.GetSeriesById(expected.SeriesId.Value).Name);
                _book.SeriesIndex.Should().Be(expected.SeriesIndex);
            }

            var authors = _authorRepository.GetAuthorsByBook(expected.Id);
            _book.Authors.Should().HaveSameCount(authors);
            foreach (var author in authors)
            {
                var actual = _book.Authors.SingleOrDefault(a => a.Id == author.Id);
                actual.Name.Should().Be(author.Name);

                actual.Link("self")
                      .ShouldBeGet()
                      .EndingWith($"libraries/{_libraryId}/authors/{author.Id}");

                return this;
            }

            var catergories = _categoryRepository.GetCategoriesByBook(expected.Id);
            _book.Categories.Should().HaveSameCount(catergories);
            foreach (var catergory in catergories)
            {
                var actual = _book.Categories.SingleOrDefault(a => a.Id == catergory.Id);
                actual.Name.Should().Be(catergory.Name);

                actual.Link("self")
                      .ShouldBeGet()
                      .EndingWith($"libraries/{_libraryId}/categories/{catergory.Id}");

                return this;
            }
            
            // var tags = _categoryRepository.GetCategoriesByBook(expected.Id);
            // _book.Categories.Should().HaveSameCount(catergories);
            // foreach (var catergory in catergories)
            // {
            //     var actual = _book.Categories.SingleOrDefault(a => a.Id == catergory.Id);
            //     actual.Name.Should().Be(catergory.Name);
            //
            //     actual.Link("self")
            //           .ShouldBeGet()
            //           .EndingWith($"libraries/{_libraryId}/categories/{catergory.Id}");
            //
            //     return this;
            // }

            return this;
        }

        public BookAssert ShouldBeSameAs(BookView expected)
        {
            _book.Should().NotBeNull();
            _book.Title.Should().Be(expected.Title);
            _book.Description.Should().Be(expected.Description);
            _book.Language.Should().Be(expected.Language);

            _book.IsPublic.Should().Be(expected.IsPublic);
            _book.IsPublished.Should().Be(expected.IsPublished);
            _book.Copyrights.Should().Be(expected.Copyrights);
            _book.DateAdded.Should().BeCloseTo(expected.DateAdded, TimeSpan.FromSeconds(2));
            _book.DateUpdated.Should().BeCloseTo(expected.DateUpdated, TimeSpan.FromSeconds(2));
            _book.Status.Should().Be(expected.Status);
            _book.YearPublished.Should().Be(expected.YearPublished);
            _book.SeriesId.Should().Be(expected.SeriesId);
            _book.SeriesName.Should().Be(_seriesRepository.GetSeriesById(expected.SeriesId.Value).Name);
            _book.SeriesIndex.Should().Be(expected.SeriesIndex);
            _book.Source.Should().Be(expected.Source);
            _book.Publisher.Should().Be(expected.Publisher);

            var authors = _authorRepository.GetAuthorsByBook(expected.Id);
            _book.Authors.Should().HaveSameCount(authors);
            foreach (var author in authors)
            {
                var actual = _book.Authors.SingleOrDefault(a => a.Id == author.Id);
                actual.Name.Should().Be(author.Name);
                actual.Link("self")
                      .ShouldBeGet()
                      .EndingWith($"libraries/{_libraryId}/authors/{author.Id}");
            }

            _book.Categories.Should().HaveSameCount(expected.Categories);

            return this;
        }

        public BookAssert ShouldBeSameCategories(IEnumerable<CategoryDto> expectedCategories)
        {
            expectedCategories.Should().HaveSameCount(_book.Categories);
            foreach (var expectedCategory in expectedCategories)
            {
                var category = _book.Categories.SingleOrDefault(c => c.Id == expectedCategory.Id);
                category.Should().NotBeNull();
                category.Id.Should().Be(expectedCategory.Id);
                category.Name.Should().Be(expectedCategory.Name);
            }
            return this;
        }
        
        public BookAssert ShouldHaveMatchingTags(IEnumerable<TagView> expectedTags)
        {
            expectedTags.Should().HaveSameCount(_book.Tags);
            foreach (var expectedTag in expectedTags)
            {
                var tag = _book.Tags.SingleOrDefault(c => c.Name == expectedTag.Name);
                tag.Should().NotBeNull();
            }
            return this;
        }

        public BookAssert ShouldHaveCategories(IEnumerable<CategoryDto> expectedCategories)
        {
            var bookCategories = _categoryRepository.GetCategoriesByBook(_book.Id);
            bookCategories.Should().HaveSameCount(expectedCategories);
            foreach (var expectedCategory in expectedCategories)
            {
                var category = bookCategories.SingleOrDefault(c => c.Id == expectedCategory.Id);
                category.Should().NotBeNull();
                category.Id.Should().Be(expectedCategory.Id);
            }
            return this;
        }

        public BookAssert ShouldHaveDeletedBook(int id)
        {
            _bookRepository.GetBookById(id).Should().BeNull();
            return this;
        }

        public BookAssert ShouldHaveDeletedBookImage(int bookId, long? imageId, string fileNamee)
        {
            var image = _bookRepository.GetBookImage(bookId);
            image.Should().BeNull();
            return this;
        }

        public BookAssert ShouldNotHaveUpdatedBookImage(int bookId, byte[] oldImage)
        {
            var imageUrl = _bookRepository.GetBookImageUrl(bookId);
            imageUrl.Should().NotBeNull();
            var image = _fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().Equal(oldImage);
            return this;
        }

        public BookAssert ShouldHaveAddedBookImage(int bookId)
        {
            var imageUrl = _bookRepository.GetBookImageUrl(bookId);
            imageUrl.Should().NotBeNull();
            var image = _fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNullOrEmpty();
            return this;
        }

        public BookAssert ShouldHaveUpdatedBookImage(int bookId, byte[] newImage )
        {
            var imageUrl = _bookRepository.GetBookImageUrl(bookId);
            imageUrl.Should().NotBeNull();
            var image = _fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNull().And.Equal(newImage);
            return this;
        }
    }
}
