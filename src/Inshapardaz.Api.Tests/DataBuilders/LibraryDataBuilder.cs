using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Fakes;
using Inshapardaz.Database.SqlServer;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories;
using RandomData = Inshapardaz.Api.Tests.Helpers.RandomData;

namespace Inshapardaz.Api.Tests.DataBuilders
{
    public class LibraryDataBuilder
    {
        private IDbConnection _connection;
        private readonly FakeFileStorage _fileStorage;
        private List<FileDto> _files = new List<FileDto>();
        private bool _enablePeriodicals;
        private int? _accountId;
        private Role _role;
        private string _startWith;
        private bool _withImage = true;
        public LibraryDto Library => Libraries.FirstOrDefault();

        public IEnumerable<LibraryDto> Libraries { get; private set; }

        public LibraryDataBuilder(IProvideConnection connectionProvider, IFileStorage fileStorage)
        {
            _connection = connectionProvider.GetConnection();
            _fileStorage = fileStorage as FakeFileStorage;
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
                                     .With(a => a.FilePath, Helpers.RandomData.BlobUrl)
                                     .With(a => a.IsPublic, true)
                                     .Create();
                _connection.AddFile(libraryImage);

                _files.Add(libraryImage);
                _fileStorage.SetupFileContents(libraryImage.FilePath, Helpers.RandomData.Bytes);
                _connection.AddFile(libraryImage);
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

            _connection.AddLibraries(Libraries);

            if (_accountId.HasValue)
            {
                _connection.AssignLibrariesToUser(Libraries, _accountId.Value, _role);
            }

            return Library;
        }

        public void CleanUp()
        {
            if (Libraries != null)
                _connection.DeleteLibraries(Libraries.Select(l => l.Id));
            _connection.DeleteFiles(_files);
        }

    }
}
