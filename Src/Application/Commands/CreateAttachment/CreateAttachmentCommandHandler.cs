namespace Isitar.FileStorage.Application.Commands.CreateAttachment
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Contract;
    using ImageMagick;
    using Interfaces;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Settings;
    using Attachment = Entities.Attachment;

    public class CreateAttachmentCommandHandler : IRequestHandler<CreateAttachmentCommand>
    {
        private readonly IFileDb fileDb;
        private readonly SizeSettings sizeSettings;
        private readonly FileStorageSettings fileStorageSettings;
        private readonly ILogger<CreateAttachmentCommandHandler> logger;

        public CreateAttachmentCommandHandler(IFileDb fileDb, SizeSettings sizeSettings, FileStorageSettings fileStorageSettings, ILogger<CreateAttachmentCommandHandler> logger)
        {
            this.fileDb = fileDb;
            this.sizeSettings = sizeSettings;
            this.fileStorageSettings = fileStorageSettings;
            this.logger = logger;
        }

        private List<string> storedFiles = new List<string>();
        
        private async Task SaveFileAsync(Stream content, string fileName)
        {
            try
            {
                var fullSavePath = Path.Combine(fileStorageSettings.BasePath, fileName);
                ;
                if (!Directory.Exists(Path.GetDirectoryName(fullSavePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fullSavePath));
                }

                var fs = File.Create(fullSavePath);
                content.Seek(0, SeekOrigin.Begin);
                await content.CopyToAsync(fs);
                fs.Close();
            }
            catch (Exception e)
            {
                logger.LogError($"Error saving file: {e}\n{e.StackTrace}");
            }
          
            storedFiles.Add(fileName);
        }
        
        
        public async Task<Unit> Handle(CreateAttachmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var stream = new MemoryStream();
                request.AttachmentStream.Position = 0;
                await request.AttachmentStream.CopyToAsync(stream, cancellationToken);

                var attachment = new Attachment
                {
                    Id = request.Id,
                    Filename = request.Filename!.Trim(),
                    MimeType = request.MimeType.Trim(),
                    Size = AttachmentSize.MEDIA_SIZE_ORIG,
                    FileSize = stream.Length,
                };
                var filename = attachment.RealFilename();
                await SaveFileAsync(stream, filename);

                stream.Position = 0;
  

                await fileDb.AddAsync(attachment, cancellationToken);


                if (request.MimeType?.Trim().StartsWith("image") ?? false)
                {

                    foreach (var settingEntry in sizeSettings.Settings)
                    {
                        stream.Position = 0;
                        using var img = new MagickImage(stream);
                        var size = new MagickGeometry(settingEntry.MaxWidth, settingEntry.MaxHeight);
                        switch (Path.GetExtension(request.Filename))
                        {
                            case ".jpg":
                                img.Format = MagickFormat.Pjpeg;
                                img.Interlace = Interlace.Jpeg;
                                break;
                            case ".png":
                                img.Interlace = Interlace.Png;
                                break;
                        }

                        img.Resize(size);
                        img.Quality = settingEntry.Quality;


                        var resizedStream = new MemoryStream();
                        await img.WriteAsync(resizedStream);
                        var fileSize = resizedStream.Length;
                        var resizedAttachment = new Attachment()
                        {
                            Id = Guid.NewGuid(),
                            OriginalAttachmentId = request.Id,
                            Filename = request.Filename?.Trim(),
                            MimeType = request.MimeType?.Trim(),
                            FileSize = fileSize,
                            Size = settingEntry.Size,
                        };
                        await SaveFileAsync(resizedStream, resizedAttachment.RealFilename());
                        await fileDb.AddAsync(resizedAttachment, cancellationToken);
                    }
                }

                await fileDb.CommitAsync(cancellationToken);
            }
            catch
            {
                foreach (var storedFile in storedFiles)
                {
                    try
                    {
                        File.Delete(storedFile);
                    }
                    catch
                    {
                        // ignored
                    }
                }
                throw;
            }

            return Unit.Value;
        }
    }
}