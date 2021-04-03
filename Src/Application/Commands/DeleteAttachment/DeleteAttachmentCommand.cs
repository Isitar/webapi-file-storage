namespace Isitar.FileStorage.Application.Commands.DeleteAttachment
{
    using System;
    using MediatR;

    public class DeleteAttachmentCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}