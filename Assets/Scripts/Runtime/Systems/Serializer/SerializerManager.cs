using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Runtime.Systems.Serializer
{
    public class SerializerManager : MonoBehaviour, ISerializerService
    {
        [SerializeField] GameData gameData;
        IDataService dataService;

        public GameData GameData => gameData;

        void Awake() => dataService = new FileDataService(new JsonSerializer());

        void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

        void Start() => NewGame();

        void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Menu")
                return;

            Bind<PlayerDataHandler, PlayerData>(gameData.playerData);
        }

        void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();

            if (entity != null)
            {
                data ??= new TData { Id = entity.ID };
                entity.Bind(data);
            }
        }

        void Bind<T, TData>(List<TData> datas) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entities = FindObjectsByType<T>(FindObjectsSortMode.None);

            foreach (var entity in entities)
            {
                var data = datas.FirstOrDefault(d => d.Id == entity.ID);
                if (data == null)
                {
                    data = new TData { Id = entity.ID };
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
                CurrentLevelName = "Demo"
            };

            SceneManager.LoadScene(gameData.CurrentLevelName);
        }

        public void SaveGame() => dataService.Save(gameData);

        public void LoadGame(string gameName)
        {
            gameData = dataService.Load(gameName);

            if (string.IsNullOrWhiteSpace(gameData.CurrentLevelName))
                gameData.CurrentLevelName = "Demo";

            SceneManager.LoadScene(gameData.CurrentLevelName);
        }

        public void ReloadGame() => LoadGame(gameData.Name);

        public void DeleteGame(string gameName) => dataService.Delete(gameName);
    }
}