namespace Isitar.FileStorage.Application.Settings
{
    using System.Collections.Generic;
    using Contract;

    public class SizeSettingEntry
    {
        public AttachmentSize Size { get; set; }
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
        public bool KeepAspectRatio { get; set; }
        public int Quality { get; set; }
    }

    public class SizeSettings
    {
        public ICollection<SizeSettingEntry> Settings = new List<SizeSettingEntry>();
    }
}