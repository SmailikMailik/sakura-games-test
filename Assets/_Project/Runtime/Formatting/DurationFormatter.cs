using System;

namespace Project.Runtime.Formatting
{
    /// <summary>
    /// Преобразует длительность в текстовое представление.
    /// </summary>
    internal static class DurationFormatter
    {
        private const int SecondsPerMinute = 60;

        /// <summary>
        /// Представляет полные секунды длительности в формате mm:ss без ограничения числа минут.
        /// </summary>
        internal static string FormatMinutesAndSeconds(double totalSeconds)
        {
            var minutes = Math.Floor(totalSeconds / SecondsPerMinute);
            var seconds = Math.Floor(totalSeconds % SecondsPerMinute);
            return $"{minutes:00}:{seconds:00}";
        }
    }
}