namespace Isitar.FileStorage.Application.Queries.AttachmentInSize
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Contract;
    using Exceptions;
    using ExtensionMethods;
    using Interfaces;
    using MediatR;
    using Settings;

    public class AttachmentInSizeQueryHandler : IRequestHandler<AttachmentInSizeQuery, (Stream content, Attachment attachment)>
    {
        private readonly IFileDb fileDb;
        private readonly FileStorageSettings storageSettings;

        public AttachmentInSizeQueryHandler(IFileDb fileDb, FileStorageSettings storageSettings)
        {
            this.fileDb = fileDb;
            this.storageSettings = storageSettings;
        }

        private async Task<Stream> OpenAsync(string filename)
        {
            var fs = new FileStream(Path.Combine(storageSettings.BasePath, filename), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            fs.Seek(0, SeekOrigin.Begin);
            var ms = new MemoryStream();
            await fs.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public async Task<(Stream content, Attachment attachment)> Handle(AttachmentInSizeQuery request, CancellationToken cancellationToken)
        {
            var attachment = await fileDb.ByIdAndSizeAsync(request.Id, request.Size) ?? await fileDb.ByIdAsync(request.Id);

            if (null == attachment)
            {
                throw new AttachmentNotFoundException(request.Id);
            }

            var content = request.ReturnContent
                ? await OpenAsync(attachment.RealFilename())
                : null;
            return (content, attachment.Base());
        }
    }
}