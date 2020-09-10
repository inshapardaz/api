﻿using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class UpdateIssueContentRequest : LibraryAuthorisedCommand
    {
        public UpdateIssueContentRequest(ClaimsPrincipal claims, int libraryId, int periodicalId, int issueId, string language, string mimeType)
            : base(claims, libraryId)
        {
            PeriodicalId = periodicalId;
            IssueId = issueId;
            Language = language;
            MimeType = mimeType;
        }

        public int PeriodicalId { get; }
        public int IssueId { get; }

        public string Language { get; }
        public string MimeType { get; }
        public FileModel Content { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public IssueContentModel Content { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateIssueContentRequestHandler : RequestHandlerAsync<UpdateIssueContentRequest>
    {
        private readonly IIssueRepository _issueRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public UpdateIssueContentRequestHandler(IIssueRepository issueRepository, IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _issueRepository = issueRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<UpdateIssueContentRequest> HandleAsync(UpdateIssueContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var issue = await _issueRepository.GetIssueById(command.LibraryId, command.PeriodicalId, command.IssueId, cancellationToken);
            if (issue != null)
            {
                var issueContent = await _issueRepository.GetIssueContent(command.LibraryId, command.PeriodicalId, command.IssueId, command.Language, command.MimeType, cancellationToken);
                if (issueContent != null)
                {
                    if (!string.IsNullOrWhiteSpace(issueContent.ContentUrl))
                    {
                        await _fileStorage.TryDeleteFile(issueContent.ContentUrl, cancellationToken);
                    }

                    var url = await StoreFile(command.PeriodicalId, command.IssueId, command.Content.FileName, command.Content.Contents, cancellationToken);
                    issueContent.ContentUrl = url;
                    await _issueRepository.UpdateIssueContentUrl(command.LibraryId,
                                                            command.PeriodicalId,
                                                            command.IssueId,
                                                            command.Language,
                                                            command.MimeType,
                                                            url, cancellationToken);

                    command.Result.Content = issueContent;
                }
                else
                {
                    var url = await StoreFile(command.PeriodicalId, command.IssueId, command.Content.FileName, command.Content.Contents, cancellationToken);
                    command.Content.FilePath = url;
                    command.Content.IsPublic = issue.IsPublic;
                    var file = await _fileRepository.AddFile(command.Content, cancellationToken);
                    await _issueRepository.AddIssueContent(command.LibraryId, command.PeriodicalId, command.IssueId, file.Id, command.Language, command.MimeType, cancellationToken);

                    command.Result.HasAddedNew = true;
                    command.Result.Content = await _issueRepository.GetIssueContent(command.LibraryId, command.PeriodicalId, command.IssueId, command.Language, command.MimeType, cancellationToken); ;
                }
            }

            return await base.HandleAsync(command, cancellationToken);
        }

        private async Task<string> StoreFile(int periodicalId, int issueId, string fileName, byte[] contents, CancellationToken cancellationToken)
        {
            var filePath = GetUniqueFileName(periodicalId, issueId, fileName);
            return await _fileStorage.StoreFile(filePath, contents, cancellationToken);
        }

        private static string GetUniqueFileName(int periodicalId, int issueId, string fileName)
        {
            var fileNameWithoutExtension = Path.GetExtension(fileName).Trim('.');
            return $"periodicals/{periodicalId}/issues/{issueId}/{Guid.NewGuid():N}.{fileNameWithoutExtension}";
        }
    }
}
