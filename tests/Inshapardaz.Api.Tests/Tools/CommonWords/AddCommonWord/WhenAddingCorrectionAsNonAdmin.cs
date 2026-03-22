using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.CommonWords.AddCommonWord
{
    [TestFixture(Role.Reader)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.LibraryAdmin)]
    public class WhenAddingCommomWordAsNonAdmin(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var commonWord = CommonWordBuilder.Build();

            _response = await Client.PostObject($"/tools/en/words", commonWord);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeForbidden() => _response.ShouldBeForbidden();
    }
}
