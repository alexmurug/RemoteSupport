using System;

namespace RemoteSupport.Shared.Messages
{
    [Serializable]
    public class ValidationResponse : ResponseMessageBase
    {
        public ValidationResponse(RequestMessageBase request)
            : base(request)
        {
        }

        public bool IsValid { get; set; }
    }
}