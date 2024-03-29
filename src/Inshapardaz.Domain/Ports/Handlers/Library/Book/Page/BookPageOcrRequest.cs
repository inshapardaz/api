﻿using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Handlers.Library;
using Paramore.Brighter;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Book.Page
{
    public class BookPageOcrRequest : LibraryBaseCommand
    {
        public BookPageOcrRequest(int libraryId, int bookId, int sequenceNumber, string apiKey)
            : base(libraryId)
        {
            BookId = bookId;
            SequenceNumber = sequenceNumber;
            ApiKey = apiKey;
        }

        public int BookId { get; set; }
        public int SequenceNumber { get; }
        public string ApiKey { get; }
    }

    public class BookPageOcrRequestHandler : RequestHandlerAsync<BookPageOcrRequest>
    {
        private readonly IBookPageRepository _bookPageRepository;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IProvideOcr _ocr;

        public BookPageOcrRequestHandler(IBookPageRepository bookPageRepository, IQueryProcessor queryProcessor, IProvideOcr ocr)
        {
            _bookPageRepository = bookPageRepository;
            _queryProcessor = queryProcessor;
            _ocr = ocr;
        }

        public override async Task<BookPageOcrRequest> HandleAsync(BookPageOcrRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var bookPage = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.SequenceNumber, cancellationToken);
            if (bookPage != null && bookPage.ImageId.HasValue)
            {
                var image = await _queryProcessor.ExecuteAsync(new GetFileQuery(bookPage.ImageId.Value, 0, 0));

                if (image != null)
                {
                    var text = await _ocr.PerformOcr(image.Contents, command.ApiKey, cancellationToken);
                    bookPage.Text = text;
                    await _bookPageRepository.UpdatePage(command.LibraryId, bookPage.BookId, bookPage.SequenceNumber, text, bookPage.ImageId.Value, bookPage.Status, bookPage.ChapterId, cancellationToken);
                    return await base.HandleAsync(command, cancellationToken);
                }
            }

            throw new NotFoundException();
        }
    }
}
