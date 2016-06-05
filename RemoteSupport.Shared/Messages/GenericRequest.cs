using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace RemoteSupport.Shared.Messages
{
    [Serializable]
    public class GenericRequest : RequestMessageBase
    {
        public GenericRequest()
        {
            InnerMessage = new MemoryStream();
        }

        public GenericRequest(RequestMessageBase request)
            : this()
        {
            var f = new BinaryFormatter();
            f.Serialize(InnerMessage, request);
            InnerMessage.Position = 0;
        }

        internal MemoryStream InnerMessage { get; set; }

        public GenericRequest ExtractInnerMessage()
        {
            var f = new BinaryFormatter();
            f.Binder = new AllowAllAssemblyVersionsDeserializationBinder();
            return f.Deserialize(InnerMessage) as GenericRequest;
        }
    }
}