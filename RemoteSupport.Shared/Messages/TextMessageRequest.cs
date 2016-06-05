using System;

namespace RemoteSupport.Shared.Messages
{
    [Serializable]
    public class TextMessageRequest : RequestMessageBase
    {
        public string Message { get; set; }
    }
}