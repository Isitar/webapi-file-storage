namespace Isitar.FileStorage.Application.Interfaces
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Contract;
    using Attachment = Entities.Attachment;

    public interface IFileDb
    {
        public IQueryable<Attachment> Attachments();
        public Task<Attachment> ByIdAsync(Guid id);
        public Task<Attachment> ByIdAndSizeAsync(Guid id, AttachmentSize size);
        public Task AddAsync(Attachment attachment, CancellationToken cancellationToken);
        public Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        public Task CommitAsync(CancellationToken cancellationToken);
        public Task RollbackAsync(CancellationToken cancellationToken);
    }
}