using UnityEngine;

namespace Game.Runtime.Utilities.Helpers.Timers
{
    /// <summary>
    /// Timer that counts down from a specific value to zero.
    /// </summary>
    public class CountdownTimer : Timer
    {
        public override bool IsFinished => CurrentTime <= 0f;

        public CountdownTimer(float value) : base(value) { }

        public override void Tick()
        {
            if (IsRunning && CurrentTime > 0f)
                CurrentTime -= Time.deltaTime;

            if (IsRunning && CurrentTime <= 0f)
                Stop();
        }
    }
}