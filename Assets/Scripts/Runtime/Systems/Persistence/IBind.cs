using Game.Runtime.Utilities.Extensions;

namespace Game.Runtime.Systems.Persistence
{
    public interface IBind<TData> where TData : ISaveable
    {
        SerializableGuid Id { get; set; }

        void Bind(TData data);
    }
}