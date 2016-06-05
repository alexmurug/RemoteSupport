using System;
using RemoteSupport.Shared.Messages;

namespace RemoteSupport.Server.EventArguments
{
    public class ClientValidatingEventArgs
    {
        private readonly Action _confirmAction;
        private readonly Action _refuseAction;

        public ClientValidatingEventArgs(Action confirmAction, Action refuseAction)
        {
            _confirmAction = confirmAction;
            _refuseAction = refuseAction;
        }

        public Receiver Receiver { get; set; }
        public ValidationRequest Request { get; set; }

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