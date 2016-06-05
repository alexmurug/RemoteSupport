using System;

namespace RemoteSupport.Shared.Models
{
    public class ResponseCallbackObject
    {
        public Delegate CallBack { get; set; }
        public Guid Id { get; set; }
    }
}