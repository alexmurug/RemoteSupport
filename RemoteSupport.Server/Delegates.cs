using RemoteSupport.Server.EventArguments;

namespace RemoteSupport.Server
{
    public static class Delegates
    {
        public delegate void ClientBasicDelegate(Receiver receiver);

        public delegate void ClientValidatingDelegate(ClientValidatingEventArgs args);
    }
}

