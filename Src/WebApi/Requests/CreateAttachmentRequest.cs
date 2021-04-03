namespace Isitar.FileStorage.WebApi.Requests
{
    using Microsoft.AspNetCore.Http;

    public class CreateAttachmentRequest
    {
        public IFormFile File { get; set; }
    }
}