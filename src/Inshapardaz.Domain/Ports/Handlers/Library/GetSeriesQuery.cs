using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetSeriesQuery : LibraryBaseQuery<IEnumerable<SeriesModel>>
    {
        public GetSeriesQuery(int libraryId)
            : base(libraryId)
        {
        }
    }

    public class GetSeriesQueryHandler : QueryHandlerAsync<GetSeriesQuery, IEnumerable<SeriesModel>>
    {
        private readonly ISeriesRepository _seriesRepository;

        public GetSeriesQueryHandler(ISeriesRepository seriesRepository)
        {
            _seriesRepository = seriesRepository;
        }

        public override async Task<IEnumerable<SeriesModel>> ExecuteAsync(GetSeriesQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _seriesRepository.GetSeries(command.LibraryId, cancellationToken);
        }
    }
}
