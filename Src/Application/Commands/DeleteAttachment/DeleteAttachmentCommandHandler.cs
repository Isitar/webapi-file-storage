namespace Isitar.FileStorage.Application.Commands.DeleteAttachment
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Exceptions;
    using Interfaces;
    using MediatR;
    using Settings;

    public class DeleteAttachmentCommandHandler : IRequestHandler<DeleteAttachmentCommand>
    {
        private readonly IFileDb fileDb;
        private readonly FileStorageSettings storageSettings;

        public DeleteAttachmentCommandHandler(IFileDb fileDb, FileStorageSettings storageSettings)
        {
            this.fileDb = fileDb;
            this.storageSettings = storageSettings;
        }

        private async Task DeleteFile(string filename)
        {
            File.Delete(Path.Combine(storageSettings.BasePath, filename));
            await Task.CompletedTask;
        }

        public async Task<Unit> Handle(DeleteAttachmentCommand request, CancellationToken cancellationToken)
        {
            var attachment = await fileDb.ByIdAsync(request.Id);
            if (null == attachment)
            {
                throw new AttachmentNotFoundException(request.Id);
            }

            foreach (var subAttachment in attachment.SubAttachments)
            {
                await DeleteFile(subAttachment.RealFilename());
            }

            await DeleteFile(attachment.RealFilename());

            await fileDb.DeleteAsync(request.Id, cancellationToken);
            await fileDb.CommitAsync(cancellationToken);
            return Unit.Value;
        }
    }
}