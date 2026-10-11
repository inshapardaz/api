using Bogus;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.Corrections.Usage
{
    [TestFixture(Role.Writer, false)]
    [TestFixture(Role.LibraryAdmin, false)]
    [TestFixture(Role.Reader, true)]
    public class WhenRecordingCorrectionUsage(Role role, bool isDictionaryWriter) : TestBase(role, isDictionaryWriter: isDictionaryWriter)
    {
        private HttpResponseMessage _response;
        private List<CorrectionDto> _corrections;
        private CorrectionDto _otherList;
        private readonly string _language = new Faker().Random.String2(6);

        [OneTimeSetUp]
        public async Task Setup()
        {
            _corrections = CorrectionBuilder.WithLanguage(_language).WithProfile(CorrectionProfiles.Autocorrect)
                .BuildWith(("teh", "the", 10), ("adn", "and", 0)).ToList();
            _otherList = CorrectionBuilder.WithProfile(CorrectionProfiles.Punctuation).BuildWith(("..", "…", 0)).Single();

            _response = await Client.PostObject($"/tools/{_language}/corrections/autocorrect/usage", new[]
            {
                new CorrectionUsageView { Id = _corrections[0].Id, Count = 2 },
                new CorrectionUsageView { Id = _corrections[0].Id, Count = 1 },
                new CorrectionUsageView { Id = _corrections[1].Id, Count = 5 },
                new CorrectionUsageView { Id = _corrections[1].Id, Count = -4 },
                new CorrectionUsageView { Id = _otherList.Id, Count = 7 },
            });
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnNoContent() => _response.ShouldBeNoContent();

        [Test]
        public void ShouldAddCountsToUsage()
        {
            var repository = Services.GetService<ICorrectionTestRepository>();
            repository.GetCorrectionById(_corrections[0].Id).Usage.Should().Be(13);
            repository.GetCorrectionById(_corrections[1].Id).Usage.Should().Be(5);
        }

        [Test]
        public void ShouldIgnoreCorrectionsFromOtherLists() =>
            Services.GetService<ICorrectionTestRepository>().GetCorrectionById(_otherList.Id).Usage.Should().Be(0);
    }

    [TestFixture]
    public class WhenRecordingCorrectionUsageAsReader() : TestBase(Role.Reader)
    {
        private HttpResponseMessage _response;
        private CorrectionDto _correction;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _correction = CorrectionBuilder.WithProfile(CorrectionProfiles.Autocorrect).BuildWith(("teh", "the", 0)).Single();
            _response = await Client.PostObject($"/tools/{_correction.Language}/corrections/autocorrect/usage",
                new[] { new CorrectionUsageView { Id = _correction.Id, Count = 1 } });
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeForbidden() => _response.ShouldBeForbidden();

        [Test]
        public void ShouldNotChangeUsage() =>
            Services.GetService<ICorrectionTestRepository>().GetCorrectionById(_correction.Id).Usage.Should().Be(0);
    }

    [TestFixture]
    public class WhenAddingCorrectionToUnknownList() : TestBase(Role.Admin)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup() =>
            _response = await Client.PostObject("/tools/ur/corrections/grammar", new CorrectionView { IncorrectText = "a", CorrectText = "b" });

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeBadRequest() => _response.ShouldBeBadRequest();
    }
}
