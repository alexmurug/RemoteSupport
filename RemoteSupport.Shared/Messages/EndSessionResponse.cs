using System;

namespace RemoteSupport.Shared.Messages
{
    [Serializable]
    public class EndSessionResponse : ResponseMessageBase
    {
        public EndSessionResponse(EndSessionRequest request)
            : base(request)
        {
        }
    }
}