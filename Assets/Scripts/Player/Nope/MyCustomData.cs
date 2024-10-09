using Unity.Netcode; // Make sure this namespace is included for NetworkVariable
using Unity.Collections; // Required for FixedString usage if used
using System; // Required for [Serializable]

namespace ITKombat
{
    // Struct to store custom data
    [Serializable]
    public struct MyCustomData : INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public FixedString128Bytes message; // Use FixedString for network serialization

        // Serialize and deserialize the struct for network synchronization
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);
        }
    }
}
