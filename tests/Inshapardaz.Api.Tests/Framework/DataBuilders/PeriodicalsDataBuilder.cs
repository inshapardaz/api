using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Views.Library;
using RandomData = Inshapardaz.Api.Tests.Framework.Helpers.RandomData;
using Inshapardaz.Domain.Models.Library;
using Bogus;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Tests.Framework.DataBuilders
{

    public class PeriodicalsDataBuilder
    {
        private readonly CategoriesDataBuilder _categoriesBuilder;
        private readonly TagsDataBuilder _tagsBuilder;
        private readonly FakeFileStorage _fileStorage;

        private List<PeriodicalDto> _periodicals;
        private readonly List<FileDto> _files = new List<FileDto>();
        private List<CategoryDto> _categories = new List<CategoryDto>();
        private List<IssueDto> _issues = new List<IssueDto>();
        private List<TagDto> _tags = new List<TagDto>();

        private PeriodicalFrequency? _frequency = null;

        private int _libraryId;
        private int _categoriesCount, _tagsCount;
        private string _language = "en";
        private bool _hasImage = true;
        private int _issueCount = 0;

        public IEnumerable<PeriodicalDto> Periodicals => _periodicals;

        private IFileTestRepository _fileRepository;
        private IPeriodicalTestRepository _periodicalRepository;
        private IIssueTestRepository _issueRepository;
        private ICategoryTestRepository _categoryRepository;
        public ITagTestRepository _tagRepository;

        public PeriodicalsDataBuilder(IFileStorage fileStorage,
                                CategoriesDataBuilder categoriesBuilder,
                                TagsDataBuilder tagsBuilder,
                                IFileTestRepository fileRepository,
                                IPeriodicalTestRepository periodicalRepository,
                                IIssueTestRepository issueRepository,
                                ICategoryTestRepository categoryRepository,
                                ITagTestRepository tagRepository)
        {
            _fileStorage = fileStorage as FakeFileStorage;
            _categoriesBuilder = categoriesBuilder;
            _tagsBuilder = tagsBuilder;
            _fileRepository = fileRepository;
            _periodicalRepository = periodicalRepository;
            _issueRepository = issueRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
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
        
        
        public PeriodicalsDataBuilder WithTags(int tagsCount)
        {
            _tagsCount = tagsCount;
            return this;
        }
        public PeriodicalsDataBuilder WithTags(params TagDto[] tags)
        {
            _tags = tags.ToList();
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
                          .With(b => b.Tags, _tags.Any() ? _tags.Select(c => c.ToView()) : Array.Empty<TagView>())
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

            IEnumerable<CategoryDto> categories;

            if (_categoriesCount > 0 && !_categories.Any())
            {
                categories = _categoriesBuilder.WithLibrary(_libraryId).Build(_categoriesCount);
            }
            else
            {
                categories = _categories;
            }
            
            IEnumerable<TagDto> tags;

            if (_tagsCount > 0 && !_tags.Any())
            {
                tags = _tagsBuilder.WithLibrary(_libraryId).Build(_tagsCount);
            }
            else
            {
                tags = _tags;
            }
            
            foreach (var periodical in _periodicals)
            {
                _periodicalRepository.AddPeriodical(periodical);
                if (_hasImage)
                {

                    var fileName = FilePathHelper.GetPeriodicalImageFileName(RandomData.FileName(MimeTypes.Jpg));
                    var filePath = FilePathHelper.GetPeriodicalImageFilePath(periodical.Id, fileName);
                    var periodicalImage = fixture.Build<FileDto>()
                        .With(a => a.FilePath, filePath)
                        .With(a => a.IsPublic, true)
                        .Create();
                    _fileRepository.AddFile(periodicalImage);

                    _files.Add(periodicalImage);
                    _fileStorage.SetupFileContents(periodicalImage.FilePath, RandomData.Bytes);

                    periodical.ImageId = periodicalImage.Id;
                }
                else
                {
                    periodical.ImageId = null;
                }

                _periodicalRepository.UpdatePeriodical(periodical);


                var issues = fixture.Build<IssueDto>()
                    .With(x => x.PeriodicalId, periodical.Id)
                    .CreateMany(_issueCount);
                _issueRepository.AddIssues(issues);
                _issues.AddRange(issues);

                if (categories != null && categories.Any())
                    _categoryRepository.AddPeriodicalToCategories(periodical.Id, categories);
                
                
                if (tags != null && tags.Any())
                    _tagRepository.AddPeriodicalToTags(periodical.Id, tags);
            }

            return _periodicals;
        }

        public void CleanUp()
        {
            _issueRepository.DeleteIssues(_issues);
            _fileRepository.DeleteFiles(_files);
            _periodicalRepository.DeletePeriodicals(_periodicals);
            _categoriesBuilder.CleanUp();
        }
    }
}
