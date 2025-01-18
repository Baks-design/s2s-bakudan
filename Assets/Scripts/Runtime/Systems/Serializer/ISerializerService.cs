namespace Game.Runtime.Systems.Serializer
{
    public interface ISerializerService
    {
        void NewGame();
        void LoadGame(string gameName);
        void ReloadGame();
        void DeleteGame(string gameName);
        void SaveGame();
    }
}