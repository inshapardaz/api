using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetSeriesByIdQuery : LibraryBaseQuery<SeriesModel>
    {
        public GetSeriesByIdQuery(int libraryId, int seriesId)
            : base(libraryId)
        {
            SeriesId = seriesId;
        }

        public int SeriesId { get; }
    }

    public class GetSeriesByIdQueryHandler : QueryHandlerAsync<GetSeriesByIdQuery, SeriesModel>
    {
        private readonly ISeriesRepository _seriesRepository;
        private readonly IBookRepository _bookRepository;

        public GetSeriesByIdQueryHandler(ISeriesRepository seriesRepository, IBookRepository bookRepository)
        {
            _seriesRepository = seriesRepository;
            _bookRepository = bookRepository;
        }

        public override async Task<SeriesModel> ExecuteAsync(GetSeriesByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _seriesRepository.GetSeriesById(command.LibraryId, command.SeriesId, cancellationToken);
        }
    }
}
