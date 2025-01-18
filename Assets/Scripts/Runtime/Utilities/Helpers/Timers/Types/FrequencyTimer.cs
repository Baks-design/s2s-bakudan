using System;
using UnityEngine;

namespace Game.Runtime.Utilities.Helpers.Timers
{
    /// <summary>
    /// Timer that ticks at a specific frequency. (N times per second)
    /// </summary>
    public class FrequencyTimer : Timer
    {
        private float _timeThreshold;

        public int TicksPerSecond { get; private set; }
        public override bool IsFinished => !IsRunning;

        public event Action OnTick = delegate { };

        public FrequencyTimer(int ticksPerSecond) : base(0) => CalculateTimeThreshold(ticksPerSecond);

        public override void Tick()
        {
            if (IsRunning && CurrentTime >= _timeThreshold)
            {
                CurrentTime -= _timeThreshold;
                OnTick.Invoke();
            }

            if (IsRunning && CurrentTime < _timeThreshold)
                CurrentTime += Time.deltaTime;
        }

        public override void Reset() => CurrentTime = 0f;

        public void Reset(int newTicksPerSecond)
        {
            CalculateTimeThreshold(newTicksPerSecond);
            Reset();
        }

        private void CalculateTimeThreshold(int ticksPerSecond)
        {
            TicksPerSecond = ticksPerSecond;
            _timeThreshold = 1f / TicksPerSecond;
        }
    }
}