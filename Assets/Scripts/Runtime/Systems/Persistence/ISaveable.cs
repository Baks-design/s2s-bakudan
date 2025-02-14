using Game.Runtime.Utilities.Extensions;

namespace Game.Runtime.Systems.Persistence
{
    public interface ISaveable
    {
        SerializableGuid Id { get; set; }
    }
}