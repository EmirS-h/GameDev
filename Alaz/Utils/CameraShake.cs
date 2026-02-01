using System.Numerics;

namespace Alaz.Utils
{
    public static class CameraShake
    {
        private static float _shakeIntensity = 0f;
        private static float _shakeDuration = 0f;
        private static Random _rnd = new Random();

        /// <summary>
        /// Triggers a screen shake.
        /// </summary>
        /// <param name="duration">How long the shake lasts in seconds.</param>
        /// <param name="intensity">How many pixels the camera can shift.</param>
        public static void Shake(float duration, float intensity)
        {
            _shakeDuration = duration;
            _shakeIntensity = intensity;
        }

        public static Vector2 GetOffset(float dt)
        {
            if (_shakeDuration > 0)
            {
                _shakeDuration -= dt;

                float currentIntensity = _shakeIntensity * (_shakeDuration / 1.0f);
                float offsetX = (float)(_rnd.NextDouble() * 2.0 - 1.0) * _shakeIntensity;
                float offsetY = (float)(_rnd.NextDouble() * 2.0 - 1.0) * _shakeIntensity;

                return new Vector2(offsetX, offsetY);
            }

            return Vector2.Zero;
        }
    }
}
