using System;

namespace RemoteSupport.Shared.Messages
{
    [Serializable]
    public class MessageBase
    {
        public MessageBase()
        {
            Exception = new Exception();
        }

        public Guid CallbackId { get; set; }
        public bool HasError { get; set; }
        public Exception Exception { get; set; }
    }
}