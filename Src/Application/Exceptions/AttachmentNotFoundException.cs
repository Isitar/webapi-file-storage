namespace Isitar.FileStorage.Application.Exceptions
{
    using System;

    public class AttachmentNotFoundException : Exception
    {
        public AttachmentNotFoundException(Guid id) : base()
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}