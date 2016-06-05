using System;

namespace RemoteSupport.Shared.Enums
{
    [Serializable]
    public enum StatusEnum
    {
        Connected,
        Disconnected,
        Validated,
        InSession
    }
}