using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Ports.Database.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly IDatabaseContext _databaseContext;

        public FileRepository(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<File> GetFileById(int id, CancellationToken cancellationToken)
        {
            var file = await _databaseContext.File.SingleOrDefaultAsync(i => i.Id == id, cancellationToken);
            return file.Map<Entities.File, File>();
        }

        public async Task<File> AddFile(File file, string url, CancellationToken cancellationToken)
        {
            var entity = file.Map<File, Entities.File>();

            entity.FilePath = url;

            _databaseContext.File.Add(entity);
            await _databaseContext.SaveChangesAsync(cancellationToken);

            return entity.Map<Entities.File, File>();
        }

        public async Task<File> UpdateFile(File file, string url, CancellationToken cancellationToken)
        {
            var entity = await _databaseContext.File.SingleOrDefaultAsync(f => f.Id == file.Id, cancellationToken);
            if (entity == null)
            {
                throw new NotFoundException();
            }

            entity.FilePath = url;
            entity.FileName = file.FileName;
            entity.MimeType = file.MimeType;
            entity.LiveUntil = file.LiveUntil;
            entity.FilePath = url;

            await _databaseContext.SaveChangesAsync(cancellationToken);

            return entity.Map<Entities.File, File>();
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
