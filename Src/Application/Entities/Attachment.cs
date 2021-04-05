namespace Isitar.FileStorage.Application.Entities
{
    using System;
    using System.Collections.Generic;
    using Contract;

    public class Attachment : Contract.Attachment
    {
        public Guid? OriginalAttachmentId { get; set; }
        public AttachmentSize Size { get; set; }

        public virtual ICollection<Attachment> SubAttachments { get; set; } = new HashSet<Attachment>();

        public Contract.Attachment Base()
        {
            return new Contract.Attachment
            {
                Id = Id,
                Filename = Filename,
                MimeType = MimeType,
                FileSize = FileSize,
            };
        }
    }
}