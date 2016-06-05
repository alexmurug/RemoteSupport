using System;
using RemoteSupport.Shared.Messages;

namespace RemoteSupport.Client.EventArguments
{
    public class FileUploadRequestEventArguments
    {
        private readonly Action _confirmAction;
        private readonly Action _refuseAction;

        public FileUploadRequestEventArguments(Action confirmAction, Action refuseAction)
        {
            _confirmAction = confirmAction;
            _refuseAction = refuseAction;
        }

        public FileUploadRequest Request { get; set; }

        public void Confirm(string fileName)
        {
            Request.DestinationFilePath = fileName;
            _confirmAction();
        }

        public void Refuse()
        {
            _refuseAction();
        }
    }
}