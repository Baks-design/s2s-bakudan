using UnityEngine;
using Game.Runtime.Utilities.Patterns.EventBus;
using Game.Runtime.Systems.Events;
using Game.Runtime.Utilities.Helpers.Timers;

namespace Game.Runtime.Systems.GameManagement
{
    public class GameTime : MonoBehaviour
    {
        readonly CountdownTimer gameCountdown = new(300f);

        void Start() => gameCountdown.Start();

        void Update() => EventBus<UIEvent>.Raise(new UIEvent { currentGameTime = gameCountdown.CurrentTime });
    }
}