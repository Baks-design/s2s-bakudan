using System.Collections.Generic;
using System.Linq;
using Game.Runtime.Systems.Scenes;
using Game.Runtime.Utilities.Patterns.ServiceLocator;
using UnityEngine;

namespace Game.Runtime.Systems.Persistence
{
    public class SerializeManager : MonoBehaviour, ISerializeService
    {
        [SerializeField] GameData gameData;
        IDataService dataService;
        ISceneLoaderService sceneLoaderService;

        public GameData GameData => gameData;

        void Awake() => dataService = new FileDataService(new JsonSerializer());

        void OnEnable() => Bind<PlayerPersistence, PlayerData>(gameData.playerData);

        void Start() => NewGame();

        void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
            if (entity != null)
            {
                data ??= new TData { Id = entity.Id };
                entity.Bind(data);
            }
        }

        void Bind<T, TData>(List<TData> datas) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entities = FindObjectsByType<T>(FindObjectsSortMode.None);
            foreach (var entity in entities)
            {
                var data = datas.FirstOrDefault(d => d.Id == entity.Id);
                if (data == null)
                {
                    data = new TData { Id = entity.Id };
                    datas.Add(data);
                }
                entity.Bind(data);
            }
        }

        public void NewGame()
        {
            gameData = new GameData
            {
                Name = "Game",
                CurrentLevelName = SceneID.NewGame
            };

            ServiceLocator.Global.Get(out sceneLoaderService);
            sceneLoaderService.LoadSceneGroup(gameData.CurrentLevelName.GetHashCode());
        }

        public void SaveGame() => dataService.Save(gameData);

        public void LoadGame(string gameName)
        {
            gameData = dataService.Load(gameName);
            gameData.CurrentLevelName = SceneID.LoadGame;

            ServiceLocator.Global.Get(out sceneLoaderService);
            sceneLoaderService.LoadSceneGroup(gameData.CurrentLevelName.GetHashCode());
        }

        public void ReloadGame() => LoadGame(gameData.Name);

        public void DeleteGame(string gameName) => dataService.Delete(gameName);
    }
}