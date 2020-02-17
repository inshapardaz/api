using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;

namespace Inshapardaz.Ports.Database.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly IDatabaseContext _databaseContext;

        public FileRepository(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<FileModel> GetFileById(int id, bool isPublic, CancellationToken cancellationToken)
        {
            var file = await _databaseContext.File.SingleOrDefaultAsync(i => i.Id == id && (!isPublic || i.IsPublic == isPublic), cancellationToken);
            return file.Map();
        }

        public async Task<FileModel> AddFile(FileModel file, string url, bool isPublic, CancellationToken cancellationToken)
        {
            var entity = file.Map();

            entity.FilePath = url;
            entity.IsPublic = isPublic;
            entity.DateCreated = DateTime.UtcNow;

            _databaseContext.File.Add(entity);
            await _databaseContext.SaveChangesAsync(cancellationToken);

            return entity.Map();
        }

        public async Task<FileModel> UpdateFile(FileModel file, string url, bool isPublic, CancellationToken cancellationToken)
        {
            var entity = await _databaseContext.File.SingleOrDefaultAsync(f => f.Id == file.Id, cancellationToken);
            if (entity == null)
            {
                throw new NotFoundException();
            }

            entity.FilePath = url;
            entity.FileName = file.FileName;
            entity.MimeType = file.MimeType;
            entity.FilePath = url;
            entity.IsPublic = isPublic;
            entity.DateCreated = DateTime.UtcNow;

            await _databaseContext.SaveChangesAsync(cancellationToken);

            return entity.Map();
        }

        public async Task DeleteFile(int id, CancellationToken cancellationToken)
        {
            var entity = await _databaseContext.File.SingleOrDefaultAsync(f => f.Id == id, cancellationToken);
            if (entity == null)
            {
                throw new NotFoundException();
            }

            _databaseContext.File.Remove(entity);

            await _databaseContext.SaveChangesAsync(cancellationToken);
        }
    }
}
