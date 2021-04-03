namespace Isitar.FileStorage.Contract
{
    using System;

    /// <summary>
    /// Attachment meta data for a given attachment
    /// </summary>
    public class Attachment
    {
        /// <summary>
        /// An unique id, identifying the attachment
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// the filename provided during save
        /// </summary>
        public string Filename { get; set; }
        
        /// <summary>
        /// The mimetype if it can be found
        /// </summary>
        public string MimeType { get; set; }
        
        /// <summary>
        /// The file size in bytes 
        /// </summary>
        public long FileSize { get; set; }
    }
}