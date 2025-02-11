using UnityEngine;
using Game.Runtime.Utilities.Patterns.ServiceLocator;
using Game.Runtime.Systems.Audio;
using Game.Runtime.Systems.VFX;
using Game.Runtime.Systems.Scenes;
using Game.Runtime.Utilities.Patterns.Flyweight;

namespace Game.Runtime.Systems.Management
{
    public class ServicesInitializer : MonoBehaviour
    {
        [SerializeField] GameManager gameManager;
        [SerializeField] MusicManager musicManager;
        [SerializeField] SoundManager soundManager;
        [SerializeField] EffectManager effectsManager;
        [SerializeField] SceneLoader sceneLoaderManager;
        [SerializeField] FlyweightFactory flyweightFactory;

        void Awake()
        {
            ServiceLocator.Global.Register<IGameService>(gameManager);
            ServiceLocator.Global.Register<IMusicService>(musicManager);
            ServiceLocator.Global.Register<ISoundService>(soundManager);
            ServiceLocator.Global.Register<IEffectService>(effectsManager);
            ServiceLocator.Global.Register<ISceneLoaderService>(sceneLoaderManager);
            ServiceLocator.Global.Register<IFlyweightService>(flyweightFactory);
        }
    }
}