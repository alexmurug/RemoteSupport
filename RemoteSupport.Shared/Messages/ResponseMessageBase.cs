using System;

namespace RemoteSupport.Shared.Messages
{
    [Serializable]
    public class ResponseMessageBase : MessageBase
    {
        public ResponseMessageBase(RequestMessageBase request)
        {
            DeleteCallbackAfterInvoke = true;
            CallbackId = request.CallbackId;
        }

        public bool DeleteCallbackAfterInvoke { get; set; }
    }
}