using UnityEngine;
using Game.Runtime.Utilities.Patterns.EventBus;
using Game.Runtime.Utilities.Helpers.Timers;
using Game.Runtime.Components.Events;

namespace Game.Runtime.Systems.Management
{
    public class GameTime : MonoBehaviour
    {
        readonly CountdownTimer gameCountdown = new(300f);

        void Start() => gameCountdown.Start();

        void Update() => EventBus<UIEvent>.Raise(new UIEvent { currentGameTime = gameCountdown.CurrentTime });
    }
}