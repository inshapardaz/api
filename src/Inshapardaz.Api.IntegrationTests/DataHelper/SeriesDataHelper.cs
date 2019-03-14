using System.Threading;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;

namespace Inshapardaz.Api.IntegrationTests.DataHelper
{
    public class SeriesDataHelper
    {
        private readonly ISeriesRepository _seriesRepository;

        public SeriesDataHelper(ISeriesRepository seriesRepository)
        {
            _seriesRepository = seriesRepository;
        }

        public Series Create(string series)
        {
            return _seriesRepository.AddSeries(new Series { Name = series} , CancellationToken.None).Result;
        }

        public Series Get(int seriesId)
        {
            return _seriesRepository.GetSeriesById(seriesId, CancellationToken.None).Result;
        }

        public void Delete(int seriesId)
        {
            _seriesRepository.DeleteSeries(seriesId, CancellationToken.None).Wait();
        }
    }
}