using RemoteSupport.Client.EventArguments;

namespace RemoteSupport.Client
{
    public static class Delegates
    {
        public delegate void FileUploadRequestDelegate(Client client, FileUploadRequestEventArguments args);

        public delegate void SessionRequestDelegate(Client client, SessionRequestEventArguments args);
    }
}