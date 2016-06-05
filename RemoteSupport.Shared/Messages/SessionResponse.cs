using System;

namespace RemoteSupport.Shared.Messages
{
    [Serializable]
    public class SessionResponse : ResponseMessageBase
    {
        public SessionResponse(RequestMessageBase request)
            : base(request)
        {
        }

        public bool IsConfirmed { get; set; }
        public string Email { get; set; }
    }
}