using Bogus;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.Corrections.ImportCorrections
{
    [TestFixture(Role.Admin, false)]
    [TestFixture(Role.Reader, true)]
    public class WhenImportingCorrections(Role role, bool isDictionaryWriter) : TestBase(role, isDictionaryWriter: isDictionaryWriter)
    {
        private HttpResponseMessage _response;
        private CorrectionImportResultView _result;
        private readonly string _language = new Faker().Random.String2(6);

        [OneTimeSetUp]
        public async Task Setup()
        {
            CorrectionBuilder.WithLanguage(_language).WithProfile(CorrectionProfiles.Autocorrect)
                .BuildWith(("teh", "tee", 4), ("adn", "and", 2));

            _response = await Client.PostObject($"/tools/{_language}/corrections/autocorrect/bulk", new[]
            {
                new CorrectionTransferView { IncorrectText = "teh", CorrectText = "the" },
                new CorrectionTransferView { IncorrectText = "adn", CorrectText = "and" },
                new CorrectionTransferView { IncorrectText = "recieve", CorrectText = "receive", CompleteWord = true },
                new CorrectionTransferView { IncorrectText = "recieve", CorrectText = "other" },
                new CorrectionTransferView { IncorrectText = "", CorrectText = "x" },
            });
            _result = await _response.GetContent<CorrectionImportResultView>();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Services.GetService<ICorrectionTestRepository>().DeleteCorrectionsForLanguage(_language);
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldReportCounts()
        {
            _result.Added.Should().Be(1);
            _result.Updated.Should().Be(1);
            _result.Skipped.Should().Be(3);
        }

        [Test]
        public void ShouldHaveSavedList()
        {
            var saved = Services.GetService<ICorrectionTestRepository>().GetCorrections(_language, CorrectionProfiles.Autocorrect).ToList();
            saved.Should().HaveCount(3);
            saved.Single(c => c.IncorrectText == "teh").CorrectText.Should().Be("the");
            saved.Single(c => c.IncorrectText == "teh").Usage.Should().Be(4);
            saved.Single(c => c.IncorrectText == "recieve").CompleteWord.Should().BeTrue();
        }
    }

    [TestFixture(Role.Writer)]
    [TestFixture(Role.LibraryAdmin)]
    public class WhenImportingCorrectionsWithoutPermission(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup() =>
            _response = await Client.PostObject("/tools/ur/corrections/autocorrect/bulk", new[] { new CorrectionTransferView { IncorrectText = "a", CorrectText = "b" } });

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeForbidden() => _response.ShouldBeForbidden();
    }

    [TestFixture]
    public class WhenExportingCorrections() : TestBase()
    {
        private HttpResponseMessage _response;
        private List<CorrectionTransferView> _exported;
        private readonly string _language = new Faker().Random.String2(6);

        [OneTimeSetUp]
        public async Task Setup()
        {
            CorrectionBuilder.WithLanguage(_language).WithProfile(CorrectionProfiles.Punctuation).BuildWith(("..", "…", 1), (" ,", ",", 5));

            _response = await Client.GetAsync($"/tools/{_language}/corrections/punctuation/export");
            _exported = (await _response.GetContent<IEnumerable<CorrectionTransferView>>()).ToList();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnJsonFile()
        {
            _response.ShouldBeOk();
            _response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
            _response.Content.Headers.ContentDisposition.FileName.Should().Contain("punctuation");
        }

        [Test]
        public void ShouldContainTheList() =>
            _exported.Select(c => (c.IncorrectText, c.CorrectText)).Should().Equal((" ,", ","), ("..", "…"));
    }
}
