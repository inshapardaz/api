using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class GetSeriesQuery : LibraryBaseQuery<Page<SeriesModel>>
    {
        public GetSeriesQuery(int libraryId, int pageNumber, int pageSize)
            : base(libraryId)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public string Query { get; set; }
    }

    public class GetSeriesQueryHandler : QueryHandlerAsync<GetSeriesQuery, Page<SeriesModel>>
    {
        private readonly ISeriesRepository _seriesRepository;
        private readonly IFileRepository _fileRepository;

        public GetSeriesQueryHandler(ISeriesRepository seriesRepository, IFileRepository fileRepository)
        {
            _seriesRepository = seriesRepository;
            _fileRepository = fileRepository;
        }

        public override async Task<Page<SeriesModel>> ExecuteAsync(GetSeriesQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            var authors = (string.IsNullOrWhiteSpace(query.Query))
             ? await _seriesRepository.GetSeries(query.LibraryId, query.PageNumber, query.PageSize, cancellationToken)
             : await _seriesRepository.FindSeries(query.LibraryId, query.Query, query.PageNumber, query.PageSize, cancellationToken);

            foreach (var author in authors.Data)
            {
                if (author != null && author.ImageId.HasValue)
                {
                    author.ImageUrl = await ImageHelper.TryConvertToPublicFile(author.ImageId.Value, _fileRepository, cancellationToken);
                }
            }

            return authors;
        }
    }
}
