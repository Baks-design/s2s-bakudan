using UnityEngine;
using Game.Runtime.Utilities.Patterns.ServiceLocator;
using Game.Runtime.Systems.Audio;
using Game.Runtime.Systems.VFX;
using Game.Runtime.Systems.SceneManagement;
using Game.Runtime.Systems.Serializer;
using Game.Runtime.Systems.Spawn;

namespace Game.Runtime.Systems.GameManagement
{
    public class ServicesInitializer : MonoBehaviour
    {
        [SerializeField] GameManager gameManager;
        [SerializeField] MusicManager musicManager;
        [SerializeField] SoundManager soundManager;
        [SerializeField] EffectManager effectsManager;
        [SerializeField] SceneLoader sceneLoaderManager;
        [SerializeField] SerializerManager serializerManager;
        [SerializeField] CollectibleSpawnManager spawnManager;

        void Awake()
        {        
            ServiceLocator.Global.Register<IGameService>(gameManager);
            ServiceLocator.Global.Register<IMusicService>(musicManager);
            ServiceLocator.Global.Register<ISoundService>(soundManager);
            ServiceLocator.Global.Register<IEffectService>(effectsManager);
            ServiceLocator.Global.Register<ISceneLoaderService>(sceneLoaderManager);
            ServiceLocator.Global.Register<ISerializerService>(serializerManager);
            ServiceLocator.Global.Register<ISpawnService>(spawnManager);
        }
    }
}