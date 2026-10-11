using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Tools;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Transliteration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.Transliteration
{
    [TestFixture(Role.Admin, false)]
    [TestFixture(Role.Reader, true)]
    public class WhenAddingTransliterationEntry(Role role, bool isDictionaryWriter) : TestBase(role, isDictionaryWriter: isDictionaryWriter)
    {
        private HttpResponseMessage _response;
        private HttpResponseMessage _duplicate;
        private TransliterationEntryView _view;
        private TransliterationResultView _converted;
        private readonly string _roman = UniqueRoman.Next();

        [OneTimeSetUp]
        public async Task Setup()
        {
            // Warm the cache first, so the conversion below proves the add refreshed it.
            await Client.PostObject("/tools/hi/transliterate", new TransliterateRequestView { Text = _roman, To = TransliterationScript.HindiDevanagari });

            _response = await Client.PostObject("/tools/hi/transliteration/entries", new TransliterationEntryView { Roman = $"  {_roman.ToUpperInvariant()} ", Native = "भगवान", Meaning = "god" });
            _view = await _response.GetContent<TransliterationEntryView>();
            _duplicate = await Client.PostObject("/tools/hi/transliteration/entries", new TransliterationEntryView { Roman = _roman, Native = "x" });

            _converted = await (await Client.PostObject("/tools/hi/transliterate", new TransliterateRequestView { Text = _roman, To = TransliterationScript.HindiDevanagari }))
                .GetContent<TransliterationResultView>();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Services.GetService<ITransliterationTestRepository>().DeleteEntries("hi", [_roman]);
            Cleanup();
        }

        [Test]
        public void ShouldBeCreated() => _response.ShouldBeCreated();

        [Test]
        public void ShouldStoreRomanLowerCasedAndTrimmed()
        {
            _view.Roman.Should().Be(_roman);
            Services.GetService<ITransliterationTestRepository>().GetEntryByRoman("hi", _roman).Native.Should().Be("भगवान");
        }

        [Test]
        public void ShouldHaveEditingLinks()
        {
            _view.UpdateLink().ShouldPut($"/tools/hi/transliteration/entries/{_view.Id}");
            _view.DeleteLink().ShouldDelete($"/tools/hi/transliteration/entries/{_view.Id}");
        }

        [Test]
        public void ShouldRejectDuplicate() => _duplicate.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);

        [Test]
        public void ShouldUseNewEntryImmediately() => _converted.Result.Should().Be("भगवान");
    }

    [TestFixture(Role.Writer)]
    [TestFixture(Role.LibraryAdmin)]
    public class WhenAddingTransliterationEntryWithoutPermission(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup() =>
            _response = await Client.PostObject("/tools/ur/transliteration/entries", new TransliterationEntryView { Roman = UniqueRoman.Next(), Native = "x" });

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeForbidden() => _response.ShouldBeForbidden();
    }

    [TestFixture]
    public class WhenChangingTransliterationEntries() : TestBase(Role.Admin)
    {
        private HttpResponseMessage _updateResponse;
        private HttpResponseMessage _deleteResponse;
        private PageView<TransliterationEntryView> _page;
        private TransliterationEntryDto _updated;
        private TransliterationEntryDto _deleted;
        private readonly string _prefix = UniqueRoman.Next();

        [OneTimeSetUp]
        public async Task Setup()
        {
            var repository = Services.GetService<ITransliterationTestRepository>();
            _updated = repository.AddEntry("ur", $"{_prefix}a", "الف");
            _deleted = repository.AddEntry("ur", $"{_prefix}b", "ب");
            repository.AddEntry("ur", $"{_prefix}c", "ج");

            _updateResponse = await Client.PutObject($"/tools/ur/transliteration/entries/{_updated.Id}", new TransliterationEntryView { Roman = $"{_prefix}a", Native = "آ", Category = "letter" });
            _deleteResponse = await Client.DeleteAsync($"/tools/ur/transliteration/entries/{_deleted.Id}");
            _page = await (await Client.GetAsync($"/tools/ur/transliteration/entries?query={_prefix}")).GetContent<PageView<TransliterationEntryView>>();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Services.GetService<ITransliterationTestRepository>().DeleteEntries("ur", [$"{_prefix}a", $"{_prefix}b", $"{_prefix}c"]);
            Cleanup();
        }

        [Test]
        public void ShouldUpdateEntry()
        {
            _updateResponse.ShouldBeOk();
            var saved = Services.GetService<ITransliterationTestRepository>().GetEntryByRoman("ur", $"{_prefix}a");
            saved.Native.Should().Be("آ");
            saved.Category.Should().Be("letter");
        }

        [Test]
        public void ShouldDeleteEntry()
        {
            _deleteResponse.ShouldBeNoContent();
            Services.GetService<ITransliterationTestRepository>().GetEntryByRoman("ur", $"{_prefix}b").Should().BeNull();
        }

        [Test]
        public void ShouldListMatchingEntries()
        {
            _page.Data.Select(e => e.Roman).Should().Equal($"{_prefix}a", $"{_prefix}c");
            _page.CreateLink().Should().NotBeNull();
        }
    }

    [TestFixture]
    public class WhenImportingAndExportingTransliterationEntries() : TestBase(Role.Reader, isDictionaryWriter: true)
    {
        private TransliterationImportResultView _result;
        private string _export;
        private readonly string _prefix = UniqueRoman.Next();

        [OneTimeSetUp]
        public async Task Setup()
        {
            Services.GetService<ITransliterationTestRepository>().AddEntry("ur", $"{_prefix}a", "old");

            var response = await Client.PostObject("/tools/ur/transliteration/entries/bulk", new[]
            {
                new TransliterationEntryView { Roman = $"{_prefix}a", Native = "new" },
                new TransliterationEntryView { Roman = $"{_prefix}b", Native = "ب" },
                new TransliterationEntryView { Roman = $"{_prefix.ToUpperInvariant()}B", Native = "repeat" },
                new TransliterationEntryView { Roman = $"{_prefix}c", Native = "" },
            });
            response.ShouldBeOk();
            _result = await response.GetContent<TransliterationImportResultView>();
            _export = await (await Client.GetAsync("/tools/ur/transliteration/entries/export")).Content.ReadAsStringAsync();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Services.GetService<ITransliterationTestRepository>().DeleteEntries("ur", [$"{_prefix}a", $"{_prefix}b", $"{_prefix}c"]);
            Cleanup();
        }

        [Test]
        public void ShouldReportCounts()
        {
            _result.Added.Should().Be(1);
            _result.Updated.Should().Be(1);
            _result.Skipped.Should().Be(2);
        }

        [Test]
        public void ShouldReplaceExistingEntries() =>
            Services.GetService<ITransliterationTestRepository>().GetEntryByRoman("ur", $"{_prefix}a").Native.Should().Be("new");

        [Test]
        public void ShouldExportEntries() => _export.Should().Contain($"\"roman\": \"{_prefix}b\"");
    }
}
