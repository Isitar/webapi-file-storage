namespace Isitar.FileStorage.Application.Commands.CreateAttachment
{
    using System;
    using System.IO;
    using MediatR;

    public class CreateAttachmentCommand : IRequest
    {
        public Guid Id { get; set; }
        public Stream AttachmentStream { get; set; }

        public string Filename { get; set; }
        public string MimeType { get; set; }
    }
}