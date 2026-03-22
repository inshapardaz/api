using AutoFixture;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;

namespace Inshapardaz.Api.Tests.Framework.DataBuilders
{
    public class CorrectionBuilder(ICorrectionTestRepository correctionRepository)
    {
        private List<CorrectionDto> _corrections = new List<CorrectionDto>();

        public CorrectionDto Build() => Build(1).First();

        public IEnumerable<CorrectionDto> Build(int count)
        {
            var corrections = new Fixture()
                .Build<CorrectionDto>()
                .With(x => x.Language, () => RandomData.Locale)
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
                .Create();
        }

        public void Cleanup() => correctionRepository.DeleteCorrections(_corrections);
    }
}
