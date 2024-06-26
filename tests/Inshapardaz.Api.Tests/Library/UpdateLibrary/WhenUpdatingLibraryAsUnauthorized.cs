﻿using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.UpdateLibrary
{
    [TestFixture]
    public class WhenUpdatingLibraryAsUnauthorized : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var library = new LibraryView { Name = RandomData.Name, Language = RandomData.Locale, SupportsPeriodicals = RandomData.Bool, FileStoreType = "Database" };

            _response = await Client.PutObject($"/libraries/{LibraryId}", library);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorisedResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
