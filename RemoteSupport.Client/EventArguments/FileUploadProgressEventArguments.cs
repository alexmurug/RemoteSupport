namespace RemoteSupport.Client.EventArguments
{
    public class FileUploadProgressEventArguments
    {
        public string DestinationPath { get; set; }
        public string FileName { get; set; }
        public int CurrentPosition { get; set; }
        public long TotalBytes { get; set; }
    }
}