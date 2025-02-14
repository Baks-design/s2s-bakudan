using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Runtime.Utilities.Patterns.ServiceLocator
{
    [DisallowMultipleComponent]
    public class ServiceLocator : MonoBehaviour
    {
        static ServiceLocator _global;
        static readonly Dictionary<Scene, ServiceLocator> _sceneContainers = new();
        static readonly List<GameObject> _tmpSceneGameObjects = new();
        readonly ServiceManager _services = new();
        const string GlobalServiceLocatorName = "ServiceLocator [Global]";
        const string SceneServiceLocatorName = "ServiceLocator [Scene]";

        internal void ConfigureAsGlobal(bool dontDestroyOnLoad)
        {
            if (_global == this)
            {
                Debug.LogWarning("ServiceLocator.ConfigureAsGlobal: Already configured as global", this);
                return;
            }

            if (_global != null)
            {
                Debug.LogError(
                    "ServiceLocator.ConfigureAsGlobal: Another ServiceLocator is already configured as global", this);
                return;
            }

            _global = this;
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }

        internal void ConfigureForScene()
        {
            var scene = gameObject.scene;

            if (_sceneContainers.ContainsKey(scene))
            {
                Debug.LogError(
                    "ServiceLocator.ConfigureForScene: Another ServiceLocator is already configured for this scene", this);
                return;
            }

            _sceneContainers.Add(scene, this);
        }

        /// <summary>
        /// Gets the global ServiceLocator instance. Creates a new one if none exists.
        /// </summary>
        public static ServiceLocator Global
        {
            get
            {
                if (_global != null)
                    return _global;

                if (FindFirstObjectByType<ServiceLocatorGlobal>() is { } found)
                {
                    found.BootstrapOnDemand();
                    return _global;
                }

                var container = new GameObject(GlobalServiceLocatorName, typeof(ServiceLocator));
                container.AddComponent<ServiceLocatorGlobal>().BootstrapOnDemand();

                return _global;
            }
        }

        /// <summary>
        /// Returns the <see cref="ServiceLocator"/> configured for the scene of a MonoBehaviour. Falls back to the global instance.
        /// </summary>
        public static ServiceLocator ForSceneOf(MonoBehaviour mb)
        {
            var scene = mb.gameObject.scene;

            if (_sceneContainers.TryGetValue(scene, out var container) && container != mb)
                return container;

            _tmpSceneGameObjects.Clear();
            scene.GetRootGameObjects(_tmpSceneGameObjects);

            foreach (var go in _tmpSceneGameObjects)
            {
                if (go.TryGetComponent(out ServiceLocatorScene bootstrapper) && bootstrapper.Container != mb)
                {
                    bootstrapper.BootstrapOnDemand();
                    return bootstrapper.Container;
                }
            }

            return Global;
        }

        /// <summary>
        /// Gets the closest ServiceLocator instance to the provided MonoBehaviour in hierarchy, the ServiceLocator for its scene, or the global ServiceLocator.
        /// </summary>
        public static ServiceLocator For(MonoBehaviour mb)
        {
            if (mb == null)
                throw new ArgumentNullException(nameof(mb));

            var serviceLocator = mb.GetComponentInParent<ServiceLocator>();

            return serviceLocator != null ? serviceLocator : ForSceneOf(mb) ?? Global;
        }

        /// <summary>
        /// Registers a service to the ServiceLocator using the service's type.
        /// </summary>
        public ServiceLocator Register<T>(T service)
        {
            _services.Register(service);
            return this;
        }

        /// <summary>
        /// Registers a service to the ServiceLocator using a specific type.
        /// </summary>
        public ServiceLocator Register(Type type, object service)
        {
            _services.Register(type, service);
            return this;
        }

        /// <summary>
        /// Gets a service of a specific type. If no service of the required type is found, an error is thrown.
        /// </summary>
        public ServiceLocator Get<T>(out T service) where T : class
        {
            if (TryGetService(out service))
                return this;

            if (TryGetNextInHierarchy(out var container))
            {
                container.Get(out service);
                return this;
            }

            throw new ArgumentException($"ServiceLocator.Get: Service of type {typeof(T).FullName} not registered");
        }

        /// <summary>
        /// Allows retrieval of a service of a specific type. An error is thrown if the required service does not exist.
        /// </summary>
        public T Get<T>() where T : class
        {
            if (TryGetService(out T service))
                return service;

            if (TryGetNextInHierarchy(out var container))
                return container.Get<T>();

            throw new ArgumentException($"Could not resolve type '{typeof(T).FullName}'.");
        }

        /// <summary>
        /// Tries to get a service of a specific type. Returns whether or not the process is successful.
        /// </summary>
        public bool TryGet<T>(out T service) where T : class
        {
            if (TryGetService(out service))
                return true;

            return TryGetNextInHierarchy(out var container) && container.TryGet(out service);
        }

        bool TryGetService<T>(out T service) where T : class => _services.TryGet(out service);

        bool TryGetNextInHierarchy(out ServiceLocator container)
        {
            if (this == _global)
            {
                container = null;
                return false;
            }

            container = transform.parent != null
                ? transform.parent.GetComponentInParent<ServiceLocator>()
                : ForSceneOf(this);

            return container != null;
        }

        void OnDestroy()
        {
            if (this == _global)
                _global = null;
            else if (_sceneContainers.ContainsValue(this))
                _sceneContainers.Remove(gameObject.scene);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            _global = null;
            _sceneContainers.Clear();
            _tmpSceneGameObjects.Clear();
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/ServiceLocator/Add Global")]
        static void AddGlobal() => new GameObject(GlobalServiceLocatorName, typeof(ServiceLocatorGlobal));

        [MenuItem("GameObject/ServiceLocator/Add Scene")]
        static void AddScene() => new GameObject(SceneServiceLocatorName, typeof(ServiceLocatorScene));
#endif
    }
}