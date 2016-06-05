using System;
using System.IO;

namespace RemoteSupport.Shared.Messages
{
    [Serializable]
    public class RemoteDesktopResponse : ResponseMessageBase
    {
        public RemoteDesktopResponse(RequestMessageBase request)
            : base(request)
        {
            DeleteCallbackAfterInvoke = false;
        }

        public MemoryStream FrameBytes { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public bool Cancel { get; set; }
    }
}