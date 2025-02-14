using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.Utilities.Helpers
{
    public static class WaitFor
    {
        public static WaitForFixedUpdate FixedUpdate { get; } = new WaitForFixedUpdate();
        public static WaitForEndOfFrame EndOfFrame { get; } = new WaitForEndOfFrame();
        static readonly Dictionary<float, WaitForSeconds> WaitForSecondsDict = new(100, new FloatComparer());

        public static WaitForSeconds Seconds(float seconds)
        {
            if (seconds < 1f / Application.targetFrameRate)
                return null;

            if (!WaitForSecondsDict.TryGetValue(seconds, out var forSeconds))
            {
                forSeconds = new WaitForSeconds(seconds);
                WaitForSecondsDict[seconds] = forSeconds;
            }

            return forSeconds;
        }

        class FloatComparer : IEqualityComparer<float>
        {
            const float Tolerance = 1e-5f;

            public bool Equals(float x, float y) => Mathf.Abs(x - y) <= Tolerance;

            public int GetHashCode(float obj) => obj.GetHashCode();
        }
    }
}