namespace Game.Runtime.Systems.Serializer
{
    public interface ISaveable
    {
        SerializableGuid Id { get; set; }
    }
}