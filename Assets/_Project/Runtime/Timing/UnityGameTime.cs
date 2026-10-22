using UnityEngine;

namespace Project.Runtime.Timing
{
    /// <summary>
    /// Связывает игровое время приложения с часами и масштабом времени Unity.
    /// </summary>
    internal sealed class UnityGameTime : IGameTime
    {
        public double TimeAsDouble => Time.timeAsDouble;
        public float DeltaTime => Time.deltaTime;
        public float TimeScale
        {
            get => Time.timeScale;
            set => Time.timeScale = value;
        }
    }
}