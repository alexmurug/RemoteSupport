using System;

namespace RemoteSupport.Shared.Messages
{
    [Serializable]
    public class ValidationRequest : RequestMessageBase
    {
        public string Email { get; set; }
    }
}