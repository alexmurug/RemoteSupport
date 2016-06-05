using System;
using RemoteSupport.Shared.Messages;

namespace RemoteSupport.Client.UI.MessagesExtensions
{
    [Serializable]
    public class CalcMessageResponse : GenericResponse
    {
        public CalcMessageResponse(CalcMessageRequest request)
            : base(request)
        {
        }

        public int Result { get; set; }
    }
}