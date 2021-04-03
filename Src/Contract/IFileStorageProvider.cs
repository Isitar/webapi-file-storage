namespace Isitar.FileStorage.Contract
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public interface IFileStorageProvider
    {
        /// <summary>
        /// Saves the provided content in file with the provided name
        /// </summary>
        /// <param name="content">the content to be written in the file, only supports utf-8</param>
        /// <param name="filename">the filename</param>
        /// <param name="mimeType">the mimetype</param>
        /// <returns>an attachment instance</returns>
        public Task<Guid> SaveAsync(string content, string filename, string mimeType);

        /// <summary>
        /// Saves the provided content in file with the provided name
        /// </summary>
        /// <param name="content">the content to be written in the file</param>
        /// <param name="filename">the filename</param>
        /// <param name="mimeType">the mimetype</param>
        /// <returns>an attachment instance</returns>
        public Task<Guid> SaveAsync(Stream content, string filename, string mimeType);

        /// <summary>
        /// Reads the content of a given guid
        /// throws if not found
        /// </summary>
        /// <param name="id">The id of the attachment</param>
        /// <param name="size">The size of the returned attachment</param>
        /// <returns>the content as stream and an attachment instance</returns>
        public Task<(Stream content, Attachment attachment)> ReadAsync(Guid id, AttachmentSize size = AttachmentSize.MEDIA_SIZE_ORIG);

        /// <summary>
        /// Returns the meta-data for a given guid
        /// </summary>
        /// <param name="id">The id of the attachment</param>
        /// <param name="size">The size of the returned attachment</param>
        /// <returns>the attachment instance</returns>
        public Task<Attachment> MetaDataAsync(Guid id, AttachmentSize size = AttachmentSize.MEDIA_SIZE_ORIG);
        
        /// <summary>
        /// Removes a file
        /// </summary>
        /// <param name="id">the id of the attachment</param>
        public Task DeleteAsync(Guid id);
    }
}