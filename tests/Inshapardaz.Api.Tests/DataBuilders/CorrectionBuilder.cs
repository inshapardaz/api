using AutoFixture;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Inshapardaz.Domain.Adapters;

namespace Inshapardaz.Api.Tests.DataBuilders
{
    public class CorrectionBuilder
    {
        private List<CorrectionDto> _corrections = new List<CorrectionDto>();
        private IDbConnection _connection;

        public CorrectionBuilder(IProvideConnection connectionProvider)
        {
            _connection = connectionProvider.GetConnection();
        }

        public CorrectionDto Build() => Build(1).First();

        public IEnumerable<CorrectionDto> Build(int count)
        {
            var corrections = new Fixture()
                .Build<CorrectionDto>()
                .With(x => x.Language, () => RandomData.Locale)
                .CreateMany(count);

            _corrections.AddRange(corrections);
            _connection.AddCorrections(corrections);

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
            _connection.DeleteCorrections(_corrections);
        }
    }
}
