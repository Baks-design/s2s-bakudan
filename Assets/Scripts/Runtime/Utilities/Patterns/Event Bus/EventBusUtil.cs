using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Game.Runtime.Utilities.Patterns.EventBus
{
    /// <summary>
    /// Contains methods and properties related to event buses and event types in the Unity application.
    /// </summary>
    public static class EventBusUtil
    {
        static readonly Lazy<IReadOnlyList<Type>> _eventTypes = new(() => PredefinedAssemblyUtil.GetTypes(typeof(IEvent)));
        static readonly Lazy<IReadOnlyList<Type>> _eventBusTypes = new(InitializeAllBuses);
        static readonly Dictionary<Type, MethodInfo> _clearMethodsCache = new();

        public static IReadOnlyList<Type> EventTypes => _eventTypes.Value;
        public static IReadOnlyList<Type> EventBusTypes => _eventBusTypes.Value;
#if UNITY_EDITOR
        public static PlayModeStateChange PlayModeState { get; set; }

        [InitializeOnLoadMethod]
        public static void InitializeEditor()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            PlayModeState = state;
            if (state == PlayModeStateChange.ExitingPlayMode)
                ClearAllBuses();
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            // Initialization is handled by Lazy<T>
        }

        static List<Type> InitializeAllBuses()
        {
            if (EventTypes == null)
            {
                Debug.LogError("EventTypes is not initialized.");
                return new List<Type>();
            }

            var typedef = typeof(EventBus<>);
            return EventTypes.Select(eventType => typedef.MakeGenericType(eventType)).ToList();
        }

        public static void ClearAllBuses()
        {
            if (EventBusTypes == null)
            {
                Debug.LogError("EventBusTypes is not initialized.");
                return;
            }

            Debug.Log("Clearing all buses...");
            foreach (var busType in EventBusTypes)
            {
                if (!_clearMethodsCache.TryGetValue(busType, out var clearMethod))
                {
                    clearMethod = busType.GetMethod("Clear", BindingFlags.Static | BindingFlags.NonPublic);
                    if (clearMethod == null)
                    {
                        Debug.LogError($"Clear method not found for {busType.Name}");
                        continue;
                    }
                    _clearMethodsCache[busType] = clearMethod;
                }
                clearMethod.Invoke(null, null);
            }
        }
    }
}