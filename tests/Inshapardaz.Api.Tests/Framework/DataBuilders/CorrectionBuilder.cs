using AutoFixture;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Tests.Framework.DataBuilders
{
    public class CorrectionBuilder(ICorrectionTestRepository correctionRepository)
    {
        private List<CorrectionDto> _corrections = new List<CorrectionDto>();
        private string _language;
        private string _profile;

        public static string RandomProfile => RandomData.PickRandom(CorrectionProfiles.All);

        public CorrectionBuilder WithLanguage(string language)
        {
            _language = language;
            return this;
        }

        public CorrectionBuilder WithProfile(string profile)
        {
            _profile = profile;
            return this;
        }

        public CorrectionDto Build() => Build(1).First();

        // Corrections with these exact texts and usage counts, in the builder's language and profile.
        public IEnumerable<CorrectionDto> BuildWith(params (string Incorrect, string Correct, long Usage)[] entries)
        {
            var corrections = entries.Select(e => new CorrectionDto
            {
                Language = _language ?? RandomData.Locale,
                Profile = _profile ?? RandomProfile,
                IncorrectText = e.Incorrect,
                CorrectText = e.Correct,
                Usage = e.Usage
            }).ToList();

            _corrections.AddRange(corrections);
            correctionRepository.AddCorrections(corrections);
            return corrections;
        }

        public IEnumerable<CorrectionDto> Build(int count)
        {
            var corrections = new Fixture()
                .Build<CorrectionDto>()
                .With(x => x.Language, () => _language ?? RandomData.Locale)
                .With(x => x.Profile, () => _profile ?? RandomProfile)
                .With(x => x.Usage, 0)
                .CreateMany(count);

            _corrections.AddRange(corrections);
            correctionRepository.AddCorrections(corrections);

            return corrections;
        }


        public CorrectionView BuildCorrection()
        {
            return new Fixture()
                .Build<CorrectionView>()
                .With(x => x.Language, () => RandomData.Locale)
                .With(x => x.Profile, () => RandomProfile)
                .Create();
        }

        public void Cleanup() => correctionRepository.DeleteCorrections(_corrections);
    }
}
