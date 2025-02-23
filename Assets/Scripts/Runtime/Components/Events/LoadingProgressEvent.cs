using System;
using Game.Runtime.Utilities.Patterns.EventBus;

namespace Game.Runtime.Components.Events
{
    public struct LoadingProgressEvent : IEvent
    {
        public IProgress<float> progress; 
    }
}