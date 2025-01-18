using Game.Runtime.Utilities;

namespace Game.Runtime.Systems.Serializer
{
    public interface IBind<TData> where TData : ISaveable
    {
        SerializableGuid ID { get; set; }
        
        void Bind(TData data);
    }
}