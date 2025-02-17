﻿using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.CommonWords.GetCommonWordById
{
    [TestFixture]
    public class WhenGettingCommonWordWithWritePermissions : TestBase
    {
        private HttpResponseMessage _response;
        private CommonWordDto _expected;
        private CommonWordAssert _assert;

        public WhenGettingCommonWordWithWritePermissions()
            : base(Role.Admin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var words = CommonWordBuilder.Build(4);
            _expected = words.PickRandom();

            _response = await Client.GetAsync($"/tools/{_expected.Language}/words/{_expected.Id}");
            _assert = Services.GetService<CommonWordAssert>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _assert.ShouldHaveSelfLink();
        }
        
        [Test]
        public void ShouldHaveUpdateLink()
        {
            _assert.ShouldHaveUpdateLink();
        }

        [Test]
        public void ShouldHaveDeleteLink()
        {
            _assert.ShouldHaveDeleteLink();
        }

        [Test]
        public void ShouldReturnCorrectData()
        {
            _assert.ShouldHaveCorrectWordReturned(_expected);
        }
    }
}
