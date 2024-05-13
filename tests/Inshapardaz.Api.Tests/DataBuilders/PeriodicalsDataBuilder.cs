using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Fakes;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Adapters.Database.SqlServer;
using RandomData = Inshapardaz.Api.Tests.Helpers.RandomData;
using Inshapardaz.Domain.Models.Library;
using Bogus;
using Inshapardaz.Domain.Adapters;

namespace Inshapardaz.Api.Tests.DataBuilders
{

    public class PeriodicalsDataBuilder
    {
        private readonly IDbConnection _connection;
        private readonly CategoriesDataBuilder _categoriesBuilder;

        private readonly FakeFileStorage _fileStorage;

        private List<PeriodicalDto> _periodicals;
        private readonly List<FileDto> _files = new List<FileDto>();
        private List<CategoryDto> _categories = new List<CategoryDto>();
        private List<IssueDto> _issues = new List<IssueDto>();

        private PeriodicalFrequency? _frequency = null;

        private int _libraryId;
        private int _categoriesCount;
        private string _language = "en";
        private bool _hasImage = true;
        private int _issueCount = 0;

        public IEnumerable<PeriodicalDto> Periodicals => _periodicals;

        public PeriodicalsDataBuilder(IProvideConnection connectionProvider, IFileStorage fileStorage,
                                CategoriesDataBuilder categoriesBuilder)
        {
            _connection = connectionProvider.GetConnection();
            _fileStorage = fileStorage as FakeFileStorage;
            _categoriesBuilder = categoriesBuilder;
        }

        public PeriodicalsDataBuilder WithCategories(int categoriesCount = 1)
        {
            _categoriesCount = categoriesCount;
            return this;
        }

        public PeriodicalsDataBuilder WithCategory(CategoryDto category)
        {
            _categories.Add(category);
            return this;
        }

        public PeriodicalsDataBuilder WithCategories(IEnumerable<CategoryDto> categories)
        {
            _categories = categories.ToList();
            return this;
        }

        public PeriodicalsDataBuilder WithIssues(int issueCount)
        {
            _issueCount = issueCount;
            return this;
        }
        public PeriodicalsDataBuilder WithLanguage(string language)
        {
            _language = language;
            return this;
        }

        public PeriodicalsDataBuilder WithNoImage()
        {
            _hasImage = false;
            return this;
        }

        public PeriodicalsDataBuilder WithLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public PeriodicalsDataBuilder WithFrequency(PeriodicalFrequency frequency)
        {
            _frequency = frequency;
            return this;
        }

        public PeriodicalView BuildView()
        {
            var fixture = new Fixture();

            return fixture.Build<PeriodicalView>()
                          .With(b => b.Categories, _categories.Any() ? _categories.Select(c => c.ToView()) : new CategoryView[0])
                          .Create();
        }

        public PeriodicalDto Build()
        {
            return Build(1).Single();
        }

        public IEnumerable<PeriodicalDto> Build(int numberOfBooks)
        {
            var fixture = new Fixture();

            _periodicals = fixture.Build<PeriodicalDto>()
                          .With(b => b.LibraryId, _libraryId)
                          .With(b => b.Language, _language)
                          .With(b => b.ImageId, _hasImage ? RandomData.Number : 0)
                          .With(b => b.Frequency, () => _frequency.HasValue ? _frequency.Value : new Faker().PickRandom<PeriodicalFrequency>())
                          .CreateMany(numberOfBooks)
                          .ToList();

            foreach (var periodical in _periodicals)
            {
                FileDto bookImage = null;
                if (_hasImage)
                {
                    bookImage = fixture.Build<FileDto>()
                                         .With(a => a.FilePath, RandomData.BlobUrl)
                                         .With(a => a.IsPublic, true)
                                         .Create();
                    _connection.AddFile(bookImage);

                    _files.Add(bookImage);
                    _fileStorage.SetupFileContents(bookImage.FilePath, RandomData.Bytes);
                    _connection.AddFile(bookImage);

                    periodical.ImageId = bookImage.Id;
                }
                else
                {
                    periodical.ImageId = null;
                }

                _connection.AddPeriodical(periodical);


                var issues = fixture.Build<IssueDto>()
                    .With(x => x.PeriodicalId, periodical.Id)
                    .CreateMany(_issueCount);
                _connection.AddIssues(issues);
                _issues.AddRange(issues);

                IEnumerable<CategoryDto> categories;

                if (_categoriesCount > 0 && !_categories.Any())
                {
                    categories = _categoriesBuilder.WithLibrary(_libraryId).Build(_categoriesCount);
                }
                else
                {
                    categories = _categories;
                }

                if (categories != null && categories.Any())
                    _connection.AddPeriodicalToCategories(periodical.Id, categories);
            }

            return _periodicals;
        }

        public void CleanUp()
        {
            _connection.DeletePeriodicals(_periodicals);
            _connection.DeleteIssues(_issues);
            _connection.DeleteFiles(_files);
            _categoriesBuilder.CleanUp();
        }
    }
}
