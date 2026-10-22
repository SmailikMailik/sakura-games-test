using System;
using VContainer;

namespace Project.Runtime.Randomness
{
    /// <summary>
    /// Хранит случайную последовательность приложения независимо от UnityEngine.Random.
    /// </summary>
    internal sealed class SystemRandomSource : IRandomSource
    {
        private readonly Random _random;

        [Inject]
        internal SystemRandomSource()
        {
            _random = new();
        }

        internal SystemRandomSource(int seed)
        {
            _random = new(seed);
        }

        /// <summary>
        /// Возвращает целое значение от minimum включительно до maximum исключительно.
        /// </summary>
        public int Range(int minimum, int maximum)
        {
            return _random.Next(minimum, maximum);
        }

        /// <summary>
        /// Возвращает вещественное значение между minimum и maximum.
        /// </summary>
        public float Range(float minimum, float maximum)
        {
            var sample = (float)_random.NextDouble();
            return minimum + (maximum - minimum) * sample;
        }
    }
}