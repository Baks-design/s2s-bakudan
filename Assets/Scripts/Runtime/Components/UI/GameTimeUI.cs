using UnityEngine;
using Game.Runtime.Utilities.Patterns.EventBus;
using UnityEngine.UI;
using Game.Runtime.Components.Events;

namespace Game.Runtime.Components.UI
{
    public class GameTimeUI : MonoBehaviour
    {
        [SerializeField] Text text;
        [SerializeField] float initGameTime = 300f;
        EventBinding<UIEvent> uiEventBinding;

        void OnEnable()
        {
            uiEventBinding = new EventBinding<UIEvent>(HandleUIEvent);
            EventBus<UIEvent>.Register(uiEventBinding);
        }

        void Start() => text.text = initGameTime.ToString();

        void OnDisable() => EventBus<UIEvent>.Deregister(uiEventBinding);

        void HandleUIEvent(UIEvent uIEvent) => text.text = uIEvent.currentGameTime.ToString();
    }
}
