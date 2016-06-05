using System;

namespace RemoteSupport.Shared.Messages
{
    [Serializable]
    public class RemoteDesktopRequest : RequestMessageBase
    {
        public RemoteDesktopRequest()
        {
            Quality = 50;
        }

        public int Quality { get; set; }
    }
}