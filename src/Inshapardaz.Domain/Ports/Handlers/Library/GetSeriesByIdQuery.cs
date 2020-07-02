using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
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
        private readonly IFileRepository _fileRepository;

        public GetSeriesByIdQueryHandler(ISeriesRepository seriesRepository, IBookRepository bookRepository, IFileRepository fileRepository)
        {
            _seriesRepository = seriesRepository;
            _bookRepository = bookRepository;
            _fileRepository = fileRepository;
        }

        public override async Task<SeriesModel> ExecuteAsync(GetSeriesByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var series = await _seriesRepository.GetSeriesById(command.LibraryId, command.SeriesId, cancellationToken);

            if (series != null && series.ImageId.HasValue)
            {
                series.ImageUrl = await ImageHelper.TryConvertToPublicFile(series.ImageId.Value, _fileRepository, cancellationToken);
            }

            return series;
        }
    }
}
