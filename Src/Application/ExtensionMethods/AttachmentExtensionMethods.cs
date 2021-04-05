namespace Isitar.FileStorage.Application.ExtensionMethods
{
    using System.IO;
    using Contract;
    using Attachment = Entities.Attachment;

    public static class AttachmentExtensionMethods
    {
        public static string RealFilename(this Attachment attachment)
        {
            var extension = Path.GetExtension(attachment.Filename);
            var suffix = attachment.Size switch
            {
                AttachmentSize.MEDIA_SIZE_ORIG => string.Empty,
                AttachmentSize.MEDIA_SIZE_S => "_S",
                AttachmentSize.MEDIA_SIZE_M => "_M",
                AttachmentSize.MEDIA_SIZE_L => "_L",
                _ => "",
            };
            return $"{attachment.OriginalAttachmentId ?? attachment.Id}{suffix}{extension}";
        }
    }
}