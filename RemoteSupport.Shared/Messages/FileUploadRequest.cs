using System;

namespace RemoteSupport.Shared.Messages
{
    [Serializable]
    public class FileUploadRequest : RequestMessageBase
    {
        public FileUploadRequest()
        {
            BufferSize = 1024;
        }

        public FileUploadRequest(FileUploadResponse response)
            : this()
        {
            CallbackId = response.CallbackId;
            FileName = response.FileName;
            TotalBytes = response.TotalBytes;
            CurrentPosition = response.CurrentPosition;
            SourceFilePath = response.SourceFilePath;
            DestinationFilePath = response.DestinationFilePath;
        }

        public string FileName { get; set; }
        public long TotalBytes { get; set; }
        public int CurrentPosition { get; set; }
        public string SourceFilePath { get; set; }
        public string DestinationFilePath { get; set; }
        public byte[] BytesToWrite { get; set; }
        public int BufferSize { get; set; }
    }
}