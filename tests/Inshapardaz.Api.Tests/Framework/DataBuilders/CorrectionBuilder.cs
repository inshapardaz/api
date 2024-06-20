using AutoFixture;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using System.Collections.Generic;
using System.Linq;

namespace Inshapardaz.Api.Tests.Framework.DataBuilders
{
    public class CorrectionBuilder
    {
        private List<CorrectionDto> _corrections = new List<CorrectionDto>();

        private ICorrectionTestRepository _correctionRepository;

        public CorrectionBuilder(ICorrectionTestRepository correctionRepository)
        {
            _correctionRepository = correctionRepository;

        }

        public CorrectionDto Build() => Build(1).First();

        public IEnumerable<CorrectionDto> Build(int count)
        {
            var corrections = new Fixture()
                .Build<CorrectionDto>()
                .With(x => x.Language, () => RandomData.Locale)
                .CreateMany(count);

            _corrections.AddRange(corrections);
            _correctionRepository.AddCorrections(corrections);

            return corrections;
        }


        public CorrectionView BuildCorrection()
        {
            return new Fixture()
                .Build<CorrectionView>()
                .With(x => x.Language, () => RandomData.Locale)
                .Create();
        }

        public void Cleanup()
        {
            _correctionRepository.DeleteCorrections(_corrections);
        }
    }
}
