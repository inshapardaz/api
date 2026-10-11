using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Tools;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.Spelling.RecordReplacement
{
    [TestFixture]
    public class WhenRecordingReplacementAsReader() : TestBase(Role.Reader)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup() =>
            _response = await Client.PostObject("/tools/ur/spellchecker/replace", new SpellingReplacementView { Word = "kitb", Replacement = "kitab" });

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeForbidden() => _response.ShouldBeForbidden();
    }

    [TestFixture]
    public class WhenRecordingReplacementAsAnonymous() : TestBase()
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup() =>
            _response = await Client.PostObject("/tools/ur/spellchecker/replace", new SpellingReplacementView { Word = "kitb", Replacement = "kitab" });

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeUnauthorized() => _response.ShouldBeUnauthorized();
    }
}
