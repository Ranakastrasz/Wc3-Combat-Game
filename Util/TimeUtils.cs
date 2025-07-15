namespace Wc3_Combat_Game.Util
{
    public static class TimeUtils
    {
        public static bool HasElapsed(float currentTime, float lastEventTime, float duration) =>
            currentTime >= lastEventTime + duration;

        public static float Lerp(float start, float end, float t) =>
            start + (end - start) * t;

    }


}
