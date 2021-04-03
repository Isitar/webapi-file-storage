namespace Isitar.FileStorage.Application.Queries.AttachmentInSize
{
    using System;
    using System.IO;
    using Contract;
    using MediatR;

    public class AttachmentInSizeQuery: IRequest<(Stream content, Attachment attachment)>
    {
        public Guid Id { get; set; }
        public AttachmentSize Size { get; set; }
        public bool ReturnContent { get; set; } = true;
    }
}