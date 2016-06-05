using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace RemoteSupport.Shared.Messages
{
    [Serializable]
    public class GenericResponse : ResponseMessageBase
    {
        public GenericResponse(GenericRequest request)
            : base(request)
        {
            InnerMessage = new MemoryStream();
        }

        public GenericResponse(GenericResponse response)
            : this(new GenericRequest())
        {
            CallbackId = response.CallbackId;
            var f = new BinaryFormatter();
            f.Serialize(InnerMessage, response);
            InnerMessage.Position = 0;
        }

        internal MemoryStream InnerMessage { get; set; }

        public GenericResponse ExtractInnerMessage()
        {
            var f = new BinaryFormatter();
            f.Binder = new AllowAllAssemblyVersionsDeserializationBinder();
            return f.Deserialize(InnerMessage) as GenericResponse;
        }
    }
}