using System;

namespace Game.Runtime.Components.UI
{
    public struct LoadingProgress : IProgress<float>
    {
        const float ratio = 1f;

        public event Action<float> Progressed;

        public readonly void Report(float value) => Progressed?.Invoke(value / ratio);
    }
}