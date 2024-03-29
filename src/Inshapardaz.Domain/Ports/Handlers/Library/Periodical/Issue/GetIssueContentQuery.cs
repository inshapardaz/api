﻿using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Periodical.Issue
{
    public class GetIssueContentQuery : LibraryBaseQuery<IssueContentModel>
    {
        public GetIssueContentQuery(int libraryId, int periodicalId, int volumeNumber, int issueNumber, string language, string mimeType, int? accountId)
            : base(libraryId)
        {
            PeriodicalId = periodicalId;
            VolumeNumber = volumeNumber;
            IssueNumber = issueNumber;
            MimeType = mimeType;
            AccountId = accountId;
            Language = language;
        }

        public int PeriodicalId { get; set; }
        public int VolumeNumber { get; set; }
        public int IssueNumber { get; set; }

        public string MimeType { get; set; }
        public int? AccountId { get; }
        public string Language { get; set; }
    }

    public class GetIssueContentQueryHandler : QueryHandlerAsync<GetIssueContentQuery, IssueContentModel>
    {
        private readonly ILibraryRepository _libraryRepository;
        private readonly IIssueRepository _issueRepository;
        private readonly IFileRepository _fileRepository;

        public GetIssueContentQueryHandler(ILibraryRepository libraryRepository, IIssueRepository issueRepository, IFileRepository fileRepository)
        {
            _libraryRepository = libraryRepository;
            _issueRepository = issueRepository;
            _fileRepository = fileRepository;
        }

        public override async Task<IssueContentModel> ExecuteAsync(GetIssueContentQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            var issue = await _issueRepository.GetIssue(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, cancellationToken);
            if (issue == null)
            {
                throw new NotFoundException();
            }

            if (!issue.IsPublic && !command.AccountId.HasValue)
            {
                throw new UnauthorizedException();
            }

            if (string.IsNullOrWhiteSpace(command.Language))
            {
                var library = await _libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
                if (library == null)
                {
                    throw new BadRequestException();
                }

                command.Language = library.Language;
            }

            var bookContent = await _issueRepository.GetIssueContent(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.Language, command.MimeType, cancellationToken);

            //if (bookContent != null)
            //{
            //    if (command.AccountId.HasValue)
            //    {
            //        await _issueRepository.AddRecentBook(command.LibraryId, command.AccountId.Value, command.BookId, cancellationToken);
            //    }

            //    if (issue.IsPublic)
            //    {
            //        bookContent.ContentUrl = await ImageHelper.TryConvertToPublicFile(bookContent.FileId, _fileRepository, cancellationToken);
            //    }
            //}

            return bookContent;
        }
    }
}
