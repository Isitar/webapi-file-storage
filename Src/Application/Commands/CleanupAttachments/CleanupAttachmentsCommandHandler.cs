namespace Isitar.FileStorage.Application.Commands.CleanupAttachments
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Interfaces;
    using MediatR;
    using Settings;

    public class CleanupAttachmentsCommandHandler : IRequestHandler<CleanupAttachmentsCommand>
    {
        private readonly IFileDb fileDb;
        private readonly FileStorageSettings fileStorageSettings;

        public CleanupAttachmentsCommandHandler(IFileDb fileDb, FileStorageSettings fileStorageSettings)
        {
            this.fileDb = fileDb;
            this.fileStorageSettings = fileStorageSettings;
        }
        
        public async Task<Unit> Handle(CleanupAttachmentsCommand request, CancellationToken cancellationToken)
        {
            foreach (var file in Directory.EnumerateFiles(fileStorageSettings.BasePath))
            {
                var id = Guid.Parse(Path.GetFileNameWithoutExtension(file).Split("_")[0]);
                if (null == await fileDb.ByIdAsync(id))
                {
                    File.Delete(file);
                }
            }
            
            return Unit.Value;
        }
    }
}