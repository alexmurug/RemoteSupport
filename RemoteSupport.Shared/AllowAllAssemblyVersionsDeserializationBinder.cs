using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace RemoteSupport.Shared
{
    public sealed class AllowAllAssemblyVersionsDeserializationBinder : SerializationBinder
    {
        // ReSharper disable once RedundantAssignment
        public override Type BindToType(string assemblyName, string typeName)
        {
            var currentAssembly = Assembly.GetExecutingAssembly().FullName;

            // In this case we are always using the current assembly
            assemblyName = currentAssembly;

            // Get the type using the typeName and assemblyName
            var typeToDeserialize = Type.GetType($"{typeName}, {assemblyName}");

            return typeToDeserialize;
        }
    }
}