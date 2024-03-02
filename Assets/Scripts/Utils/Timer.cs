using UnityEngine;

namespace YuzuValen.Utils
{
    public class Timer
    {
        private float startTime = 0;

        public void Start()
        {
            startTime = Time.time;
        }
        public void Reset() => Start();
        public float GetTimeElapsed()
        {
            return Time.time - startTime;
        }
        #region operator overloads
        public static bool operator >(Timer timer, float duration)
            => timer.GetTimeElapsed() > duration;

        public static bool operator <(Timer timer, float duration)
            => timer.GetTimeElapsed() < duration;

        public static bool operator >=(Timer timer, float duration)
            => timer.GetTimeElapsed() >= duration;

        public static bool operator <=(Timer timer, float duration)
            => timer.GetTimeElapsed() <= duration;
        #endregion
    }
}
