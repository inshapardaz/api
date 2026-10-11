using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Tools;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.Spelling.CheckSpelling
{
    [TestFixture]
    public class WhenCheckingTooManyWords() : TestBase()
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var words = Enumerable.Range(0, 2001).Select(i => $"word{i}");
            _response = await Client.PostObject("/tools/ur/spellchecker/check", new SpellCheckRequestView { Words = words });
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnBadRequest() => _response.ShouldBeBadRequest();
    }
}
