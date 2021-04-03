namespace Isitar.FileStorage.WebApi.Controllers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Contract;
    using Microsoft.AspNetCore.Mvc;
    using Requests;

    [ApiController]
    public class AttachmentController : ControllerBase
    {
        private readonly IFileStorageProvider fileStorageProvider;

        public AttachmentController(IFileStorageProvider fileStorageProvider)
        {
            this.fileStorageProvider = fileStorageProvider;
        }

        [HttpPost(ApiRoutes.Attachment.CreateAttachment)]
        public async Task<IActionResult> CreateAttachmentAsync([FromForm] CreateAttachmentRequest createAttachmentRequest)
        {
            var ms = new MemoryStream();

            await createAttachmentRequest.File.CopyToAsync(ms);
            var id = await fileStorageProvider.SaveAsync(ms, createAttachmentRequest.File.FileName, createAttachmentRequest.File.ContentType);
            return Ok(id);
        }

        [HttpGet(ApiRoutes.Attachment.SingleAttachment)]
        public async Task<IActionResult> SingleAttachmentAsync(Guid id, AttachmentSize size = AttachmentSize.MEDIA_SIZE_ORIG)
        {
            var res = await fileStorageProvider.ReadAsync(id, size);
            res.content.Position = 0;
            return File(res.content, res.attachment.MimeType, res.attachment.Filename);
        }

        [HttpGet(ApiRoutes.Attachment.SingleAttachmentData)]
        public async Task<IActionResult> SingleAttachmentDataAsync(Guid id, AttachmentSize size = AttachmentSize.MEDIA_SIZE_ORIG)
        {
            var res = await fileStorageProvider.MetaDataAsync(id, size);
            return Ok(res);
        }

        [HttpDelete(ApiRoutes.Attachment.DeleteAttachment)]
        public async Task<IActionResult> DeleteAttachmentAsync(Guid id)
        {
            await fileStorageProvider.DeleteAsync(id);
            return Ok();
        }
    }
}