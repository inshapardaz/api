using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataBuilders;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Accounts.DictionaryWriter
{
    [TestFixture]
    public class WhenAddingDictionaryWriterRoleToAccountThatDoesNotExist() : TestBase(Role.Admin)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.PutAsync($"/accounts/{-RandomData.Number}/roles/dictionary-writer", null);
        }

        [Test]
        public void ShouldReturnBadRequest() => _response.ShouldBeBadRequest();
    }
}
