﻿using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.UploadPeriodicalImage
{
    [TestFixture]
    public class WhenUploadingPeriodicalImageAsReader : TestBase
    {
        private HttpResponseMessage _response;
        private PeriodicalAssert _assert;
        private int _periodicalId;
        private byte[] _oldImage;

        public WhenUploadingPeriodicalImageAsReader()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var periodical = PeriodicalBuilder.WithLibrary(LibraryId).Build();
            _periodicalId = periodical.Id;
            var imageUrl = PeriodicalTestRepository.GetPeriodicalImageUrl(_periodicalId);
            _oldImage = await FileStore.GetFile(imageUrl, CancellationToken.None);

            _response = await Client.PutFile($"/libraries/{LibraryId}/periodicals/{_periodicalId}/image", RandomData.Bytes);
            _assert = Services.GetService<PeriodicalAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            PeriodicalBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbidResult()
        {
            _response.ShouldBeForbidden();
        }

        [Test]
        public void ShouldNotHaveUpdatedPeriodicalImage()
        {
            _assert.ShouldNotHaveUpdatedPeriodicalImage(_periodicalId, _oldImage);
        }
    }
}
