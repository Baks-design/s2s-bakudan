namespace Game.Runtime.Systems.Persistence
{
    public interface ISerializeService
    {
        void NewGame();
        void SaveGame();
        void LoadGame(string gameName);
        void ReloadGame();
        void DeleteGame(string gameName);
    }
}