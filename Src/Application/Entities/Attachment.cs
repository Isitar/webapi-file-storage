namespace Isitar.FileStorage.Application.Entities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Contract;

    public class Attachment : Contract.Attachment
    {
        public Guid? OriginalAttachmentId { get; set; }
        public AttachmentSize Size { get; set; }

        public virtual ICollection<Attachment> SubAttachments { get; set; } = new HashSet<Attachment>();

        public string RealFilename()
        {
            var extension = Path.GetExtension(Filename);
            var suffix = Size switch
            {
                AttachmentSize.MEDIA_SIZE_ORIG => string.Empty,
                AttachmentSize.MEDIA_SIZE_S => "_S",
                AttachmentSize.MEDIA_SIZE_M => "_M",
                AttachmentSize.MEDIA_SIZE_L => "_L",
                _ => "",
            };
            return $"{OriginalAttachmentId ?? Id}{suffix}{extension}";
        }
    }
}