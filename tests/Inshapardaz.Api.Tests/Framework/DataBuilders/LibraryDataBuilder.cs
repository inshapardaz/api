using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Domain.Models;
using RandomData = Inshapardaz.Api.Tests.Framework.Helpers.RandomData;
using Inshapardaz.Domain.Adapters.Repositories;

namespace Inshapardaz.Api.Tests.Framework.DataBuilders
{
    public class LibraryDataBuilder
    {
        private readonly FakeFileStorage _fileStorage;
        private List<FileDto> _files = new List<FileDto>();
        private bool _enablePeriodicals;
        private int? _accountId;
        private Role _role;
        private string _startWith;
        private bool _withImage = true;
        public LibraryDto Library => Libraries.FirstOrDefault();

        public IEnumerable<LibraryDto> Libraries { get; private set; }

        private IFileTestRepository _fileRepository;
        private ILibraryTestRepository _libraryRepository;

        public LibraryDataBuilder(IFileStorage fileStorage,
             IFileTestRepository fileRepository,
             ILibraryTestRepository libraryRepository)
        {
            _fileStorage = fileStorage as FakeFileStorage;
            _fileRepository = fileRepository;
            _libraryRepository = libraryRepository;
        }

        internal LibraryDataBuilder StartingWith(string startWith)
        {
            _startWith = startWith;
            return this;
        }

        public LibraryDataBuilder WithPeriodicalsEnabled(bool periodicalsEnabled = true)
        {
            _enablePeriodicals = periodicalsEnabled;
            return this;
        }

        internal LibraryDataBuilder AssignToUser(int accountId, Role role = Role.Reader)
        {
            _accountId = accountId;
            _role = role;
            return this;
        }

        internal LibraryDataBuilder WithOutAccount()
        {
            _accountId = null;
            return this;
        }

        internal LibraryDataBuilder WithoutImage()
        {
            _withImage = false;
            return this;
        }

        public LibraryDto Build(int count = 1)
        {
            var fixture = new Fixture();
            FileDto libraryImage = null;
            if (_withImage)
            {
                libraryImage = fixture.Build<FileDto>()
                                     .With(a => a.FilePath, Framework.Helpers.RandomData.FilePath)
                                     .With(a => a.IsPublic, true)
                                     .Create();
                _fileRepository.AddFile(libraryImage);

                _files.Add(libraryImage);
                _fileStorage.SetupFileContents(libraryImage.FilePath, Framework.Helpers.RandomData.Bytes);
            }

            Libraries = fixture.Build<LibraryDto>()
                                 .With(l => l.Language, "en")
                                 .With(l => l.SupportsPeriodicals, _enablePeriodicals)
                                 .With(l => l.Name, _startWith ?? RandomData.Name)
                                 .With(l => l.Description, RandomData.String)
                                 .With(l => l.PrimaryColor, RandomData.String)
                                 .With(l => l.SecondaryColor, RandomData.String)
                                 .With(l => l.OwnerEmail, RandomData.Email)
                                 .With(l => l.ImageId, libraryImage?.Id)
                                 .CreateMany(count);

            _libraryRepository.AddLibraries(Libraries);

            if (_accountId.HasValue)
            {
                _libraryRepository.AssignLibrariesToUser(Libraries, _accountId.Value, _role);
            }

            return Library;
        }

        public void CleanUp()
        {
            if (Libraries != null)
                _libraryRepository.DeleteLibraries(Libraries.Select(l => l.Id));
            _fileRepository.DeleteFiles(_files);
        }

    }
}
