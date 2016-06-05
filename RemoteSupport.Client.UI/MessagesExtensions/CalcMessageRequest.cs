using System;
using RemoteSupport.Shared.Messages;

namespace RemoteSupport.Client.UI.MessagesExtensions
{
    [Serializable]
    public class CalcMessageRequest : GenericRequest
    {
        public int A { get; set; }
        public int B { get; set; }
    }
}