using System;

namespace RemoteSupport.Shared.Messages
{
    [Serializable]
    public class FileUploadResponse : ResponseMessageBase
    {
        public FileUploadResponse(FileUploadRequest request)
            : base(request)
        {
            FileName = request.FileName;
            TotalBytes = request.TotalBytes;
            CurrentPosition = request.CurrentPosition;
            SourceFilePath = request.SourceFilePath;
            DestinationFilePath = request.DestinationFilePath;
            DeleteCallbackAfterInvoke = false;
        }

        public string FileName { get; set; }
        public long TotalBytes { get; set; }
        public int CurrentPosition { get; set; }
        public string SourceFilePath { get; set; }
        public string DestinationFilePath { get; set; }
    }
}