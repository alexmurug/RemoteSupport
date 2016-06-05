using System;
using RemoteSupport.Shared.Messages;

namespace RemoteSupport.Client.EventArguments
{
    public class SessionRequestEventArguments
    {
        private readonly Action _confirmAction;
        private readonly Action _refuseAction;

        public SessionRequestEventArguments(Action confirmAction, Action refuseAction)
        {
            _confirmAction = confirmAction;
            _refuseAction = refuseAction;
        }

        public SessionRequest Request { get; set; }

        public void Confirm()
        {
            _confirmAction();
        }

        public void Refuse()
        {
            _refuseAction();
        }
    }
}