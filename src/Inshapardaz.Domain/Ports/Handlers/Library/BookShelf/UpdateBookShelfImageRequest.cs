﻿using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static Lucene.Net.Util.Fst.Util;

namespace Inshapardaz.Domain.Ports.Handlers.Library.BookShelf
{
    public class UpdateBookShelfImageRequest : LibraryBaseCommand
    {
        public UpdateBookShelfImageRequest(int libraryId, int bookShelfId, int accountId)
            : base(libraryId)
        {
            BookShelfId = bookShelfId;
            AccountId = accountId;
        }

        public int BookShelfId { get; }
        public int AccountId { get; }
        public FileModel Image { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public FileModel File { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateBookShelfImageRequestHandler : RequestHandlerAsync<UpdateBookShelfImageRequest>
    {
        private readonly IBookShelfRepository _BookShelfRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public UpdateBookShelfImageRequestHandler(IBookShelfRepository BookShelfRepository, IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _BookShelfRepository = BookShelfRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        public override async Task<UpdateBookShelfImageRequest> HandleAsync(UpdateBookShelfImageRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var bookShelf = await _BookShelfRepository.GetBookShelfById(command.LibraryId, command.BookShelfId, cancellationToken);

            if (bookShelf == null)
            {
                throw new NotFoundException();
            }

            if (bookShelf.AccountId != command.AccountId)
            {
                throw new ForbiddenException();
            }

            if (bookShelf.ImageId.HasValue)
            {
                command.Image.Id = bookShelf.ImageId.Value;

                var existingImage = await _fileRepository.GetFileById(bookShelf.ImageId.Value, cancellationToken);
                if (existingImage != null && !string.IsNullOrWhiteSpace(existingImage.FilePath))
                {
                    await _fileStorage.TryDeleteImage(existingImage.FilePath, cancellationToken);
                }

                var url = await AddImageToFileStore(bookShelf.Id, command.Image.FileName, command.Image.Contents, command.Image.MimeType, cancellationToken);
                command.Image.FilePath = url;
                command.Image.IsPublic = true;
                await _fileRepository.UpdateFile(command.Image, cancellationToken);
                command.Result.File = command.Image;
                command.Result.File.Id = bookShelf.ImageId.Value;
            }
            else
            {
                command.Image.Id = default;
                var url = await AddImageToFileStore(bookShelf.Id, command.Image.FileName, command.Image.Contents, command.Image.MimeType, cancellationToken);
                command.Image.FilePath = url;
                command.Image.IsPublic = true;
                command.Result.File = await _fileRepository.AddFile(command.Image, cancellationToken);
                command.Result.HasAddedNew = true;

                await _BookShelfRepository.UpdateBookShelfImage(command.LibraryId, command.BookShelfId, command.Result.File.Id, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }

        private async Task<string> AddImageToFileStore(int BookShelfId, string fileName, byte[] contents, string mimeType, CancellationToken cancellationToken)
        {
            var filePath = GetUniqueFileName(BookShelfId, fileName);
            return await _fileStorage.StoreImage(filePath, contents, mimeType, cancellationToken);
        }

        private static string GetUniqueFileName(int BookShelfId, string fileName)
        {
            var fileNameWithourExtension = Path.GetExtension(fileName).Trim('.');
            return $"BookShelf/{BookShelfId}/title.{fileNameWithourExtension}";
        }
    }
}
