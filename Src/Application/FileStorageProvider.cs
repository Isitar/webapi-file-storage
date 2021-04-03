namespace Isitar.FileStorage.Application
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Commands.CreateAttachment;
    using Commands.DeleteAttachment;
    using Contract;
    using MediatR;
    using Queries.AttachmentInSize;

    public class FileStorageProvider : IFileStorageProvider
    {
        private readonly IMediator mediator;

        public FileStorageProvider(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public Task<Guid> SaveAsync(string content, string filename, string mimeType)
        {
            return SaveAsync(new MemoryStream(Encoding.UTF8.GetBytes(content)), filename, mimeType);
        }

        public async Task<Guid> SaveAsync(Stream content, string filename, string mimeType)
        {
            var id = Guid.NewGuid();
            await mediator.Send(new CreateAttachmentCommand
            {
                Id = id,
                Filename = filename,
                AttachmentStream = content,
                MimeType = mimeType
            });
            return id;
        }

        public Task<(Stream content, Attachment attachment)> ReadAsync(Guid id, AttachmentSize size = AttachmentSize.MEDIA_SIZE_ORIG)
        {
            return mediator.Send(new AttachmentInSizeQuery
            {
                Id = id,
                Size = size,
            });
        }

        public async Task<Attachment> MetaDataAsync(Guid id, AttachmentSize size = AttachmentSize.MEDIA_SIZE_ORIG)
        {
            var resp = await  mediator.Send(new AttachmentInSizeQuery
            {
                Id = id,
                Size = size,
                ReturnContent = false,
            });
            return resp.attachment;
        }

        public async Task DeleteAsync(Guid id)
        {
            await mediator.Send(new DeleteAttachmentCommand
            {
                Id = id
            });
        }
    }
}