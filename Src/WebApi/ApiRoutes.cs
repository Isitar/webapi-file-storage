namespace Isitar.FileStorage.WebApi
{
    public class ApiRoutes
    {
        private const string Root = "api";

        private const string Version = "v1";

        private const string Base = Root + "/" + Version;

        public static class Attachment
        {
            private const string AttachmentBase = Base + "/attachment";
            public const string CreateAttachment = AttachmentBase;
            public const string SingleAttachment = AttachmentBase + "/{id}";
            public const string SingleAttachmentData = AttachmentBase + "/{id}/data";
            public const string DeleteAttachment = AttachmentBase + "/{id}";
        }
    }
}