using Game.Runtime.Utilities.Patterns.EventBus;
using UnityEngine.UI;

namespace Game.Runtime.Components.Events
{
    public struct TargetEvent : IEvent
    {
        public Image targetFace;
    }
}