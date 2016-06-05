using System;

namespace RemoteSupport.Shared.Messages
{
    [Serializable]
    public class SessionRequest : RequestMessageBase
    {
        public string Email { get; set; }
    }
}