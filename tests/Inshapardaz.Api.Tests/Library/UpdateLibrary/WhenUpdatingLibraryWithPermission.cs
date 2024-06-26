﻿using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.UpdateLibrary
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    public class WhenUpdatingLibraryWithPermission : TestBase
    {
        private HttpResponseMessage _response;

        private LibraryView _expectedLibrary;
        private LibraryAssert _assert;

        public WhenUpdatingLibraryWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _expectedLibrary = new LibraryView { 
                Name = RandomData.Name, 
                Language = RandomData.Locale, 
                SupportsPeriodicals = RandomData.Bool,
                DatabaseConnection = RandomData.String,
                FileStoreType = FileStoreTypes.S3Storage.ToDescription(),
                FileStoreSource = RandomData.String
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}", _expectedLibrary);
            var returnedView = await _response.GetContent<LibraryView>();
            _assert = Services.GetService<LibraryAssert>().ForResponse(_response).ForLibrary(returnedView.Id);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveReturnedUpdatedTheLibrary()
        {
            if (_role == Role.Admin)
            {
                _assert.ShouldBeSameAs(_expectedLibrary);
            } else
            {
                _assert.ShouldBeSameWithNoConfiguration(_expectedLibrary);
            }
        }

        [Test]
        public void ShouldHaveUpdatedLibrary()
        {
            if (_role == Role.Admin)
            {
                _assert.ShouldHaveUpdatedLibrary();
            }
            else
            {
                _assert.ShouldHaveUpdatedLibraryWithoutConfiguration();
            }
        }
    }
}
